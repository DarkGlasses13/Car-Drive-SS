using Assets._Project.Architecture;
using Assets._Project.Input;
using UnityEngine;

namespace Assets._Project.Systems.Driving
{
    public class DrivingSystem : GameSystem
    {
        private readonly DrivingConfig _config;
        private readonly IPlayerInput _playerInput;
        private readonly IDrivable _drivable;
        private float _gasValue;

        public DrivingSystem(DrivingConfig config, IPlayerInput playerInput, IDrivable drivable)
        {
            _config = config;
            _playerInput = playerInput;
            _drivable = drivable;
        }

        public override void Tick()
        {
            _gasValue = _config.Speed;
        }

        public override void FixedTick()
        {
            _drivable?.RegulateGas(_gasValue * Time.fixedDeltaTime);
        }
    }
}
