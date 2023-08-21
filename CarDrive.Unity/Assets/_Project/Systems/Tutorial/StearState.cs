using Assets._Project.Architecture.UI;
using Assets._Project.Input;
using Assets._Project.Systems.Driving;

namespace Assets._Project.Systems.Tutorial
{
    public class StearState : TutorialState
    {
        private readonly DrivingSystem _drivingSystem;
        private readonly IPlayerInput _playerInput;
        private readonly IUIElement _popup;

        public StearState(ITutorialSystem system, DrivingSystem drivingSystem, IPlayerInput playerInput, IUIElement popup) : base(system)
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
            _drivingSystem.DisableGasRegulation();
            _playerInput.OnAnyInput += OnAnyInput;
        }

        private void OnAnyInput()
        {
            _popup.Hide(_system.SwitchToNextState);
        }

        public override void Exit()
        {
            _playerInput.OnAnyInput -= _system.SwitchToNextState;
        }
    }
}
