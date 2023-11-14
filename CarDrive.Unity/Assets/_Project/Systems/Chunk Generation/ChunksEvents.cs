using Assets._Project.Systems.ChunkGeneration;
using System;

namespace Assets._Project.Systems.Chunk_Generation
{
    public class ChunksEvents
    {
        public event Action<CheckPointChunk> OnCheckPointEnter;
        public event Action<CheckPointChunk> OnCheckPointPass;
        public event Action<Chunk> OnAnyPass;
        public bool IsTriggered { get; set; }
        public void Enter(CheckPointChunk chunk) => OnCheckPointEnter?.Invoke(chunk);
        public void Pass(CheckPointChunk chunk) => OnCheckPointPass?.Invoke(chunk);
        public void AnyPass(Chunk chunk) => OnAnyPass?.Invoke(chunk);
    }
}
