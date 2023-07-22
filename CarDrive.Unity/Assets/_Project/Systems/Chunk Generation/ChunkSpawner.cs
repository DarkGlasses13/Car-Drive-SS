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
        private List<Chunk> _prefabs;
        private ObjectPool<Chunk> _pool;
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
            _emptyLabel = _config.ChunkAssetLabel;
            IList<GameObject> emptyPrefabs = await _assetLoader.LoadAll<GameObject>(_emptyLabel, OnEmptyPrefabLoaded);
            _prefabs = new(emptyPrefabs.Select(chunk => chunk.GetComponent<Chunk>()));
            _pool = new(Create, null, null, null, true, defaultCapacity: 0, maxSize: 100);
            _initial = await _assetLoader.LoadAndInstantiateAsync<Chunk>("Initial Chunk", _container);
            _checkPoint = await _assetLoader.LoadAndInstantiateAsync<CheckPointChunk>("Check Point Chunk", _container);
            _checkPoint.gameObject.SetActive(false);
            _checkPoint.OnEnter += CheckPointEnter;
            _last = _initial;
        }

        public Chunk Spawn(bool withMoney = false, bool withObstacles = false)
        {
            Chunk instance = _pool.Get();
            PassedChunksCount++;
            instance.transform.position = _last.GetConnectPosition(instance);
            _last = instance;

            if (withMoney)
                instance.ShowMoney();

            if (withObstacles)
                instance.ShowObstacles();

            return instance;
        }

        public CheckPointChunk SpawnCheckPoint()
        {
            _checkPoint.transform.position = _last.GetConnectPosition(_checkPoint);
            _checkPoint.gameObject.SetActive(true);
            _last = _checkPoint;
            return _checkPoint;
        }

        private void CheckPointEnter(CheckPointChunk checkPoint) => OnCheckPointEnter?.Invoke(checkPoint);

        private void ChunkPassed(Chunk chunk) => OnChunkPassed?.Invoke(chunk);

        private Chunk Create() => Create(_prefabs);

        private T Create<T>(List<T> prefabs) where T : Chunk
        {
            T prefab = prefabs.ElementAt(Random.Range(0, prefabs.Count()));
            T instance = Object.Instantiate(prefab, _container);
            instance.OnPassed += ChunkPassed;
            return instance;
        }

        private void OnObstacleblePrefabLoaded(GameObject prefab) { }

        private void OnEmptyPrefabLoaded(GameObject prefab) { }
    }
}