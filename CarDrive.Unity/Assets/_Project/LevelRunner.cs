using Assets._Project.Architecture;
using Assets._Project.Architecture.DI;
using Assets._Project.Architecture.UI;
using Assets._Project.CameraControl;
using Assets._Project.Entities.Character;
using Assets._Project.GameStateControl;
using Assets._Project.Helpers;
using Assets._Project.Input;
using Assets._Project.SceneChange;
using Assets._Project.Systems.CheckPoint;
using Assets._Project.Systems.ChunkGeneration;
using Assets._Project.Systems.Collectables;
using Assets._Project.Systems.Collecting;
using Assets._Project.Systems.Damage;
using Assets._Project.Systems.Driving;
using Assets._Project.Systems.Progress;
using Assets._Project.Systems.Restart;
using Assets._Project.Systems.Shop;
using Assets._Project.Systems.Sound;
using Assets._Project.Systems.Tutorial;
using Assets._Project.Systems.WorldCentring;
using Cinemachine;
using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Assets._Project
{
    public class LevelRunner : RunnerWithAutomaticSystemsInitialization
    {
        [SerializeField]
        private Transform
            _camerasContainer,
            _chunksContainer,
            _entityContainer,
            _hudContainer,
            _popupContainer,
            _fxContainer,
            _tutorialContainer,
            _loadingScreenContainer;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private AudioSource _leveMusic;
        private IPlayerInput _playerInput;
        private Cinematographer _cinematographer;
        private CharacterCar _characterCar;
        private Player _player;
        private GameState _gameState;
        private Coroutiner _coroutiner;
        private SceneChanger _sceneChanger;
        private LoadingScreen _loadingScreen;
        private Money _money;
        private IItemDatabase _itemDatabase;
        private Inventory _inventory;
        private TutorialSystem _tutorialSystem;
        private UniversalAdditionalCameraData _additionalCameraData;
        private Camera _uiCamera;

        protected override async Task CreateSystems()
        {
            DIContainer projectContainer = FindObjectOfType<DIContainer>();
            LocalAssetLoader assetLoader = projectContainer.Get<LocalAssetLoader>();
            _sceneChanger = projectContainer.Get<SceneChanger>();
            _loadingScreen = projectContainer.Get<LoadingScreen>();
            _player = projectContainer.Get<Player>();
            _gameState = new(GameStates.WaitForRun);
            _coroutiner = projectContainer.Get<Coroutiner>();
            _money = projectContainer.Get<Money>();
            _itemDatabase = projectContainer.Get<IItemDatabase>();
            _playerInput = projectContainer.Get<IPlayerInput>();
            _cinematographer = projectContainer.Get<Cinematographer>();
            Camera playerCamera = projectContainer.Get<Camera>();
            _additionalCameraData = playerCamera.GetComponent<UniversalAdditionalCameraData>();
            _uiCamera = await assetLoader.LoadAndInstantiateAsync<Camera>("UI Camera", _camerasContainer);
            _canvas.worldCamera = _uiCamera;
            _additionalCameraData.cameraStack.Add(_uiCamera);
            _additionalCameraData.cameraStack.Reverse();
            _cinematographer.AddCamera(GameCamera.Run, await assetLoader
                .LoadAndInstantiateAsync<CinemachineVirtualCamera>("Run Virtual Camera", _camerasContainer));
            _cinematographer.AddCamera(GameCamera.Lose, await assetLoader
                .LoadAndInstantiateAsync<CinemachineVirtualCamera>("Lose Virtual Camera", _camerasContainer));

            SpawnData characterCarSpawnData = new(_entityContainer, Vector3.zero + Vector3.up * 0.026f, Quaternion.identity);
            _characterCar = new CharacterCarFactory(await assetLoader.Load<GameObject>("Character Car")).Create(characterCarSpawnData);
            _characterCar.gameObject.SetActive(false);
            ChunkGenerationConfig chunkGenerationConfig = await assetLoader.Load<ChunkGenerationConfig>("Chunk Generation Config");
            CheckPointChunk checkPoint = await assetLoader.LoadAndInstantiateAsync<CheckPointChunk>("Check Point Chunk", _chunksContainer);
            ChunkGenerationSystem chunkGenerationSystem = new(assetLoader, chunkGenerationConfig, _chunksContainer, checkPoint, _gameState);
            DrivingSystem drivingSystem = new(assetLoader, _playerInput, _characterCar,
                _player, _gameState, _coroutiner, _cinematographer, new(-5.5f, 5.5f));

            CharacterCarDamageSystem damageSystem = new(assetLoader, _gameState, _characterCar, _coroutiner, drivingSystem, _money, _leveMusic);
            CollectablesConfig collectablesConfig = projectContainer.Get<CollectablesConfig>();
            UICounter uiMoneyCounter = await assetLoader.LoadAndInstantiateAsync<UICounter>("UI Money Counter", _hudContainer);
            ProgressBar progressBar = await assetLoader.LoadAndInstantiateAsync<ProgressBar>("Progress Bar", _hudContainer);
            CheckPointPopup checkPointPopup = await assetLoader.LoadAndInstantiateAsync<CheckPointPopup>("Check Point Popup", _popupContainer);
            Button playButton = await assetLoader.LoadAndInstantiateAsync<Button>("Play Button", checkPointPopup.BalanceAndPlayButtonSection);
            UIEquipment equipment = await assetLoader.LoadAndInstantiateAsync<UIEquipment>("Equipment", checkPointPopup.OtherSection);
            UIInventory uiInventory = await assetLoader.LoadAndInstantiateAsync<UIInventory>("Merge", checkPointPopup.OtherSection);
            uiInventory.Construct(_canvas, equipment);
            UICounter lootBoxIcon = await assetLoader.LoadAndInstantiateAsync<UICounter>("Loot Box Icon", checkPointPopup.OtherSection);
            PriceTagButton lootBoxBuyButton = await assetLoader
                .LoadAndInstantiateAsync<PriceTagButton>("Buy Loot Box Button", checkPointPopup.OtherSection);
            _inventory = new Inventory(uiInventory.SlotsCount, equipment.SlotsCount, _player.Equipment, _player.Items);
            RestartSystem restartSystem = new(_gameState, assetLoader, _popupContainer, this, _leveMusic, _inventory, _player, _money);
            CheckPointSystem checkPointSystem = new(_gameState, _hudContainer, checkPoint,
                checkPointPopup, uiMoneyCounter, playButton, _money, _leveMusic, _player);
            CollectingSystem levelMoneyCollectingSystem = new(collectablesConfig, _money, _itemDatabase, _inventory, _characterCar, uiMoneyCounter);
            InventorySystem inventorySystem = new(_inventory, _itemDatabase, uiInventory, checkPointPopup, _player);
            ShopSystem shopSystem = new(_inventory, _itemDatabase, lootBoxBuyButton, _money, collectablesConfig, _player);
            WorldCentringSystem worldCentringSystem = new(_characterCar.transform, checkPoint, _entityContainer,
                _chunksContainer, _camerasContainer);
            SoundSystem soundSystem = new(assetLoader, _hudContainer, playerCamera.GetComponent<AudioListener>());
            ProgressSystem progressSystem = new(progressBar, checkPoint, _characterCar.transform, _player);
            _tutorialSystem = new(_gameState, _inventory, _money, _player);
            StearState stearState = new(_tutorialSystem, drivingSystem, _playerInput, 
                await assetLoader.LoadAndInstantiateAsync<IUIElement>("Tutorial Stear Popup", _popupContainer, isActive: false), _coroutiner, checkPoint);
            GasRegulationState gasRegulationState = new(_tutorialSystem, drivingSystem,
                await assetLoader.LoadAndInstantiateAsync<IUIElement>("Tutorial Gas Reguation Popup", _popupContainer, isActive: false), checkPoint);
            MergeState mergeState = new(_tutorialSystem, _money, collectablesConfig, inventorySystem, shopSystem, lootBoxBuyButton,
                await assetLoader.LoadAndInstantiateAsync<TutorialHighlighter>("Tutorial Highlighter", _tutorialContainer, isActive: false), uiInventory,
                await assetLoader.LoadAndInstantiateAsync<Image>("Finger", _tutorialContainer, isActive: false), playButton, _player, this);
            _tutorialSystem.AddStates(stearState, gasRegulationState, mergeState);

            _systems = new()
            {
                _tutorialSystem,
                chunkGenerationSystem,
                worldCentringSystem,
                drivingSystem,
                damageSystem,
                restartSystem,
                levelMoneyCollectingSystem,
                checkPointSystem,
                inventorySystem,
                shopSystem,
                soundSystem,
                progressSystem,
            };
        }

        protected override void OnInitializationCompleted()
        {
            _characterCar.gameObject.SetActive(true);
            _cinematographer.SwitchCamera(GameCamera.Run, isReset: true, _characterCar.transform, _characterCar.transform);
            _leveMusic.Play();
            _loadingScreen.FadeOut(OnFadeOut);
        }

        private void OnFadeOut()
        {
            if (_player.IsTutorialCompleted == false)
                _tutorialSystem.Start();

            _playerInput.Enable();
            _gameState.Switch(GameStates.Run);
        }

        [Button("Get 100 coins")]
        public void GetMoney()
        {
            _money.Add(100);
        }

        [Button("Get All Stuff")]
        public void GetAllStuff()
        {
            _inventory.Add(_itemDatabase.GetByIDs("it_Mhl_8", "it_Bks_8", "it_Egn_8", "it_Aclr_8"));
        }

        protected override void OnForceRestart()
        {
            _loadingScreen.FadeIn(OnFadeIn);
        }

        private void OnFadeIn()
        {
            _leveMusic.Pause();
            _coroutiner.StopAllCoroutines();
            DOTween.KillAll();
            _additionalCameraData.cameraStack.Remove(_uiCamera);
            _cinematographer.RemoveCamera(GameCamera.Run);
            _cinematographer.RemoveCamera(GameCamera.Lose);
            _sceneChanger.Change("Level");
        }
    }
}
