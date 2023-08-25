using Assets._Project.Systems.Collecting;
using Assets._Project.Systems.Shop;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Systems.Tutorial
{
    public class MergeState : TutorialState
    {
        private readonly Money _money;
        private readonly CollectablesConfig _collectablesConfig;
        private readonly InventorySystem _inventorySystem;
        private readonly ShopSystem _shopSystem;
        private readonly PriceTagButton _lootBoxBuyButton;
        private readonly TutorialHighlighter _highlighter;
        private readonly UIInventory _uiInventory;
        private readonly Image _finger;
        private readonly Button _playButton;
        private readonly Player _player;
        private readonly Runner _runner;
        private Tweener _fingerShakeTween;
        private TweenerCore<Vector3, Vector3, VectorOptions> _fingerSwapTween;

        public MergeState(ITutorialSystem system, Money money, CollectablesConfig collectablesConfig,
            InventorySystem inventorySystem, ShopSystem shopSystem, PriceTagButton lootBoxBuyButton,
            TutorialHighlighter highlighter, UIInventory uiInventory, Image finger, Button playButton,
            Player player, Runner runner) : base(system)
        {
            _money = money;
            _collectablesConfig = collectablesConfig;
            _inventorySystem = inventorySystem;
            _shopSystem = shopSystem;
            _lootBoxBuyButton = lootBoxBuyButton;
            _highlighter = highlighter;
            _uiInventory = uiInventory;
            _finger = finger;
            _playButton = playButton;
            _player = player;
            _runner = runner;
        }

        public override void Enter()
        {
            _money.Add(_collectablesConfig.LootBoxPrice * 2);
            _shopSystem.Disable();
            _playButton.interactable = false;
            Vector2 position = _lootBoxBuyButton.Button.targetGraphic.rectTransform.position;
            Vector2 size = _lootBoxBuyButton.Button.targetGraphic.rectTransform.rect.size;
            _lootBoxBuyButton.Button.onClick.AddListener(OnBuyButtonClicked);
            _finger.gameObject.SetActive(true);
            _finger.rectTransform.position = _lootBoxBuyButton.Button.targetGraphic.rectTransform.position;
            _fingerShakeTween = _finger.rectTransform
                .DOShakeScale(1, 0.5f, 5, 0)
                .SetLoops(-1)
                .Play();
        }

        private void OnBuyButtonClicked()
        {
            _lootBoxBuyButton.Button.onClick.RemoveListener(OnBuyButtonClicked);
            _fingerShakeTween?.Pause();
            _inventorySystem.Add("it_Egn_4", "it_Egn_4");
            _finger.rectTransform.position = _uiInventory.GetFirstSlotPosition();
            _finger.transform.localScale = Vector2.one;
            _fingerSwapTween = _finger.rectTransform
                .DOMove(_uiInventory.GetLastSlotPosition(), 1)
                .SetLoops(-1, LoopType.Yoyo)
                .Play();

            _inventorySystem.OnMerged += OnMerged;
            _inventorySystem.OnSwaped += OnSwaped;
        }

        private void OnSwaped(int from, int to)
        {
            _fingerSwapTween?.ChangeStartValue(_uiInventory.GetFirstSlotPosition());
            _fingerSwapTween?.ChangeEndValue(_uiInventory.GetLastSlotPosition());
            _fingerSwapTween?.Restart();
        }

        private void OnMerged(int slot)
        {
            _inventorySystem.OnSwaped -= OnSwaped;
            _inventorySystem.OnSwaped += OnSwapedWhileEquipment;
            _inventorySystem.OnMerged -= OnMerged;
            _fingerSwapTween?.ChangeStartValue(_uiInventory.GetFirstSlotPosition());
            _fingerSwapTween.ChangeEndValue(_uiInventory.GetEquipmentSlotPosition(2));
            _fingerSwapTween?.Restart();
            _inventorySystem.OnEquiped += OnEquip;
        }

        private void OnSwapedWhileEquipment(int from, int to)
        {
            _fingerSwapTween?.ChangeStartValue(_uiInventory.GetSlotPosition(to));
            _fingerSwapTween?.Restart();
        }

        private void OnEquip(int slot)
        {
            _inventorySystem.OnSwaped -= OnSwapedWhileEquipment;
            _fingerSwapTween?.Pause();
            _playButton.interactable = true;
            _playButton.onClick.AddListener(OnPlayButtonClicked);
            _finger.rectTransform.position = _playButton.targetGraphic.rectTransform.position;
            _fingerShakeTween.Restart();
        }

        private void OnPlayButtonClicked()
        {
            _playButton.onClick.RemoveListener(OnPlayButtonClicked);
            _player.IsTutorialCompleted = true;
            _player.ResetSats();
            _shopSystem?.Enable();
            _money.SpendAll();
            _inventorySystem?.Clear();
            _runner.ForceRestart();
        }

        public override void Exit()
        {
            _shopSystem.Enable();
        }
    }
}
