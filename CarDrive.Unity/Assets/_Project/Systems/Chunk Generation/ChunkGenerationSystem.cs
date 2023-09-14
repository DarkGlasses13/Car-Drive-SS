using Assets._Project.Architecture;
using Assets._Project.GameStateControl;
using Assets._Project.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets._Project.Systems.ChunkGeneration
{
    public class ChunkGenerationSystem : GameSystem
    {
        private readonly ChunkGenerationConfig _config;
        private readonly Transform _container;
        private readonly LocalAssetLoader _assetLoader;
        private List<Chunk> _prefabs;
        private List<Chunk> _pool = new();
        private Chunk _last;
        private CheckPointChunk _checkPoint;
        private readonly GameState _gameState;
        private bool _isCheckPointPassed;
        private int _passedChunksCount;
        private List<Chunk> _currentChunks = new(), _nextChunks = new();

        public ChunkGenerationSystem(LocalAssetLoader assetLoader, ChunkGenerationConfig config,
            Transform container, CheckPointChunk checkPoint, GameState gameState)
        {
            _assetLoader = assetLoader;
            _config = config;
            _container = container;
            _checkPoint = checkPoint;
            _gameState = gameState;
            _checkPoint.gameObject.SetActive(false);
        }

        public override async Task InitializeAsync()
        {
            IList<GameObject> emptyPrefabs = await _assetLoader.LoadAll<GameObject>("In Game Chunk", OnPrefabLoaded);
            _prefabs = new(emptyPrefabs.Select(chunk => chunk.GetComponent<Chunk>()));
        }

        public override void OnEnable()
        {
            SpawnLocation();
        }

        private void SpawnLocation()
        {
            ChunkEnvironmentType currentType = GetRandomType();
            _currentChunks.Add(SpawnInitial(currentType));

            for (int i = 1; i < _config.ChunksBetweenCheckPoints; i++)
            {
                _currentChunks.Add(SpawnRandom(currentType));
            }

            SpawnCheckPoint().OnPassed += OnPassed;
            ChunkEnvironmentType nextType = GetRandomType();
            _nextChunks.Add(SpawnInitial(nextType));

            for (int i = 0; i < _config.ChunksBetweenCheckPoints; i++)
            {
                _nextChunks.Add(SpawnRandom(nextType));
            }
        }

        private Chunk SpawnInitial(ChunkEnvironmentType environmentType) => Spawn(environmentType, whithCollectables: false, withObstacles: false);

        private Chunk SpawnRandom(ChunkEnvironmentType environmentType)
        {
            bool withMoney = _config.IsMoneyEnabled && _config.MoneyDensity >= Random.value;
            bool withObstacles = _config.IsObstaclesEnabled && _config.GeneralObstacleDensity >= Random.value;
            Chunk instance = Spawn(environmentType, withMoney, withObstacles);
            return instance;
        }

        private void OnPassed(Chunk chunk)
        {
            if (_gameState.Current == GameStates.Finish)
                return;

            if (chunk is CheckPointChunk)
            {
                _isCheckPointPassed = true;
            }

            if (_isCheckPointPassed)
            {
                _passedChunksCount++;

                if (_passedChunksCount >= _config.ChunksPassedBeforeDespawn && _isCheckPointPassed)
                {
                    SpawnCheckPoint();
                    _passedChunksCount = 0;
                    _isCheckPointPassed = false;
                    Despawn(_currentChunks);
                    _currentChunks.Clear();
                    _currentChunks.AddRange(_nextChunks);
                    _nextChunks.Clear();
                    ChunkEnvironmentType nextType = GetRandomType();
                    _nextChunks.Add(SpawnInitial(nextType));

                    for (int i = 0; i < _config.ChunksBetweenCheckPoints; i++)
                    {
                        _nextChunks.Add(SpawnRandom(nextType));
                    }
                }
            }
        }

        private void Despawn(IEnumerable<Chunk> chunks)
        {
            foreach (Chunk chunk in chunks)
            {
                Despawn(chunk);
            }
        }

        private Chunk Spawn(ChunkEnvironmentType type, bool whithCollectables = false, bool withObstacles = false, bool isCheckpoint = false)
        {
            Chunk chunk;

            if (isCheckpoint)
            {
                chunk = _checkPoint;
            }
            else
            {
                IEnumerable<Chunk> chunksByType = _pool.Where(chunk => chunk.gameObject.activeInHierarchy == false && chunk.EnvironmentType == type);

                chunk = chunksByType.Count() > 0 
                    ? chunksByType.ElementAt(Random.Range(0, chunksByType.Count())) 
                    : Create(type);

                if (whithCollectables)
                    chunk.ShowCollectables();

                if (withObstacles)
                    chunk.ShowObstacles();
            }

            chunk.transform.position = _last != null ? _last.GetConnectPosition(chunk) : Vector3.zero;
            chunk.InvokeOnSpawn();
            _last = chunk;
            chunk.gameObject.SetActive(true);
            return chunk;
        }

        private CheckPointChunk SpawnCheckPoint()
        {
            CheckPointChunk checkPoint = (CheckPointChunk)Spawn(ChunkEnvironmentType.None, whithCollectables: false, withObstacles: false, isCheckpoint: true);
            return checkPoint;
        }

        private void Despawn(Chunk chunk)
        {
            chunk.gameObject.SetActive(false);
            chunk.HideAll();
        }

        private void DespawnAll()
        {
            _checkPoint.gameObject.SetActive(false);
            _pool.ForEach(chunk => Despawn(chunk));
        }

        private Chunk Create(ChunkEnvironmentType type)
        {
            var prefabsByType = _prefabs.Where(prefab => prefab.EnvironmentType == type);
            Chunk instance = Object.Instantiate(prefabsByType.ElementAt(Random.Range(0, prefabsByType.Count())), _container);
            instance.Init();
            instance.HideAll();
            instance.OnPassed += OnPassed;
            return instance;
        }

        private void OnPrefabLoaded(GameObject prefab) { }

        private ChunkEnvironmentType GetRandomType()
        {
            return (ChunkEnvironmentType)Random
                .Range(1, (int)Enum
                .GetValues(typeof(ChunkEnvironmentType))
                .Cast<ChunkEnvironmentType>()
                .Max());
        }
    }
}
