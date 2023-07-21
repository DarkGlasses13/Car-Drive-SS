using Assets._Project.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets._Project.Systems.ChunkGeneration
{
    public class ChunkSpawner
    {
        public event Action<Chunk> OnChunkPassed;
        public event Action<CheckPointChunk> OnCheckPointEnter;

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

        public int PassedChunksCount { get; private set; }

        public ChunkSpawner(LocalAssetLoader assetLoader, ChunkGenerationConfig config, Transform container)
        {
            _config = config;
            _assetLoader = assetLoader;
            _container = container;
        }

        public async Task InitializeAsync()
        {
            _emptyLabel = _config.EmptyChunkAssetLabel;
            _obstaclebleLabel = _config.ObstaclebleChunkAssetLabel;
            IList<GameObject> emptyPrefabs = await _assetLoader.LoadAll<GameObject>(_emptyLabel, OnEmptyPrefabLoaded);
            _emptyPrefabs = new(emptyPrefabs.Select(chunk => chunk.GetComponent<Chunk>()));
            _emptyPool = new(CreateEmpty, null, null, null, true, defaultCapacity: 0, maxSize: 100);
            IList<GameObject> obstacleblePrefabs = await _assetLoader.LoadAll<GameObject>(_obstaclebleLabel, OnObstacleblePrefabLoaded);
            _obstacleblePrefabs = new(obstacleblePrefabs.Select(chunk => chunk.GetComponent<ObstaclebleChunk>()));
            _obstacleblePool = new(CreateObstacleble, null, null, null, true, defaultCapacity: 0, maxSize: 100);
            _initial = await _assetLoader.LoadAndInstantiateAsync<Chunk>("Initial Chunk", _container);
            _checkPoint = await _assetLoader.LoadAndInstantiateAsync<CheckPointChunk>("Check Point Chunk", _container);
            _checkPoint.gameObject.SetActive(false);
            _checkPoint.OnEnter += CheckPointEnter;
            _last = _initial;
        }

        public Chunk SpawnEmpty() => Spawn(_emptyPool);

        public ObstaclebleChunk SpawnObstacleable() => Spawn(_obstacleblePool);

        public CheckPointChunk SpawnCheckPoint()
        {
            _checkPoint.transform.position = _last.GetConnectPosition(_checkPoint);
            _checkPoint.gameObject.SetActive(true);
            _last = _checkPoint;
            return _checkPoint;
        }

        private void CheckPointEnter(CheckPointChunk checkPoint) => OnCheckPointEnter?.Invoke(checkPoint);

        private void ChunkPassed(Chunk chunk) => OnChunkPassed?.Invoke(chunk);

        private Chunk CreateEmpty() => Create(_emptyPrefabs);

        private ObstaclebleChunk CreateObstacleble() => Create(_obstacleblePrefabs);

        private T Create<T>(List<T> prefabs) where T : Chunk
        {
            T prefab = prefabs.ElementAt(Random.Range(0, prefabs.Count()));
            T instance = Object.Instantiate(prefab, _container);
            instance.OnPassed += ChunkPassed;
            return instance;
        }

        private void OnObstacleblePrefabLoaded(GameObject prefab) { }

        private void OnEmptyPrefabLoaded(GameObject prefab) { }

        private T Spawn<T>(IObjectPool<T> pool) where T : Chunk
        {
            T instance = pool.Get();
            PassedChunksCount++;
            instance.transform.position = _last.GetConnectPosition(instance);
            _last = instance;
            return instance;
        }
    }
}