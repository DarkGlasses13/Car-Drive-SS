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
        private GameState _gameState;

        protected override async Task CreateSystems()
        {
            DIContainer projectContainer = FindObjectOfType<DIContainer>();
            LocalAssetLoader assetLoader = projectContainer.Get<LocalAssetLoader>();
            Player player = projectContainer.Get<Player>();
            _gameState = new(GameStates.WaitForRun);
            Coroutiner coroutiner = projectContainer.Get<Coroutiner>();
            Money money = projectContainer.Get<Money>();
            IItemDatabase itemDatabase = projectContainer.Get<IItemDatabase>();
            _playerInput = projectContainer.Get<IPlayerInput>();
            _cinematographer = projectContainer.Get<Cinematographer>();
            Camera playerCamera = await assetLoader.LoadAndInstantiateAsync<Camera>("Player Camera", _camerasContainer);
            Camera uiCamera = await assetLoader.LoadAndInstantiateAsync<Camera>("UI Camera", _camerasContainer);
            _canvas.worldCamera = uiCamera;
            playerCamera.GetComponent<UniversalAdditionalCameraData>().cameraStack.Add(uiCamera);
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
            DrivingSystem drivingSystem = new(assetLoader, _playerInput, _characterCar, player, _gameState, coroutiner, _cinematographer);
            CharacterCarDamageSystem damageSystem = new(assetLoader, _gameState, _characterCar, coroutiner);
            CollectablesConfig collectablesConfig = projectContainer.Get<CollectablesConfig>();
            UICounter uiMoneyCounter = await assetLoader.LoadAndInstantiateAsync<UICounter>("UI Money Counter", _hudContainer);
            CheckPointPopup checkPointPopup = await assetLoader.LoadAndInstantiateAsync<CheckPointPopup>("Check Point Popup", _popupContainer);
            Button playButton = await assetLoader.LoadAndInstantiateAsync<Button>("Play Button", checkPointPopup.BalanceAndPlayButtonSection);
            UIEquipment equipment = await assetLoader.LoadAndInstantiateAsync<UIEquipment>("Equipment", checkPointPopup.EquipmentSection);
            UIInventory uiInventory = await assetLoader.LoadAndInstantiateAsync<UIInventory>("Merge", checkPointPopup.MergeAndBuyButtonSection);
            uiInventory.Construct(_canvas, equipment);
            PriceTagButton buyButton = await assetLoader
                .LoadAndInstantiateAsync<PriceTagButton>("Shop Buy Button", checkPointPopup.MergeAndBuyButtonSection);
            IInventory inventory = new Inventory(uiInventory.SlotsCount, equipment.SlotsCount);
            RestartSystem restartSystem = new(_gameState, assetLoader, _popupContainer, this, _leveMusic, inventory, player, money);
            CheckPointSystem checkPointSystem = new(_gameState, _hudContainer, checkPoint,
                checkPointPopup, uiMoneyCounter, playButton, money, _leveMusic);
            CollectingSystem levelMoneyCollectingSystem = new(collectablesConfig, money, itemDatabase, inventory, _characterCar, uiMoneyCounter);
            InventorySystem inventorySystem = new(inventory, itemDatabase, uiInventory, checkPointPopup, player);
            ShopSystem shopSystem = new(inventory, itemDatabase, buyButton, money, collectablesConfig);
            WorldCentringSystem worldCentringSystem = new(_characterCar.transform, checkPoint, _entityContainer,
                _chunksContainer, _camerasContainer);
            SoundSystem soundSystem = new(assetLoader, _hudContainer, playerCamera.GetComponent<AudioListener>());

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
            };
        }

        protected override void OnInitializationCompleted()
        {
            _characterCar.gameObject.SetActive(true);
            _cinematographer.SwitchCamera(GameCamera.Run, isReset: true, _characterCar.transform, _characterCar.transform);
            _playerInput.Enable();
            _leveMusic.Play();
            _gameState.Switch(GameStates.Run);
        }
    }
}
