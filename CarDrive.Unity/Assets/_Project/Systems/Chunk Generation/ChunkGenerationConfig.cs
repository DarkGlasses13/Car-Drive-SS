using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets._Project.Systems.ChunkGeneration
{
    [CreateAssetMenu(menuName = "Config/Chunk Generation")]
    public class ChunkGenerationConfig : ScriptableObject
    {
        [field: SerializeField] public AssetLabelReference ChunkAssetLabel { get; private set; }
        [field: SerializeField] public int ChunksBetweenCheckPoints { get; private set; }
        [field: SerializeField] public int ChunksPassedBeforeDespawn { get; private set; }
        [field: SerializeField, Label("Enable Obstacles")] public bool IsObstaclesEnabled { get; private set; }
        [field: SerializeField, Range(0, 1), ShowIf("IsObstaclesEnabled")] public float GeneralObstacleDensity { get; private set; }
        [field: SerializeField, Label("Enable Money")] public bool IsMoneyEnabled { get; private set; }
        [field: SerializeField, Range(0, 1), ShowIf("IsMoneyEnabled")] public float MoneyDensity { get; private set; }
    }
}
