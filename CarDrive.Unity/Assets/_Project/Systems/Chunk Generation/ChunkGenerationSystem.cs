using Assets._Project.Architecture;
using Assets._Project.GameStateControl;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Systems.ChunkGeneration
{
    public class ChunkGenerationSystem : GameSystem, IGameStateSwitchHandler
    {
        private readonly ChunkGenerationConfig _config;
        private readonly ChunkSpawner _spawner;
        private readonly GameState _gameState;

        public ChunkGenerationSystem(ChunkGenerationConfig config, ChunkSpawner spawner, GameState gameState)
        {
            _config = config;
            _spawner = spawner;
            _gameState = gameState;
        }

        public override async Task InitializeAsync()
        {
            await _spawner.InitializeAsync();
        }

        public override void Enable()
        {
            _gameState.OnSwitched += OnSateSwitched;
            _spawner.OnChunkPassed += OnPassed;
            SpawnInitial();
        }

        public void OnSateSwitched(GameStates state)
        {

        }

        private void SpawnInitial()
        {
            int afterCheckPointChunksAmount = 30;

            for (int i = 1; i < _config.InitialAmount; i++)
                Spawn();

            _spawner.SpawnCheckPoint();

            for (int i = 1; i < afterCheckPointChunksAmount; i++)
                Spawn();
        }

        private void Spawn()
        {
            bool withMoney = _config.IsMoneyEnabled && _config.MoneyDensity >= Random.value;
            bool withObstacles = _config.IsObstaclesEnabled && _config.GeneralObstacleDensity >= Random.value;
            _spawner.Spawn(withMoney, withObstacles);
        }

        private void OnPassed(Chunk chunk)
        {
            Spawn();
        }

        public override void Disable()
        {
            _gameState.OnSwitched -= OnSateSwitched;
            _spawner.OnChunkPassed -= OnPassed;
        }
    }
}
