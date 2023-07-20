using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Assets._Project.Input
{
    public class UniversalPlayerInput : IPlayerInput
    {
        public event Action<float> OnGasRegulate;
        public event Action<float> OnStear;
        private readonly IPlayerInputConfig _config;
        public float StearValue => _config.StearInputAction.ReadValue<float>();
        public float GasValue => _config.GasRegulationInputAction.ReadValue<float>();

        public UniversalPlayerInput(IPlayerInputConfig config)
        {
            _config = config;
            EnhancedTouchSupport.Enable();
        }

        public void Enable()
        {
            _config.GasRegulationInputAction.Enable();
            _config.StearInputAction.Enable();
            _config.StearInputAction.performed += Stear;
            Touch.onFingerUp += Swipe;
        }

        private void Swipe(Finger finger)
        {
            Vector2 swipeDirection = finger.screenPosition - finger.currentTouch.startScreenPosition;

            if (swipeDirection.magnitude >= _config.DeadZone)
            {
                Vector2Int direction = CalculateDirection(swipeDirection.normalized);

                if (direction.x != 0)
                    OnStear?.Invoke(direction.x);

                if (direction.y != 0)
                    OnGasRegulate?.Invoke(direction.y);
            }
        }

        private void Stear(InputAction.CallbackContext context)
        {
            OnStear?.Invoke(context.ReadValue<float>());
        }

        private Vector2Int CalculateDirection(Vector2 normalizedSwipeDirection)
        {
            int x = Mathf.RoundToInt(normalizedSwipeDirection.x 
                - _config.SwipeDirectionThreshold * Mathf.Sign(normalizedSwipeDirection.x));
            int y = Mathf.RoundToInt(normalizedSwipeDirection.y 
                - _config.SwipeDirectionThreshold * Mathf.Sign(normalizedSwipeDirection.y));
            return new Vector2Int(x, y);
        }

        public void Disable()
        {
            _config.GasRegulationInputAction.Disable();
            _config.StearInputAction.Disable();
            _config.StearInputAction.performed -= Stear;
            Touch.onFingerUp -= Swipe;
        }
    }
}
