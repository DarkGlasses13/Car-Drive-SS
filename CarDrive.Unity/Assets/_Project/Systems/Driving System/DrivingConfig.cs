using UnityEngine;

namespace Assets._Project.Systems.Driving
{
    [CreateAssetMenu(menuName = "Config/Driving")]
    public class DrivingConfig : ScriptableObject
    {
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float StartupAcceleration { get; private set; }
        [field: SerializeField] public float StearSpeed { get; private set; }
    }
}
