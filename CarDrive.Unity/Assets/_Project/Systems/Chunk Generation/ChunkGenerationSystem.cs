using Assets._Project.Architecture;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace Assets._Project.Systems.ChunkGeneration
{
    public class ChunkGenerationSystem : GameSystem
    {
        private readonly ChunkGenerationConfig _config;
        private readonly List<Chunk> _prefabs;
        private readonly Transform _container;
        private ObjectPool<Chunk> _pool;
        private Chunk _last;
        //private int _passedCount;

        public ChunkGenerationSystem(ChunkGenerationConfig config, IEnumerable<Chunk> prefabs, Transform container)
        {
            _config = config;
            _prefabs = new List<Chunk>(prefabs);
            _container = container;
            _pool = new(OnCreate, null, null, OnDestroy, true, defaultCapacity: config.InitialAmount, maxSize: 100);
        }

        public override void Initialize()
        {
            _last = Object.Instantiate(_prefabs.SingleOrDefault(chunk => chunk.IsInitial), _container);

            for (int i = 1; i < _config.InitialAmount; i++)
            {
                Spawn();
            }
        }

        private void Spawn()
        {
            Chunk instance = _pool.Get();
            instance.transform.position = _last.GetConnectPosition(instance);
            _last = instance;
        }

        public override void Tick()
        {

        }

        private void OnPassed(Chunk chunk)
        {
            //_passedCount++;

            //if (_passedCount >= _config.ChunksPassedBeforeCentering)
            //{
            //    _passedCount = 0;
            //    // Center
            //}

            Spawn();
        }

        private Chunk OnCreate()
        {
            IEnumerable<Chunk> prefabs = _prefabs.Where(prefab => prefab.IsInitial == false);
            Chunk prefab = prefabs.ElementAt(Random.Range(0, prefabs.Count()));
            Chunk instance = Object.Instantiate(prefab, _container);
            instance.OnPassed += OnPassed;
            return instance;
        }

        private void OnDestroy(Chunk chunk)
        {
            chunk.OnPassed -= OnPassed;
        }
    }
}
