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
        private readonly LocalAssetLoader _assetLoader;
        private readonly ChunksLoader _chunksLoader;
        private Chunk _last;
        private ChunksEvents _chunksEvents;
        private readonly GameState _gameState;
        private bool _isAllChunksLoaded;
        private int _sequenceIndex;
        private ChunkEnvironmentType[] _randomTypesSequence;
        private GameObject _checkPointPrefab;
        private int _activeChunksCount;
        private ChunkEnvironmentType _currentType;
        private readonly Queue<Chunk> _passedChunks = new();
        private readonly List<Chunk>
            _prefabs = new(),
            _pool = new();

        public ChunkGenerationSystem(LocalAssetLoader assetLoader, ChunksLoader chunksLoader, ChunkGenerationConfig config,
            Transform container, ChunksEvents checkPointEvent, GameState gameState)
        {
            _assetLoader = assetLoader;
            _chunksLoader = chunksLoader;
            _config = config;
            _container = container;
            _chunksEvents = checkPointEvent;
            _gameState = gameState;
        }

        public override async Task InitializeAsync()
        {
            System.Random random = new();
            _randomTypesSequence = Enum.GetValues(typeof(ChunkEnvironmentType)) as ChunkEnvironmentType[];
            _randomTypesSequence = _randomTypesSequence.OrderBy(element => random.Next()).ToArray();
            _randomTypesSequence = _randomTypesSequence.Where(element => element != ChunkEnvironmentType.None).ToArray();
            _checkPointPrefab = await _assetLoader.Load<GameObject>("Check Point Chunk");
            await LoadPrefabs();
            await SpawnInitialAsync();
        }

        private async Task LoadPrefabs()
        {
            IEnumerable<Chunk> initialPrefabs = await _chunksLoader.LoadAsync(_randomTypesSequence[_sequenceIndex]);
            _prefabs.AddRange(initialPrefabs);
            _sequenceIndex++;
            _isAllChunksLoaded = _sequenceIndex >= _randomTypesSequence.Length - 1;
        }

        private async void OnCheckPointEnter(CheckPointChunk chunk)
        {
            _chunksEvents.Enter(chunk);
            _chunksEvents.IsTriggered = true;

            if (_isAllChunksLoaded == false)
                await LoadPrefabs();
        }

        private async Task SpawnInitialAsync()
        {
            _currentType = GetRandomType();
            SpawnInitialChunk();
            _activeChunksCount++;

            for (int i = 0; i < _config.MaxChunks; i++)
            {
                SpawnTick();
                await Task.Yield();
            }
        }

        private void SpawnTick()
        {
            if (_activeChunksCount < _config.ChunksBetweenCheckPoints)
            {
                SpawnRandom(_currentType);
                _activeChunksCount++;
                return;
            }

            SpawnCheckPoint(_currentType);
            _activeChunksCount = 0;
            _currentType = GetRandomType();
        }

        private void OnPassed(Chunk chunk)
        {
            _passedChunks.Enqueue(chunk);

            if (chunk is CheckPointChunk checkPoint)
            {
                _chunksEvents.Pass(checkPoint);
                _chunksEvents.IsTriggered = false;
            }
            else
            {
                _chunksEvents.AnyPass(chunk);
            }

            if (_passedChunks.Count > _config.ChunksPassedBeforeDespawn)
                Despawn(_passedChunks.Dequeue());

            if (_gameState.Current == GameStates.Finish)
                return;

            SpawnTick();
        }

        private Chunk SpawnInitialChunk() => Spawn(_currentType, whithCollectables: false, withObstacles: false);

        private Chunk SpawnRandom(ChunkEnvironmentType environmentType)
        {
            bool withMoney = _config.IsMoneyEnabled && _config.MoneyDensity >= Random.value;
            bool withObstacles = _config.IsObstaclesEnabled && _config.GeneralObstacleDensity >= Random.value;
            Chunk instance = Spawn(environmentType, withMoney, withObstacles);
            return instance;
        }

        private Chunk Spawn(ChunkEnvironmentType type, bool whithCollectables = false, bool withObstacles = false, bool isCheckpoint = false)
        {
            Chunk chunk;

            if (isCheckpoint)
            {
                chunk = _pool.FirstOrDefault(chunk => chunk is CheckPointChunk && chunk.gameObject.activeInHierarchy == false);
                chunk = chunk == null ? CreateCheckPoint() : chunk;
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

        private CheckPointChunk CreateCheckPoint()
        {
            CheckPointChunk instance = Object.Instantiate(_checkPointPrefab, _container).GetComponent<CheckPointChunk>();
            instance.OnEnter += OnCheckPointEnter;
            instance.OnPassed += OnPassed;
            _pool.Add(instance);
            return instance;
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
            _pool.ForEach(chunk => Despawn(chunk));
            _passedChunks.Clear();
        }

        private Chunk Create(ChunkEnvironmentType type)
        {
            IEnumerable<Chunk> prefabsByType = _prefabs.Where(prefab => prefab.EnvironmentType == type);
            Chunk instance = Object.Instantiate(prefabsByType.ElementAt(Random.Range(0, prefabsByType.Count())), _container);
            instance.Init();
            instance.HideAll();
            instance.OnPassed += OnPassed;
            _pool.Add(instance);
            return instance;
        }

        private ChunkEnvironmentType GetRandomType()
        {
            IEnumerable<ChunkEnvironmentType> types = _prefabs.Select(chunk => chunk.EnvironmentType);
            return types.ElementAt(Random.Range(0, types.Count()));
        }
    }
}
