using Assets._Project.Architecture.UI;
using Assets._Project.Input;
using Assets._Project.Systems.Driving;
using UnityEngine;

namespace Assets._Project.Systems.Tutorial
{
    public class GasRegulationState : TutorialState
    {
        private readonly DrivingSystem _drivingSystem;
        private readonly IPlayerInput _playerInput;
        private readonly IUIElement _popup;

        public GasRegulationState(ITutorialSystem system, DrivingSystem drivingSystem, IPlayerInput playerInput, IUIElement popup) : base(system)
        {
            _drivingSystem = drivingSystem;
            _playerInput = playerInput; 
            _popup = popup;
        }

        public override void Enter()
        {
            _popup.Show(OnPopupShown);
        }

        private void OnPopupShown()
        {
            _drivingSystem.EnableGasRegulation();
            _playerInput.OnVerticalSwipeWithThreshold += OnSwipe;
        }

        private void OnSwipe(float value)
        {
            Debug.Log("Tutorial ended");
            //_system.SwitchToNextState();
        }
    }
}
