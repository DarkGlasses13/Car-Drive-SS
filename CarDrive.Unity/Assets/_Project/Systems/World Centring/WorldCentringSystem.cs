using Assets._Project.Architecture;
using Assets._Project.Systems.ChunkGeneration;
using UnityEngine;

namespace Assets._Project.Systems.WorldCentring
{
    public class WorldCentringSystem : GameSystem
    {
        private readonly WorldCentringConfig _config;
        private readonly ChunkSpawner _chunkSpawner;
        private int _lastPassedCount;

        public WorldCentringSystem(WorldCentringConfig config, ChunkSpawner chunkSpawner) 
        {
            _config = config;
            _chunkSpawner = chunkSpawner;
        }

        public override void Enable()
        {
            _chunkSpawner.OnChunkPassed += OnChunkPassed;
        }

        private void OnChunkPassed(Chunk chunk)
        {
            if (_chunkSpawner.PassedChunksCount >= _lastPassedCount + _config.ChunksPassedBeforeCentering)
            {
                Debug.Log("Center");
                _lastPassedCount = _chunkSpawner.PassedChunksCount;
            }
        }

        public override void Disable()
        {
            _chunkSpawner.OnChunkPassed -= OnChunkPassed;
        }
    }
}
