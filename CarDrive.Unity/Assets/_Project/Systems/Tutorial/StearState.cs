using Assets._Project.Architecture.UI;
using Assets._Project.Helpers;
using Assets._Project.Input;
using Assets._Project.Systems.Chunk_Generation;
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
        private readonly ChunksEvents _checkPointEvent;
        private readonly WaitForSeconds _waitForOpenDelay = new(1.5f);
        private Coroutine _waitDelayRoutine;

        public StearState(ITutorialSystem system, DrivingSystem drivingSystem,
            IPlayerInput playerInput, IUIElement popup, Coroutiner coroutiner, ChunksEvents checkPointEvent) : base(system)
        {
            _drivingSystem = drivingSystem;
            _playerInput = playerInput;
            _popup = popup;
            _coroutiner = coroutiner;
            _checkPointEvent = checkPointEvent;
        }

        public override void Enter()
        {
            _drivingSystem.DisableGasRegulation();
            _popup.Show();
            _checkPointEvent.OnCheckPointEnter += OnCheckPointEnter;
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
            _checkPointEvent.OnCheckPointEnter -= OnCheckPointEnter;
            _playerInput.OnAnyInput -= OnAnyInput;
        }
    }
}
