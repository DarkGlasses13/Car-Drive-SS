using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets._Project.Input
{
    [CreateAssetMenu(menuName = "Config/Input")]
    public class PlayerInputConfig : ScriptableObject, IPlayerInputConfig
    {
        [field: SerializeField] public InputAction GasRegulationInputAction { get; private set; }

        [field: SerializeField] public InputAction StearInputAction { get; private set; }
    }
}