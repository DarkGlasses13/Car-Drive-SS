using Assets._Project.Architecture.UI;
using DG.Tweening;
using System;
using UnityEngine;

namespace Assets._Project.Systems.Tutorial
{
    public class TutorialPopup : MonoBehaviour, IUIElement
    {
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
        }

        public void Show(Action callback = null)
        {
            gameObject.SetActive(true);
            _canvasGroup.DOFade(1, 0.125f).Play().SetAutoKill().OnComplete(() => callback?.Invoke());
        }

        public void Hide(Action callback = null)
        {
            _canvasGroup.DOFade(0, 0.125f).Play().SetAutoKill().OnComplete(() => callback?.Invoke());
            gameObject.SetActive(false);
        }
    }
}
