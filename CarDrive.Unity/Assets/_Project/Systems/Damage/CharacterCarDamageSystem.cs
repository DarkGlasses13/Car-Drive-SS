using Assets._Project.Architecture;
using Assets._Project.GameStateControl;
using Assets._Project.Helpers;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Systems.Damage
{
    public class CharacterCarDamageSystem : GameSystem, IGameStateSwitchHandler
    {
        private readonly LocalAssetLoader _assetLoader;
        private readonly GameState _gameState;
        private readonly IDamageable _damageable;
        private readonly Coroutiner _coroutiner;
        private CharacterCarDamageConfig _config;
        private readonly Collider[] _hits = new Collider[5];
        private int _lives;
        private bool _isImpregnability;

        public CharacterCarDamageSystem(LocalAssetLoader assetLoader, GameState gameState, IDamageable damageable, Coroutiner coroutiner)
        {
            _assetLoader = assetLoader;
            _gameState = gameState;
            _damageable = damageable;
            _coroutiner = coroutiner;
        }

        public override async Task InitializeAsync()
        {
            _config = await _assetLoader.Load<CharacterCarDamageConfig>("Character Car Damage Config");
            _lives = _config.MaxLives;
        }

        public override void Restart()
        {
            _coroutiner.StartCoroutine(RestoreRoutine());
        }

        public override void Enable()
        {
            _gameState.OnSwitched += OnSateSwitched;
        }

        public override void FixedTick()
        {
            if (_gameState.Current == GameStates.Run)
            {
                if (_isImpregnability)
                    return;

                int hitsCount = Physics.OverlapBoxNonAlloc(_damageable.Center, _config.HitboxBounds / 2,
                    _hits, _damageable.Rotation, _config.HitboxLayerMask);

                if (hitsCount > 0 )
                {
                    TakeDamage();
                }
            }
        }

        private void TakeDamage()
        {
            if (_gameState.Current == GameStates.Run)
            {
                _lives--;
                _lives = Mathf.Clamp(_lives, 0, _config.MaxLives);

                if (_lives <= 0)
                {
                    _gameState.Switch(GameStates.Lose);
                    _damageable.OnDie();
                }
            }
        }

        private IEnumerator RestoreRoutine()
        {
            _damageable.OnRestore();
            _damageable.ShowAura();
            _isImpregnability = true;
            yield return new WaitForSeconds(_config.ImpregnabilityTime);
            _isImpregnability = false;
            _damageable.HideAura();
        }

        public void OnSateSwitched(GameStates state)
        {
            if (state == GameStates.Run)
                _coroutiner.StartCoroutine(RestoreRoutine());
        }

        public override void Disable()
        {
            _gameState.OnSwitched -= OnSateSwitched;
        }
    }
}