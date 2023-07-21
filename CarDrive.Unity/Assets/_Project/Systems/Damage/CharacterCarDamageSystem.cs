using Assets._Project.Architecture;
using Assets._Project.GameStateControl;
using Assets._Project.Helpers;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Systems.Damage
{
    public class CharacterCarDamageSystem : GameSystem
    {
        private readonly LocalAssetLoader _assetLoader;
        private readonly GameState _gameState;
        private readonly IDamageable _damageable;
        private CharacterCarDamageConfig _config;
        private readonly Collider[] _hits = new Collider[5];
        private int _lives;

        public CharacterCarDamageSystem(LocalAssetLoader assetLoader, GameState gameState, IDamageable damageable)
        {
            _assetLoader = assetLoader;
            _gameState = gameState;
            _damageable = damageable;
        }

        public override async Task InitializeAsync()
        {
            _config = await _assetLoader.Load<CharacterCarDamageConfig>("Character Car Damage Config");
            _lives = _config.MaxLives;
        }

        public override void FixedTick()
        {
            if (_gameState.Current == GameStates.Run)
            {
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
                    _damageable.Die();
                }
            }
        }
    }
}