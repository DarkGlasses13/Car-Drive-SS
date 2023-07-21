using UnityEngine.InputSystem;

namespace Assets._Project.Input
{
    public interface IPlayerInputConfig
    {
        InputAction InteractAction { get; }
        InputAction StearInputAction { get; }
        InputAction GasRegulationInputAction { get; }
        float DeadZone { get; }
        float SwipeDirectionThreshold { get; }
    }
}