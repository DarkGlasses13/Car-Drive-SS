using UnityEngine.InputSystem;

namespace Assets._Project.Input
{
    public interface IPlayerInputConfig
    {
        InputAction GasRegulationInputAction { get; }
        InputAction StearInputAction { get; }
    }
}