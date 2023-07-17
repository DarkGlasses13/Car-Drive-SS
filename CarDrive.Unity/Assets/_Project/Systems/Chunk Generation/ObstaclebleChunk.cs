using Assets._Project.Entities.Obstacles;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Systems.ChunkGeneration
{
    public class ObstaclebleChunk : Chunk
    {
        [SerializeField] private Obstacle[] _obstacles;

        public IEnumerable<Obstacle> Obstacles => _obstacles.AsEnumerable();
    }
}
