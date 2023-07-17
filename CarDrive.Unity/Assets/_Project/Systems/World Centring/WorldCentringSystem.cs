using Assets._Project.Architecture;
using Assets._Project.Systems.ChunkGeneration;
using UnityEngine;

namespace Assets._Project.Systems.WorldCentring
{
    public class WorldCentringSystem : GameSystem
    {
        private readonly WorldCentringConfig _config;
        private readonly ChunkGenerationData _chunkgenerationData;
        private int _lastPassedCount;

        public WorldCentringSystem(WorldCentringConfig config, ChunkGenerationData chunkGenerationData) 
        {
            _config = config;
            _chunkgenerationData = chunkGenerationData;
        }

        public override void Tick()
        {
            if (_chunkgenerationData.PassedChunksCount >= _lastPassedCount + _config.ChunksPassedBeforeCentering)
            {
                Debug.Log("Center");
                _lastPassedCount = _chunkgenerationData.PassedChunksCount;
            }
        }
    }
}
