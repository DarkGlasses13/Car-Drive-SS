using System;
using UnityEngine;

namespace Assets._Project.Systems.ChunkGeneration
{
    public class CheckPointChunk : Chunk
    {
        public event Action<CheckPointChunk> OnEnter;

        private void OnTriggerEnter(Collider other)
        {
            OnEnter?.Invoke(this);
        }
    }
}
