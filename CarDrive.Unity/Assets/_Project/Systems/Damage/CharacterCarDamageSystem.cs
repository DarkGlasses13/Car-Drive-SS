using Assets._Project.Architecture;
using Assets._Project.GameStateControl;
using Assets._Project.Helpers;
using Assets._Project.Systems.Collectables;
using Assets._Project.Systems.Collecting;
using Assets._Project.Systems.Driving;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private readonly DrivingSystem _drivingSystem;
        private readonly Money _money;
        private readonly AudioSource _levelMusic;
        private readonly Inventory _inventory;
        private CharacterCarDamageConfig _config;
        private readonly Collider[] _hits = new Collider[5];
        private int _lives;
        private bool _isImpregnability;

        public CharacterCarDamageSystem(LocalAssetLoader assetLoader, GameState gameState,
            IDamageable damageable, Coroutiner coroutiner, DrivingSystem drivingSystem,
            Money money, AudioSource levelMusic, Inventory inventory)
        {
            _assetLoader = assetLoader;
            _gameState = gameState;
            _damageable = damageable;
            _coroutiner = coroutiner;
            _drivingSystem = drivingSystem;
            _money = money;
            _levelMusic = levelMusic;
            _inventory = inventory;
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

        public override void OnEnable()
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
                    _isImpregnability = true;
                    TakeDamage();
                }
            }
        }

        private void TakeDamage()
        {
            if (_gameState.Current == GameStates.Run)
            {
                int minEquipmentLevel = 1;
                IEnumerable<IItem> equipment = _inventory.Equipment.Where(item => item != null);

                if (equipment?.Count() > 0)
                    minEquipmentLevel = equipment.Min(item => item.MergeLevel);

                if (_money.TrySpend(_config.CrashPrice * minEquipmentLevel))
                {
                    _damageable.OnMoneyLose();
                    _coroutiner.StartCoroutine(TakeDamageRoutine());
                }
                else
                {
                    _gameState.Switch(GameStates.Lose);
                    _damageable.OnDie();
                }
            }
        }

        private IEnumerator TakeDamageRoutine()
        {
            _levelMusic.Pause();
            _drivingSystem.Disable();
            _damageable.OnCrash();
            yield return new WaitForSeconds(1);
            _levelMusic.Play();
            _drivingSystem.Enable();
            _damageable.ShowAura();
            yield return new WaitForSeconds(_config.ImpregnabilityTime);
            _isImpregnability = false;
            _damageable.HideAura();
        }

        private IEnumerator RestoreRoutine()
        {
            _lives = _config.MaxLives;
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

        public override void OnDisable()
        {
            _gameState.OnSwitched -= OnSateSwitched;
        }
    }
}