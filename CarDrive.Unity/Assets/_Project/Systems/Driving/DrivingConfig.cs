using NaughtyAttributes;
using UnityEngine;

namespace Assets._Project.Systems.Driving
{
    [CreateAssetMenu(menuName = "Config/Driving")]
    public class DrivingConfig : ScriptableObject
    {
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float StartupAcceleration { get; private set; }
        [field: SerializeField, MinMaxSlider(-1, 1)] public Vector2 GasRegulationModifire { get; private set; }
        [SerializeField] private MeshRenderer _roadReference;
        [field: SerializeField] public int RoadLines { get; private set; }
        [field: SerializeField] public float StearDuration { get; private set; }
        [field: SerializeField, Range(0, 90)] public float StearAngle { get; private set; }
        public float StearStep => _roadReference.bounds.size.x / RoadLines;

    }
}
