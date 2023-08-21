using Assets._Project.Architecture;
using Assets._Project.GameStateControl;
using Assets._Project.Helpers;
using Assets._Project.Systems.Collecting;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project.Systems.Restart
{
    public class RestartSystem : GameSystem, IGameStateSwitchHandler
    {
        private readonly GameState _gameState;
        private readonly LocalAssetLoader _assetLoader;
        private readonly Transform _container;
        private readonly IRestartable _restarter;
        private readonly AudioSource _levelMusic;
        private readonly IInventory _inventory;
        private readonly Player _player;
        private readonly Money _money;
        private RestartPopup _popup;

        public RestartSystem(GameState gameState, LocalAssetLoader assetLoader,
            Transform container, IRestartable restarter, AudioSource levelMusic,
            IInventory inventory, Player player, Money money)
        {
            _gameState = gameState;
            _assetLoader = assetLoader;
            _container = container;
            _restarter = restarter;
            _levelMusic = levelMusic;
            _inventory = inventory;
            _player = player;
            _money = money;
        }

        public override async Task InitializeAsync()
        {
            _popup = await _assetLoader.LoadAndInstantiateAsync<RestartPopup>("Restart Popup", _container, isActive: false);
        }

        public override void OnEnable()
        {
            _gameState.OnSwitched += OnSateSwitched;
            _popup.RestartButton.onClick.AddListener(OnRestartButtonClicked);
        }

        private void OnRestartButtonClicked()
        {
            _restarter.Restart();
            _popup.Hide();
            _levelMusic.Play();
            _gameState.Switch(GameStates.Run);
        }

        public void OnSateSwitched(GameStates state)
        {
            if (state == GameStates.Lose)
            {
                _levelMusic.Stop();
                _popup.Show();
                _inventory.Clear();
                _player.ResetSats();
                _money.SpendAll();
            }
        }

        public override void OnDisable()
        {
            _gameState.OnSwitched -= OnSateSwitched;
            _popup.RestartButton.onClick.RemoveListener(OnRestartButtonClicked);
        }
    }
}
