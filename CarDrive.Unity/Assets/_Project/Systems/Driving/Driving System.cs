using Assets._Project.Architecture;
using Assets._Project.GameStateControl;
using Assets._Project.Helpers;
using Assets._Project.Input;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Systems.Driving
{
    public class DrivingSystem : GameSystem
    {
        private readonly LocalAssetLoader _assetLoader;
        private readonly IPlayerInput _playerInpu;
        private readonly IDrivable _drivable;
        private readonly GameState _gameState;
        private readonly Coroutiner _coroutiner;
        private DrivingConfig _config;
        private float _gasValue;
        private Coroutine _gasRegulationRoutine;
        private float _gasRegulation = 1;

        public DrivingSystem(LocalAssetLoader assetLoader, IPlayerInput playerInput, IDrivable drivable, GameState gameState, Coroutiner coroutiner)
        {
            _assetLoader = assetLoader;
            _playerInpu = playerInput;
            _drivable = drivable;
            _gameState = gameState;
            _coroutiner = coroutiner;
        }

        public override async Task InitializeAsync()
        {
            _config = await _assetLoader.Load<DrivingConfig>("Driving Config");
        }

        public override void Enable()
        {
            _playerInpu.OnStear += Stear;
            _playerInpu.OnGasRegulate += RegulateGas;
        }

        private void RegulateGas(float value)
        {
            if (_gameState.Current == GameStates.Run)
            {
                if (_gasRegulationRoutine != null)
                    _coroutiner.StopCoroutine(_gasRegulationRoutine);

                _gasRegulationRoutine = _coroutiner.StartCoroutine(RegulateGasRoutine(value));
            }
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
            _gasValue = _config.Speed * _gasRegulation;

            if (_gameState.Current == GameStates.Run)
            {
                _drivable?.Accelerate(_gasValue * Time.deltaTime);
            }
        }

        private IEnumerator RegulateGasRoutine(float value)
        {
            float target = 0;

            if (value > 0)
                target = _config.GasRegulationRange.y;

            if (value < 0)
                target = _config.GasRegulationRange.x;

            while (Mathf.Approximately(_gasRegulation, target) == false)
            {
                _gasRegulation = Mathf.MoveTowards(_gasRegulation, target, _config.Speed * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(_config.ManeuverTime);

            while (Mathf.Approximately(_gasRegulation, 1) == false)
            {
                _gasRegulation = Mathf.MoveTowards(_gasRegulation, 1, _config.Speed * Time.deltaTime);
                yield return null;
            }
        }

        public override void Disable()
        {
            _playerInpu.OnStear -= Stear;
            _playerInpu.OnGasRegulate -= RegulateGas;
        }
    }
}
