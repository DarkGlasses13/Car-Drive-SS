using Assets._Project.Architecture;
using Assets._Project.Architecture.UI;
using Assets._Project.GameStateControl;
using Assets._Project.Helpers;
using Assets._Project.Systems.ChunkGeneration;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Systems.CheckPoint
{
    public class CheckPointSystem : GameSystem
    {
        private readonly GameState _gameState;
        private readonly CheckPointChunk _checkPoint;
        private readonly LocalAssetLoader _assetLoader;
        private readonly Transform _container;
        private readonly UICounter _moneyCounter;
        private readonly Transform _hudContainer;
        private CheckPointPopup _popup;
        private Button _playButton;

        public CheckPointSystem(GameState gameState, CheckPointChunk checkpPoint, LocalAssetLoader assetLoader,
            Transform popupContainer, UICounter moneyCounter, Transform hudContainer)
        {
            _gameState = gameState;
            _checkPoint = checkpPoint;
            _assetLoader = assetLoader;
            _container = popupContainer;
            _moneyCounter = moneyCounter;
            _hudContainer = hudContainer;
        }

        public override async Task InitializeAsync()
        {
            _popup = await _assetLoader.LoadAndInstantiateAsync<CheckPointPopup>("Check Point Popup", _container);
            _playButton = await _assetLoader.LoadAndInstantiateAsync<Button>("Play Button", _popup.MoneyBalanceAndPlayButtonPlace);
            await _assetLoader.LoadAndInstantiateAsync<RectTransform>("Equipment", _popup.OtherElementsPlace);
            await _assetLoader.LoadAndInstantiateAsync<RectTransform>("Merge", _popup.OtherElementsPlace);
            await _assetLoader.LoadAndInstantiateAsync<RectTransform>("Shop", _popup.OtherElementsPlace);
            _popup.gameObject.SetActive(false);
        }

        public override void Enable()
        {
            _checkPoint.OnEnter += OnCheckPointEnter;
            _playButton.onClick.AddListener(OnPlayButtonClicked);
        }

        private void OnPlayButtonClicked()
        {
            _moneyCounter.transform.SetParent(_hudContainer);
            _popup.Close(() => _gameState.Switch(GameStates.Run));
        }

        private void OnCheckPointEnter(CheckPointChunk chunk)
        {
            _gameState.Switch(GameStates.Finish);
            _moneyCounter.transform.SetParent(_popup.MoneyBalanceAndPlayButtonPlace);
            _popup.Open();
        }

        public override void Disable()
        {
            _checkPoint.OnEnter -= OnCheckPointEnter;
            _playButton.onClick.RemoveListener(OnPlayButtonClicked);
        }
    }
}
