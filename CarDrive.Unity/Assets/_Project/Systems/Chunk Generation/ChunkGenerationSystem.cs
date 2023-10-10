using Assets._Project.Architecture;
using Assets._Project.GameStateControl;
using Assets._Project.Helpers;
using Assets._Project.Systems.Chunk_Generation;
using System;
using System.Collections;
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
        private readonly ChunksLoader _chunksLoader;
        private readonly Coroutiner _coroutiner;
        private List<Chunk> _prefabs;
        private List<Chunk> _pool = new();
        private Chunk _last;
        private CheckPointChunk _checkPoint;
        private readonly GameState _gameState;
        private bool _isCheckPointPassed;
        private int _passedChunksCount;
        private List<Chunk> _currentChunks = new(), _nextChunks = new();

        public ChunkGenerationSystem(ChunksLoader chunksLoader, Coroutiner coroutiner, ChunkGenerationConfig config,
            Transform container, CheckPointChunk checkPoint, GameState gameState)
        {
            _chunksLoader = chunksLoader;
            _coroutiner = coroutiner;
            _config = config;
            _container = container;
            _checkPoint = checkPoint;
            _gameState = gameState;
            _checkPoint.gameObject.SetActive(false);
        }

        public override async Task InitializeAsync()
        {
            _prefabs = new(await _chunksLoader.LoadAsync());
        }

        public override void OnEnable()
        {
            _coroutiner.StartCoroutine(SpawnLocationRoutine());
        }

        private IEnumerator SpawnLocationRoutine()
        {
            ChunkEnvironmentType currentType = GetRandomType();
            _currentChunks.Add(SpawnInitial(currentType));
            int count = 0;

            for (int i = 1; i < _config.ChunksBetweenCheckPoints; i++)
            {
                if (count > 3)
                {
                    count = 0;
                    yield return null;
                }

                _currentChunks.Add(SpawnRandom(currentType));
                count++;
            }

            SpawnCheckPoint(currentType).OnPassed += OnPassed;
            ChunkEnvironmentType nextType = GetRandomType();
            _nextChunks.Add(SpawnInitial(nextType));
            count = 0;

            for (int i = 0; i < _config.ChunksBetweenCheckPoints; i++)
            {
                if (count > 3)
                {
                    count = 0;
                    yield return null;
                }

                _nextChunks.Add(SpawnRandom(nextType));
                count++;
            }
        }

        private void OnPassed(Chunk chunk) => _coroutiner.StartCoroutine(OnPassedRoutine(chunk));

        private Chunk SpawnInitial(ChunkEnvironmentType environmentType) => Spawn(environmentType, whithCollectables: false, withObstacles: false);

        private Chunk SpawnRandom(ChunkEnvironmentType environmentType)
        {
            bool withMoney = _config.IsMoneyEnabled && _config.MoneyDensity >= Random.value;
            bool withObstacles = _config.IsObstaclesEnabled && _config.GeneralObstacleDensity >= Random.value;
            Chunk instance = Spawn(environmentType, withMoney, withObstacles);
            return instance;
        }

        private IEnumerator OnPassedRoutine(Chunk chunk)
        {
            if (_gameState.Current == GameStates.Finish)
                yield break;

            if (chunk is CheckPointChunk)
            {
                _isCheckPointPassed = true;
            }

            if (_isCheckPointPassed)
            {
                _passedChunksCount++;

                if (_passedChunksCount >= _config.ChunksPassedBeforeDespawn && _isCheckPointPassed)
                {
                    ChunkEnvironmentType nextType = GetRandomType();
                    SpawnCheckPoint(nextType);
                    _passedChunksCount = 0;
                    _isCheckPointPassed = false;
                    Despawn(_currentChunks);
                    _currentChunks.Clear();
                    _currentChunks.AddRange(_nextChunks);
                    _nextChunks.Clear();
                    _nextChunks.Add(SpawnInitial(nextType));

                    for (int i = 0; i < _config.ChunksBetweenCheckPoints; i++)
                    {
                        _nextChunks.Add(SpawnRandom(nextType));
                        yield return null;
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

        private CheckPointChunk SpawnCheckPoint(ChunkEnvironmentType type)
        {
            CheckPointChunk checkPoint = (CheckPointChunk)Spawn(
                ChunkEnvironmentType.None, 
                whithCollectables: false, 
                withObstacles: false, 
                isCheckpoint: true);

            checkPoint.SetEnvironment(type);
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
