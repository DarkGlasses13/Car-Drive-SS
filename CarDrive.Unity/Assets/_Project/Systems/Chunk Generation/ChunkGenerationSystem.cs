using Assets._Project.Architecture;
using Assets._Project.GameStateControl;
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
        private readonly ChunkGenerationConfig _config;
        private readonly Transform _container;
        private readonly LocalAssetLoader _assetLoader;
        private AssetLabelReference _assetLabel;
        private List<Chunk> _prefabs;
        private int _prefabIndex;
        private Pool<Chunk> _pool;
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
            _assetLabel = _config.ChunkAssetLabel;
            IList<GameObject> emptyPrefabs = await _assetLoader.LoadAll<GameObject>(_assetLabel, OnPrefabLoaded);
            _prefabs = new(emptyPrefabs.Select(chunk => chunk.GetComponent<Chunk>()));
            _pool = new(Create, _container, 100);
        }

        public override void Enable()
        {
            SpawnLocation();
        }

        private void SpawnLocation()
        {
            _currentChunks.Add(SpawnInitial());

            for (int i = 1; i < _config.ChunksBetweenCheckPoints; i++)
            {
                _currentChunks.Add(SpawnRandom());
            }

            SpawnCheckPoint().OnPassed += OnPassed;

            for (int i = 0; i < _config.ChunksBetweenCheckPoints; i++)
            {
                _nextChunks.Add(SpawnRandom());
            }
        }

        private Chunk SpawnInitial() => Spawn(whithCollectables: false, withObstacles: false);

        private Chunk SpawnRandom()
        {
            bool withMoney = _config.IsMoneyEnabled && _config.MoneyDensity >= Random.value;
            bool withObstacles = _config.IsObstaclesEnabled && _config.GeneralObstacleDensity >= Random.value;
            Chunk instance = Spawn(withMoney, withObstacles);
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
                    for (int i = 0; i < _config.ChunksBetweenCheckPoints; i++)
                    {
                        _nextChunks.Add(SpawnRandom());
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

        private Chunk Spawn(bool whithCollectables = false, bool withObstacles = false, bool isCheckpoint = false)
        {
            Chunk chunk;

            if (isCheckpoint)
            {
                chunk = _checkPoint;
            }
            else
            {
                chunk = _pool.Get();

                if (whithCollectables)
                    chunk.ShowCollectables();

                if (withObstacles)
                    chunk.ShowObstacles();

                chunk.OnPassed += OnPassed;
            }

            chunk.transform.position = _last != null ? _last.GetConnectPosition(chunk) : Vector3.zero;
            _last = chunk;
            chunk.gameObject.SetActive(true);
            return chunk;
        }

        private CheckPointChunk SpawnCheckPoint()
        {
            CheckPointChunk checkPoint = (CheckPointChunk)Spawn(whithCollectables: false, withObstacles: false, isCheckpoint: true);
            return checkPoint;
        }

        private void Despawn(Chunk chunk)
        {
            chunk.gameObject.SetActive(false);
        }

        private void DespawnAll()
        {
            _checkPoint.gameObject.SetActive(false);
            _pool.ReleaseAll();
        }

        private void OnRelease(Chunk chunk)
        {
            chunk.HideAll();
        }

        private Chunk Create()
        {
            Chunk instance = Object.Instantiate(_prefabs[_prefabIndex], _container);
            instance.Init();
            instance.HideAll();
            _prefabIndex++;

            if (_prefabIndex >= _prefabs.Count)
                _prefabIndex = 0;

            return instance;
        }

        private void OnPrefabLoaded(GameObject prefab) { }
    }
}
