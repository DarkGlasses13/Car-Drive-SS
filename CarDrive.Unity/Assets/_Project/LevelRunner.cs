using Assets._Project.CameraControl;
using Assets._Project.DI;
using Assets._Project.Entities;
using Assets._Project.Entities.Character;
using Assets._Project.Helpers;
using Assets._Project.Input;
using Assets._Project.Systems.ChunkGeneration;
using Assets._Project.Systems.Driving;
using Assets._Project.Systems.WorldCentring;
using Cinemachine;
using UnityEngine;

namespace Assets._Project
{
    public class LevelRunner : Runner
    {
        [SerializeField] private Transform 
            _camerasContainer,
            _chunksContainer,
            _entityContainer;

        protected async void Start()
        {
            DIContainer projectContainer = FindObjectOfType<DIContainer>();
            LocalAssetLoader assetLoader = projectContainer.Get<LocalAssetLoader>();
            IEntityContainer entityContainer = new EntityContainer();
            Cinematographer cinematographer = projectContainer.Get<Cinematographer>();
            await assetLoader.LoadAndInstantiateAsync<Camera>("Player Camera", _camerasContainer);
            cinematographer.AddCamera(GameCamera.Run, await assetLoader
                .LoadAndInstantiateAsync<CinemachineVirtualCamera>("Run Virtual Camera", _camerasContainer));
            cinematographer.AddCamera(GameCamera.Lose, await assetLoader
                .LoadAndInstantiateAsync<CinemachineVirtualCamera>("Lose Virtual Camera", _camerasContainer));

            ChunkGenerationData chunkGenerationData = new();
            ChunkGenerationSystem chunkGenerationSystem = new(chunkGenerationData, assetLoader, _chunksContainer);

            WorldCentringSystem worldCentringSystem = new(await assetLoader.Load<WorldCentringConfig>("World Centring Config"), chunkGenerationData);

            SpawnData characterCarSpawnData = new(_entityContainer, Vector3.zero, Quaternion.identity);
            CharacterCar characterCar = new CharacterCarFactory(await assetLoader
                .Load<GameObject>("Character Car")).Create(characterCarSpawnData);
            DrivingSystem drivingSystem = new(await assetLoader
                .Load<DrivingConfig>("Driving Config"), projectContainer.Get<IPlayerInput>(), characterCar);

            entityContainer.Add(characterCar);

            await chunkGenerationSystem.InitializeAsync();

            _systems = new() 
            {
                chunkGenerationSystem,
                worldCentringSystem,
                drivingSystem
            };

            cinematographer.SwitchCamera(GameCamera.Run, isReset: true, characterCar.transform, characterCar.transform);
        }
    }
}
