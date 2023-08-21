using System;
using UnityEngine;

namespace Assets._Project.Input
{
    public interface IPlayerInput
    {
        event Action OnAnyInput, OnInteract;
        event Action<Vector2> OnSwipe, OnSwipeEnded;
        event Action<float> OnVerticalSwipeWithThreshold;
        void Enable();
        void Disable();
    }
}