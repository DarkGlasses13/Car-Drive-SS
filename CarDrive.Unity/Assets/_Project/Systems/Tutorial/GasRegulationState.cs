using Assets._Project.Architecture.UI;
using Assets._Project.Systems.Chunk_Generation;
using Assets._Project.Systems.ChunkGeneration;
using Assets._Project.Systems.Driving;

namespace Assets._Project.Systems.Tutorial
{
    public class GasRegulationState : TutorialState
    {
        private readonly DrivingSystem _drivingSystem;
        private readonly IUIElement _popup;
        private readonly ChunksEvents _checkPointEvent;

        public GasRegulationState(ITutorialSystem system, DrivingSystem drivingSystem,
            IUIElement popup, ChunksEvents checkPointEvent) : base(system)
        {
            _drivingSystem = drivingSystem;
            _popup = popup;
            _checkPointEvent = checkPointEvent;
        }

        public override void Enter()
        {
            if (_checkPointEvent.IsTriggered)
            {
                _system.SwitchToNextState();
                return;
            }

            _popup.Show();
            _drivingSystem.EnableGasRegulation();
            _drivingSystem.OnGasRegulated += OnGasRegulated;
            _checkPointEvent.OnCheckPointEnter += OnCheckPointEnter;
        }

        private void OnCheckPointEnter(CheckPointChunk chunk)
        {
            _popup.Hide();
            _system.SwitchToNextState();
        }

        private void OnGasRegulated(int value)
        {
            _drivingSystem.OnGasRegulated -= OnGasRegulated;
            _popup.Hide();
        }

        public override void Exit()
        {
            _drivingSystem.OnGasRegulated -= OnGasRegulated;
            _checkPointEvent.OnCheckPointEnter -= OnCheckPointEnter;
        }
    }
}
