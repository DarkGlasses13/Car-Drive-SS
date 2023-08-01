using Assets._Project.Architecture;
using Assets._Project.Architecture.DI;
using Assets._Project.Architecture.UI;
using Assets._Project.CameraControl;
using Assets._Project.Entities.Character;
using Assets._Project.GameStateControl;
using Assets._Project.Helpers;
using Assets._Project.Input;
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
using Assets._Project.Systems.WorldCentring;
using Cinemachine;
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
            _loadingScreenContainer;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private AudioSource _leveMusic;
        private IPlayerInput _playerInput;
        private Cinematographer _cinematographer;
        private CharacterCar _characterCar;
        private Player _player;
        private GameState _gameState;
        private LoadingScreen _loadingScreen;

        protected override async Task CreateSystems()
        {
            DIContainer projectContainer = FindObjectOfType<DIContainer>();
            LocalAssetLoader assetLoader = projectContainer.Get<LocalAssetLoader>();
            _loadingScreen = projectContainer.Get<LoadingScreen>();
            _player = projectContainer.Get<Player>();
            _gameState = new(GameStates.WaitForRun);
            Coroutiner coroutiner = projectContainer.Get<Coroutiner>();
            Money money = projectContainer.Get<Money>();
            IItemDatabase itemDatabase = projectContainer.Get<IItemDatabase>();
            _playerInput = projectContainer.Get<IPlayerInput>();
            _cinematographer = projectContainer.Get<Cinematographer>();
            Camera playerCamera = projectContainer.Get<Camera>();
            UniversalAdditionalCameraData additionalCameraData = playerCamera.GetComponent<UniversalAdditionalCameraData>();
            Camera uiCamera = await assetLoader.LoadAndInstantiateAsync<Camera>("UI Camera", _camerasContainer);
            _canvas.worldCamera = uiCamera;
            additionalCameraData.cameraStack.Add(uiCamera);
            additionalCameraData.cameraStack.Reverse();
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
            DrivingSystem drivingSystem = new(assetLoader, _playerInput, _characterCar, _player, _gameState, coroutiner, _cinematographer);
            CharacterCarDamageSystem damageSystem = new(assetLoader, _gameState, _characterCar, coroutiner);
            CollectablesConfig collectablesConfig = projectContainer.Get<CollectablesConfig>();
            UICounter uiMoneyCounter = await assetLoader.LoadAndInstantiateAsync<UICounter>("UI Money Counter", _hudContainer);
            Slider progressBar = await assetLoader.LoadAndInstantiateAsync<Slider>("Progress Bar", _hudContainer);
            CheckPointPopup checkPointPopup = await assetLoader.LoadAndInstantiateAsync<CheckPointPopup>("Check Point Popup", _popupContainer);
            Button playButton = await assetLoader.LoadAndInstantiateAsync<Button>("Play Button", checkPointPopup.BalanceAndPlayButtonSection);
            UIEquipment equipment = await assetLoader.LoadAndInstantiateAsync<UIEquipment>("Equipment", checkPointPopup.OtherSection);
            UIInventory uiInventory = await assetLoader.LoadAndInstantiateAsync<UIInventory>("Merge", checkPointPopup.OtherSection);
            uiInventory.Construct(_canvas, equipment);
            UICounter lootBoxPrice = await assetLoader.LoadAndInstantiateAsync<UICounter>("Loot Box Price", checkPointPopup.OtherSection);
            PriceTagButton buyButton = await assetLoader
                .LoadAndInstantiateAsync<PriceTagButton>("Buy Loot Box Button", checkPointPopup.OtherSection);
            IInventory inventory = new Inventory(uiInventory.SlotsCount, equipment.SlotsCount);
            RestartSystem restartSystem = new(_gameState, assetLoader, _popupContainer, this, _leveMusic, inventory, _player, money);
            CheckPointSystem checkPointSystem = new(_gameState, _hudContainer, checkPoint,
                checkPointPopup, uiMoneyCounter, playButton, money, _leveMusic);
            CollectingSystem levelMoneyCollectingSystem = new(collectablesConfig, money, itemDatabase, inventory, _characterCar, uiMoneyCounter);
            InventorySystem inventorySystem = new(inventory, itemDatabase, uiInventory, checkPointPopup, _player);
            ShopSystem shopSystem = new(inventory, itemDatabase, buyButton, money, collectablesConfig, lootBoxPrice);
            WorldCentringSystem worldCentringSystem = new(_characterCar.transform, checkPoint, _entityContainer,
                _chunksContainer, _camerasContainer);
            SoundSystem soundSystem = new(assetLoader, _hudContainer, playerCamera.GetComponent<AudioListener>());
            ProgressSystem progressSystem = new(progressBar, checkPoint, _characterCar.transform);

            _systems = new()
            {
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
            _playerInput.Enable();
            _gameState.Switch(GameStates.Run);
        }
    }
}
