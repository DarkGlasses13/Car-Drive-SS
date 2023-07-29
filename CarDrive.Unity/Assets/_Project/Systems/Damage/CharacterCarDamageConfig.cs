using UnityEngine;

namespace Assets._Project.Systems.Damage
{
    [CreateAssetMenu(menuName = "Config/Character Car Damage")]
    public class CharacterCarDamageConfig : ScriptableObject
    {
        [field: SerializeField] public LayerMask HitboxLayerMask { get; private set; }
        [field: SerializeField] public int MaxLives { get ; private set; }
        [field: SerializeField] public Vector3 HitboxBounds { get; private set; }
        [field: SerializeField] public float ImpregnabilityTime { get; private set; }
    }
}
