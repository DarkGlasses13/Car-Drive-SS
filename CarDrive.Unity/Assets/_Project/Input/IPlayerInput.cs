using System;
using UnityEngine;

namespace Assets._Project.Input
{
    public interface IPlayerInput
    {
        event Action OnInteract;
        event Action<Vector2> OnSwipeEnded;
        event Action<Vector2> OnSwipe;
        void Enable();
        void Disable();
    }
}