using System;
using UnityEngine;

namespace Assets._Project.Systems.ChunkGeneration
{
    public class CheckPointChunk : Chunk
    {
        public bool IsTriggered { get; private set; } = false;

        public event Action<CheckPointChunk> OnEnter;

        private void OnTriggerEnter(Collider other)
        {
            IsTriggered = true;
            OnEnter?.Invoke(this);
        }

        protected override void OnTriggerExit(Collider other)
        {
            base.OnTriggerExit(other);
            IsTriggered = false;
        }
    }
}
