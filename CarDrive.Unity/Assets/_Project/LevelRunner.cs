using Assets._Project.CameraControl;
using Assets._Project.DI;
using Assets._Project.Entities.Character;
using Assets._Project.GameStateControl;
using Assets._Project.Helpers;
using Assets._Project.Input;
using Assets._Project.Systems.ChunkGeneration;
using Assets._Project.Systems.Damage;
using Assets._Project.Systems.Driving;
using Assets._Project.Systems.WorldCentring;
using Cinemachine;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project
{
    public class LevelRunner : RunnerWithAutomaticSystemsInitialization
    {
        [SerializeField] private Transform 
            _camerasContainer,
            _chunksContainer,
            _entityContainer;
        private IPlayerInput _playerInput;
        private Cinematographer _cinematographer;
        private CharacterCar _characterCar;

        protected override async Task CreateSystems()
        {
            DIContainer projectContainer = FindObjectOfType<DIContainer>();
            LocalAssetLoader assetLoader = projectContainer.Get<LocalAssetLoader>();
            GameState gameState = new(GameStates.Run);
            Coroutiner coroutiner = projectContainer.Get<Coroutiner>();
            _playerInput = projectContainer.Get<IPlayerInput>();
            _cinematographer = projectContainer.Get<Cinematographer>();
            await assetLoader.LoadAndInstantiateAsync<Camera>("Player Camera", _camerasContainer);
            _cinematographer.AddCamera(GameCamera.Run, await assetLoader
                .LoadAndInstantiateAsync<CinemachineVirtualCamera>("Run Virtual Camera", _camerasContainer));
            _cinematographer.AddCamera(GameCamera.Lose, await assetLoader
                .LoadAndInstantiateAsync<CinemachineVirtualCamera>("Lose Virtual Camera", _camerasContainer));

            SpawnData characterCarSpawnData = new(_entityContainer, Vector3.zero + Vector3.up * 0.5f, Quaternion.identity);
            _characterCar = new CharacterCarFactory(await assetLoader.Load<GameObject>("Character Car")).Create(characterCarSpawnData);
            _characterCar.gameObject.SetActive(false);

            ChunkGenerationConfig chunkGenerationConfig = await assetLoader.Load<ChunkGenerationConfig>("Chunk Generation Config");
            ChunkSpawner chunkSpawner = new(assetLoader, chunkGenerationConfig, _chunksContainer);
            ChunkGenerationSystem chunkGenerationSystem = new(chunkGenerationConfig, chunkSpawner, gameState);
            WorldCentringSystem worldCentringSystem = new(await assetLoader.Load<WorldCentringConfig>("World Centring Config"), chunkSpawner);
            DrivingSystem drivingSystem = new(assetLoader, _playerInput, _characterCar, gameState, coroutiner);
            CharacterCarDamageSystem damageSystem = new(assetLoader, gameState, _characterCar);

            _systems = new()
            {
                chunkGenerationSystem,
                worldCentringSystem,
                drivingSystem,
                damageSystem
            };
        }

        protected override void OnInitializationCompleted()
        {
            _characterCar.gameObject.SetActive(true);
            _cinematographer.SwitchCamera(GameCamera.Run, isReset: true, _characterCar.transform, _characterCar.transform);
            _playerInput.Enable();
        }
    }
}
