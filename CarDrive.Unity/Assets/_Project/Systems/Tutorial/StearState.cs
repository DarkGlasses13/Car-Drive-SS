using Assets._Project.Architecture.UI;
using Assets._Project.Helpers;
using Assets._Project.Input;
using Assets._Project.Systems.ChunkGeneration;
using Assets._Project.Systems.Driving;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Systems.Tutorial
{
    public class StearState : TutorialState
    {
        private readonly DrivingSystem _drivingSystem;
        private readonly IPlayerInput _playerInput;
        private readonly IUIElement _popup;
        private readonly Coroutiner _coroutiner;
        private readonly CheckPointChunk _checkPoint;
        private readonly WaitForSeconds _waitForOpenDelay = new(1.5f);
        private Coroutine _waitDelayRoutine;

        public StearState(ITutorialSystem system, DrivingSystem drivingSystem,
            IPlayerInput playerInput, IUIElement popup, Coroutiner coroutiner, CheckPointChunk checkPoint) : base(system)
        {
            _drivingSystem = drivingSystem;
            _playerInput = playerInput;
            _popup = popup;
            _coroutiner = coroutiner;
            _checkPoint = checkPoint;
        }

        public override void Enter()
        {
            _drivingSystem.DisableGasRegulation();
            _popup.Show();
            _checkPoint.OnEnter += OnCheckPointEnter;
            _playerInput.OnAnyInput += OnAnyInput;
        }

        private void OnCheckPointEnter(CheckPointChunk chunk)
        {
            if (_waitDelayRoutine != null)
                _coroutiner.StopCoroutine(_waitDelayRoutine);

            _popup.Hide();
            _system.SwitchToNextState();
        }

        private void OnAnyInput()
        {
            _playerInput.OnAnyInput -= OnAnyInput;
            _popup.Hide();
            _waitDelayRoutine = _coroutiner.StartCoroutine(WaitDelayRoutine());
        }

        private IEnumerator WaitDelayRoutine()
        {
            yield return _waitForOpenDelay;
            _system.SwitchToNextState();
        }

        public override void Exit()
        {
            _checkPoint.OnEnter -= OnCheckPointEnter;
            _playerInput.OnAnyInput -= OnAnyInput;
        }
    }
}
