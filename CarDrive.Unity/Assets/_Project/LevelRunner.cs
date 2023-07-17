using Assets._Project.CameraControl;
using Assets._Project.DI;
using Assets._Project.Entities;
using Assets._Project.Helpers;
using Assets._Project.Input;
using Assets._Project.Systems.ChunkGeneration;
using Assets._Project.Systems.Driving;
using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets._Project
{
    public class LevelRunner : Runner
    {
        [SerializeField] private Transform 
            _camerasContainer,
            _chunksContainer,
            _entityContainer;

        [SerializeField] private AssetLabelReference _chunkAssetLable;

        protected async void Start()
        {
            DIContainer projectContainer = FindObjectOfType<DIContainer>();
            LocalAssetLoader assetLoader = projectContainer.Get<LocalAssetLoader>();
            Cinematographer cinematographer = projectContainer.Get<Cinematographer>();
            await assetLoader.LoadAndInstantiate<Camera>("Player Camera", _camerasContainer);
            cinematographer.AddCamera(GameCamera.Run, await assetLoader
                .LoadAndInstantiate<CinemachineVirtualCamera>("Run Virtual Camera", _camerasContainer));
            cinematographer.AddCamera(GameCamera.Lose, await assetLoader
                .LoadAndInstantiate<CinemachineVirtualCamera>("Lose Virtual Camera", _camerasContainer));

            IList<GameObject> chunks = await assetLoader
                .LoadAll<GameObject>(_chunkAssetLable, chunk => { });
            ChunkGenerationSystem chunkGenerationSystem = new(await assetLoader
                .Load<ChunkGenerationConfig>("Chunk Generation Config"), chunks
                .Select(chunk => chunk.GetComponent<Chunk>()), _chunksContainer);


            SpawnData characterCarSpawnData = new(_entityContainer, Vector3.zero, Quaternion.identity);
            CharacterCar characterCar = new CharacterCarFactory(await assetLoader
                .Load<GameObject>("Character Car")).Create(characterCarSpawnData);
            DrivingSystem drivingSystem = new(await assetLoader
                .Load<DrivingConfig>("Driving Config"), projectContainer.Get<IPlayerInput>(), characterCar);

            chunkGenerationSystem.Initialize();

            _systems = new() 
            {
                chunkGenerationSystem,
                drivingSystem
            };

            cinematographer.SwitchCamera(GameCamera.Run, isReset: true, characterCar.transform, characterCar.transform);
        }
    }
}
