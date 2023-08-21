using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Assets._Project.Input
{
    public class UniversalPlayerInput : IPlayerInputHandler, IPlayerInput
    {
        public event Action OnAnyInput;
        public event Action OnInteract;
        public event Action<Vector2> OnSwipe;
        public event Action<Vector2> OnSwipeEnded;
        public event Action<float> OnVerticalSwipeWithThreshold;

        private readonly IPlayerInputConfig _config;
        private Vector2 _swipeDelta;

        public UniversalPlayerInput(IPlayerInputConfig config)
        {
            _config = config;
            EnhancedTouchSupport.Enable();
        }

        public void Enable()
        {
            _config.InteractAction.Enable();
            _config.GasRegulationInputAction.Enable();
            //_config.StearInputAction.Enable();
            _config.InteractAction.performed += Interact;
            //_config.StearInputAction.performed += Stear;
            _config.GasRegulationInputAction.performed += RegulateGas;
            //Touch.onFingerMove += Swipe;
            Touch.onFingerDown += OnFingerDown;
            Touch.onFingerUp += EndSwipe;
        }

        private void OnFingerDown(Finger finger)
        {
            OnAnyInput?.Invoke();
        }

        private void EndSwipe(Finger finger)
        {
            Vector2 swipeDirection = finger.screenPosition - finger.currentTouch.startScreenPosition;
            OnSwipeEnded?.Invoke(swipeDirection);

            if (swipeDirection.magnitude >= _config.DeadZone)
            {
                Vector2Int direction = CalculateDirection(swipeDirection.normalized);

                //if (direction.x != 0)
                //    OnStear?.Invoke(direction.x);

                //OnStear?.Invoke(0);

                if (direction.y != 0)
                    OnVerticalSwipeWithThreshold?.Invoke(direction.y);
            }

            if (finger.currentTouch.isTap)
                OnInteract?.Invoke();
        }

        public void Read()
        {
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
                OnAnyInput?.Invoke();

            if (Touch.activeTouches.Count > 0)
            {
                _swipeDelta = Touch.activeTouches[0].delta;
                OnSwipe?.Invoke(_swipeDelta);
            }

            OnSwipe?.Invoke(Vector2.zero);
        }

        private void Interact(InputAction.CallbackContext context)
        {
            OnInteract?.Invoke();
        }

        private void RegulateGas(InputAction.CallbackContext context)
        {
            float value = context.ReadValue<float>();

            if (value != 0)
                OnSwipeEnded?.Invoke(Vector2.up * value);
        }

        private void Stear(InputAction.CallbackContext context)
        {
            //float value = context.ReadValue<float>();

            //if (value != 0)
            //    OnStear?.Invoke(Vector2.up * value);
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
            _config.InteractAction.Disable();
            _config.GasRegulationInputAction.Disable();
            //_config.StearInputAction.Disable();
            _config.InteractAction.performed -= Interact;
            //_config.StearInputAction.performed -= Stear;
            _config.GasRegulationInputAction.performed -= RegulateGas;
            //Touch.onFingerMove -= Swipe;
            Touch.onFingerDown -= OnFingerDown;
            Touch.onFingerUp -= EndSwipe;
        }
    }
}
