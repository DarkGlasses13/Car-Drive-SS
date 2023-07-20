using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets._Project.Systems.ChunkGeneration
{
    [CreateAssetMenu(menuName = "Config/Chunk Generation")]
    public class ChunkGenerationConfig : ScriptableObject
    {
        [field: SerializeField] public AssetLabelReference EmptyChunkAssetLabel { get; private set; }
        [field: SerializeField] public AssetLabelReference ObstaclebleChunkAssetLabel { get; private set; }
        [field: SerializeField] public int InitialAmount { get; private set; }
        [field: SerializeField, Label("Enable Obstacles")] public bool IsObstaclesEnabled { get; private set; }
        [field: SerializeField, Range(0, 1), ShowIf("IsObstaclesEnabled")] public float GeneralObstacleDencity { get; private set; }
    }
}
