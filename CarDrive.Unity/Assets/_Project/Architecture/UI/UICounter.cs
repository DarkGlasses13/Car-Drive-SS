using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Architecture.UI
{
    public class UICounter : MonoBehaviour, IUICounter
    {
        [SerializeField] protected CanvasGroup _canvasGroup;
        [SerializeField] protected TMP_Text _count;
        [SerializeField] protected Image _icon;

        public Sprite Icon { get => _icon.sprite; set => _icon.sprite = value; }

        public virtual void Open(Action callback)
        {
            _canvasGroup.alpha = 0;
            callback?.Invoke();
        }

        public virtual void Close(Action callback)
        {
            _canvasGroup.alpha = 1;
            callback?.Invoke();
        }

        public void Set(string value)
        {
            _count.text = value;
            OnSeted();
        }

        protected virtual void OnSeted() { }
    }
}