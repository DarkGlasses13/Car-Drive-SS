using Assets._Project.CameraControl;
using Assets._Project.DI;
using Assets._Project.Entities.Character;
using Assets._Project.GameStateControl;
using Assets._Project.Helpers;
using Assets._Project.Input;
using Assets._Project.Systems.ChunkGeneration;
using Assets._Project.Systems.Driving;
using Assets._Project.Systems.WorldCentring;
using Cinemachine;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project
{
    public class LevelRunner : Runner
    {
        [SerializeField] private Transform 
            _camerasContainer,
            _chunksContainer,
            _entityContainer;

        protected override async Task CreateSystems()
        {
            DIContainer projectContainer = FindObjectOfType<DIContainer>();
            LocalAssetLoader assetLoader = projectContainer.Get<LocalAssetLoader>();
            GameState gameState = new(GameStates.Run);
            IPlayerInput playerInput = projectContainer.Get<IPlayerInput>();
            Cinematographer cinematographer = projectContainer.Get<Cinematographer>();
            await assetLoader.LoadAndInstantiateAsync<Camera>("Player Camera", _camerasContainer);
            cinematographer.AddCamera(GameCamera.Run, await assetLoader
                .LoadAndInstantiateAsync<CinemachineVirtualCamera>("Run Virtual Camera", _camerasContainer));
            cinematographer.AddCamera(GameCamera.Lose, await assetLoader
                .LoadAndInstantiateAsync<CinemachineVirtualCamera>("Lose Virtual Camera", _camerasContainer));

            ChunkGenerationData chunkGenerationData = new();
            ChunkGenerationSystem chunkGenerationSystem = new(chunkGenerationData, assetLoader, _chunksContainer);
            await chunkGenerationSystem.InitializeAsync();

            WorldCentringSystem worldCentringSystem = new(await assetLoader
                .Load<WorldCentringConfig>("World Centring Config"), chunkGenerationData);
            await worldCentringSystem.InitializeAsync();

            SpawnData characterCarSpawnData = new(_entityContainer, Vector3.zero + Vector3.up * 0.5f, Quaternion.identity);
            CharacterCar characterCar = new CharacterCarFactory(await assetLoader
                .Load<GameObject>("Character Car")).Create(characterCarSpawnData);
            DrivingSystem drivingSystem = new(await assetLoader
                .Load<DrivingConfig>("Driving Config"), playerInput, characterCar, gameState);
            await drivingSystem.InitializeAsync();

            _systems = new()
            {
                chunkGenerationSystem,
                worldCentringSystem,
                drivingSystem
            };

            cinematographer.SwitchCamera(GameCamera.Run, isReset: true, characterCar.transform, characterCar.transform);
            playerInput.Enable();
            drivingSystem.Enable();
        }
    }
}
