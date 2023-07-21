using System;

namespace Assets._Project.Input
{
    public interface IPlayerInput
    {
        event Action OnInteract;
        event Action<float> OnGasRegulate;
        event Action<float> OnStear;
        float StearValue { get; }
        float GasValue { get; }
        void Enable();
        void Disable();
    }
}