using Assets._Project.Architecture;
using Assets._Project.Architecture.UI;
using Assets._Project.GameStateControl;
using Assets._Project.Systems.ChunkGeneration;
using Assets._Project.Systems.Collecting;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Systems.CheckPoint
{
    public class CheckPointSystem : GameSystem
    {
        private readonly GameState _gameState;
        private readonly CheckPointChunk _checkPoint;
        private readonly UICounter _uiMoneyCounter;
        private readonly RectTransform _uiMoneyCounterRectTransform;
        private readonly Vector2 _defaultUIMoneyCounterSize;
        private readonly Vector3 _defaultUIMoneyCounterPosition;
        private readonly Transform _hudContainer;
        private readonly CheckPointPopup _popup;
        private readonly Button _playButton;
        private readonly Money _money;
        private readonly AudioSource _levelMusic;

        public CheckPointSystem(GameState gameState, Transform hudContainer, CheckPointChunk checkpPoint, 
            CheckPointPopup popup, UICounter uiMoneyCounter, Button playButton, Money money, AudioSource levelMusic)
        {
            _gameState = gameState;
            _hudContainer = hudContainer;
            _checkPoint = checkpPoint;
            _popup = popup;
            _uiMoneyCounter = uiMoneyCounter;
            _uiMoneyCounterRectTransform = uiMoneyCounter.GetComponent<RectTransform>();
            _defaultUIMoneyCounterSize = _uiMoneyCounterRectTransform.sizeDelta;
            _defaultUIMoneyCounterPosition = _uiMoneyCounterRectTransform.anchoredPosition;
            _playButton = playButton;
            _money = money;
            _levelMusic = levelMusic;
            _popup.gameObject.SetActive(false);
        }

        public override void Enable()
        {
            _checkPoint.OnEnter += OnCheckPointEnter;
            _playButton.onClick.AddListener(OnPlayButtonClicked);
            _money.OnChanged += OnMoneyChanged;
        }

        private void OnMoneyChanged(int value)
        {
            _uiMoneyCounter.Set(_money.ToString());
        }

        private void OnPlayButtonClicked()
        {
            _uiMoneyCounter.transform.SetParent(_hudContainer);
            _uiMoneyCounterRectTransform.sizeDelta = _defaultUIMoneyCounterSize;
            _uiMoneyCounterRectTransform.anchoredPosition = _defaultUIMoneyCounterPosition;
            _popup.Close(() => _gameState.Switch(GameStates.Run));
            _levelMusic.Play();
        }

        private void OnCheckPointEnter(CheckPointChunk chunk)
        {
            _gameState.Switch(GameStates.Finish);
            _uiMoneyCounter.transform.SetParent(_popup.BalanceAndPlayButtonSection);
            _levelMusic.Stop();
            _popup.Open();
        }

        public override void Disable()
        {
            _checkPoint.OnEnter -= OnCheckPointEnter;
            _playButton.onClick.RemoveListener(OnPlayButtonClicked);
        }
    }
}
