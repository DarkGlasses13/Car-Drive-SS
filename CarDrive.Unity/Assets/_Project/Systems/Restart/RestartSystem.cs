using Assets._Project.Architecture;
using Assets._Project.GameStateControl;
using Assets._Project.Helpers;
using System.Collections.Generic;
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
        private RestartPopup _popup;

        public RestartSystem(GameState gameState, LocalAssetLoader assetLoader,
            Transform container, IRestartable restarter)
        {
            _gameState = gameState;
            _assetLoader = assetLoader;
            _container = container;
            _restarter = restarter;
        }

        public override async Task InitializeAsync()
        {
            _popup = await _assetLoader.LoadAndInstantiateAsync<RestartPopup>("Restart Popup", _container, isActive: false);
        }

        public override void Enable()
        {
            _gameState.OnSwitched += OnSateSwitched;
            _popup.RestartButton.onClick.AddListener(OnRestartButtonClicked);
        }

        private void OnRestartButtonClicked()
        {
            _restarter.Restart();
            _popup.Close();
            _gameState.Switch(GameStates.Run);
        }

        public void OnSateSwitched(GameStates state)
        {
            if (state == GameStates.Lose)
            {
                _popup.Open();
            }
        }

        public override void Disable()
        {
            _gameState.OnSwitched -= OnSateSwitched;
            _popup.RestartButton.onClick.RemoveListener(OnRestartButtonClicked);
        }
    }
}
