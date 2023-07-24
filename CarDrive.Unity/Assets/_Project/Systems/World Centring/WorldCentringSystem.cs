using Assets._Project.Architecture;
using Assets._Project.Systems.ChunkGeneration;

namespace Assets._Project.Systems.WorldCentring
{
    public class WorldCentringSystem : GameSystem
    {
        private readonly WorldCentringConfig _config;
        private int _lastPassedCount;

        public WorldCentringSystem(WorldCentringConfig config) 
        {
            _config = config;
        }

        public override void Enable()
        {
        }

        private void OnChunkPassed(Chunk chunk)
        {
            //if (_chunkSpawner.SpawnedChunksCount >= _lastPassedCount + _config.ChunksPassedBeforeCentering)
            //{
            //    Debug.Log("Center");
            //    _lastPassedCount = _chunkSpawner.SpawnedChunksCount;
            //}
        }

        public override void Disable()
        {
        }
    }
}
