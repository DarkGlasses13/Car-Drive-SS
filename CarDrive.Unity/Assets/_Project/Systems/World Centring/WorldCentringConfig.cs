using UnityEngine;

namespace Assets._Project.Systems.WorldCentring
{
    [CreateAssetMenu(menuName = "Config/World Centring")]
    public class WorldCentringConfig : ScriptableObject
    {
        [field: SerializeField] public int ChunksPassedBeforeCentering { get; private set; }
    }
}