using Assets._Project.Architecture;
using Assets._Project.GameStateControl;
using Assets._Project.Input;
using System;
using UnityEngine;

namespace Assets._Project.Systems.Driving
{
    public class DrivingSystem : GameSystem
    {
        private readonly DrivingConfig _config;
        private readonly IPlayerInput _playerInpu;
        private readonly IDrivable _drivable;
        private readonly GameState _gameState;
        private float _gasValue;

        public DrivingSystem(DrivingConfig config, IPlayerInput playerInput, IDrivable drivable, GameState gameState)
        {
            _config = config;
            _playerInpu = playerInput;
            _drivable = drivable;
            _gameState = gameState;
        }

        public override void Enable()
        {
            _playerInpu.OnStear += Stear;
        }

        

        private void Stear(float value)
        {
            if (_gameState.Current == GameStates.Run)
            {
                _drivable?.ChangeLine(value * _config.StearStep, _config.StearDuration, _config.StearAngle);
            }
        }

        public override void Tick()
        {
            _gasValue = _config.Speed;
        }

        public override void FixedTick()
        {
            if (_gameState.Current == GameStates.Run)
            {
                _drivable?.Accelerate(_gasValue * Time.fixedDeltaTime);
            }
        }

        public override void Disable()
        {
            _playerInpu.OnStear -= Stear;
        }
    }
}
