using Assets._Project.Architecture.UI;
using System;
using UnityEngine;

namespace Assets._Project.Systems.CheckPoint
{
    public class CheckPointPopup : MonoBehaviour, IUIElement
    {
        [field: SerializeField] public RectTransform MoneyBalanceAndPlayButtonPlace { get; private set; }
        [field: SerializeField] public RectTransform OtherElementsPlace { get; private set; }

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
