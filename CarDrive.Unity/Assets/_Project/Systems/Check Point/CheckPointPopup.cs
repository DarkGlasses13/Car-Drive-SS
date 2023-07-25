using Assets._Project.Architecture.UI;
using System;
using UnityEngine;

namespace Assets._Project.Systems.CheckPoint
{
    public class CheckPointPopup : MonoBehaviour, IUIElement
    {
        [field: SerializeField] public RectTransform BalanceAndPlayButtonSection { get; private set; }
        [field: SerializeField] public RectTransform EquipmentSection { get; private set; }
        [field: SerializeField] public RectTransform MergeAndBuyButtonSection { get; private set; }

        public void Open(Action callback = null)
        {
            gameObject.SetActive(true);
            callback?.Invoke();
        }

        public void Close(Action callback = null)
        {
            gameObject.SetActive(false);
            callback?.Invoke();
        }
    }
}
