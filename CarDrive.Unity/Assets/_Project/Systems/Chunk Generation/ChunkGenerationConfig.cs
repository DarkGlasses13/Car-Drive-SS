using UnityEngine;

namespace Assets._Project.Systems.ChunkGeneration
{
    [CreateAssetMenu(menuName = "Config/Chunk Generation")]
    public class ChunkGenerationConfig : ScriptableObject
    {
        [field: SerializeField] public int InitialAmount { get; private set; }
        //[field: SerializeField] public int ChunksPassedBeforeCentering { get; private set; }
    }
}
