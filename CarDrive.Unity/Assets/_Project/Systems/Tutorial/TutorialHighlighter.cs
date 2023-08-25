using Assets._Project.Architecture.UI;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Systems.Tutorial
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TutorialHighlighter : MonoBehaviour, IUIElement
    {
        [SerializeField] private Image _highlightArea;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Show(Action callback = null)
        {
            gameObject.SetActive(true);
            _canvasGroup.DOFade(1, 0.05f).Play().SetAutoKill().OnComplete(() => callback?.Invoke());
        }

        public void Hide(Action callback = null)
        {
            _canvasGroup.DOFade(0, 0.05f).Play().SetAutoKill().OnComplete(() => callback?.Invoke());
            gameObject.SetActive(false);
        }

        public void Highlight(Vector2 position, Vector2 size)
        {
            _highlightArea.rectTransform.sizeDelta = size;
            _highlightArea.rectTransform.position = position;
        }
    }
}