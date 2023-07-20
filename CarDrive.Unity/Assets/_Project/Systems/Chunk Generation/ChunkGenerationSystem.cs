using Assets._Project.Architecture;
using Assets._Project.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;

namespace Assets._Project.Systems.ChunkGeneration
{
    public class ChunkGenerationSystem : GameSystem
    {
        private readonly ChunkGenerationData _data;
        private readonly LocalAssetLoader _assetLoader;
        private readonly Transform _container;
        private ChunkGenerationConfig _config;
        private AssetLabelReference _emptyLabel, _obstaclebleLabel;
        private List<Chunk> _emptyPrefabs;
        private List<ObstaclebleChunk> _obstacleblePrefabs;
        private ObjectPool<Chunk> _emptyPool;
        private ObjectPool<ObstaclebleChunk> _obstacleblePool;
        private Chunk _initial, _last;
        private CheckPointChunk _checkPoint;

        public ChunkGenerationSystem(ChunkGenerationData data, LocalAssetLoader assetLoader, Transform container)
        {
            _data = data;
            _assetLoader = assetLoader;
            _container = container;
        }

        public override async Task InitializeAsync()
        {
            _config = await _assetLoader.Load<ChunkGenerationConfig>("Chunk Generation Config");
            _emptyLabel = _config.EmptyChunkAssetLabel;
            _obstaclebleLabel = _config.ObstaclebleChunkAssetLabel;
            IList<GameObject> emptyPrefabs = await _assetLoader.LoadAll<GameObject>(_emptyLabel, OnEmptyPrefabLoaded);
            _emptyPrefabs = new(emptyPrefabs.Select(chunk => chunk.GetComponent<Chunk>()));
            _emptyPool = new(CreateEmpty, null, null, OnDestroy, true, defaultCapacity: 0, maxSize: 100);
            IList<GameObject> obstacleblePrefabs = await _assetLoader.LoadAll<GameObject>(_obstaclebleLabel, OnObstacleblePrefabLoaded);
            _obstacleblePrefabs = new(obstacleblePrefabs.Select(chunk => chunk.GetComponent<ObstaclebleChunk>()));
            _obstacleblePool = new(CreateObstacleble, null, null, OnObstaclebleDestroy, true, defaultCapacity: 0, maxSize: 100);
            _initial = await _assetLoader.LoadAndInstantiateAsync<Chunk>("Initial Chunk", _container);
            _checkPoint = await _assetLoader.LoadAndInstantiateAsync<CheckPointChunk>("Check Point Chunk", _container);
            _checkPoint.gameObject.SetActive(false);
            _last = _initial;

            for (int i = 1; i < _config.InitialAmount; i++)
            {
                Spawn();
            }
        }

        private void Spawn()
        {
            Chunk instance = _config.IsObstaclesEnabled && _config.GeneralObstacleDencity >= Random.value 
                ? _obstacleblePool.Get()
                : _emptyPool.Get();

            instance.transform.position = _last.GetConnectPosition(instance);
            _last = instance;
        }

        public override void Tick()
        {
            
        }

        private void OnPassed(Chunk chunk)
        {
            _data.PassedChunksCount++;
            Spawn();
        }

        private ObstaclebleChunk CreateObstacleble() => Create(_obstacleblePrefabs);

        private void OnObstaclebleDestroy(ObstaclebleChunk chunk) => OnDestroy(chunk);

        private Chunk CreateEmpty() => Create(_emptyPrefabs);

        private T Create<T>(List<T> prefabs) where T : Chunk
        {
            T prefab = prefabs.ElementAt(Random.Range(0, prefabs.Count()));
            T instance = Object.Instantiate(prefab, _container);
            instance.OnPassed += OnPassed;
            return instance;
        }

        private void OnDestroy(Chunk chunk)
        {
            chunk.OnPassed -= OnPassed;
        }

        private void OnObstacleblePrefabLoaded(GameObject prefab) { }

        private void OnEmptyPrefabLoaded(GameObject prefab) { }
    }
}
