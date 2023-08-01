using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Architecture.UI
{
    public class FadeScreen : MonoBehaviour
    {
        [SerializeField] protected CanvasGroup _canvasGroup;
        [SerializeField, Range(0, 5)] private float _fadeInDuration, _fadeOutDuration;
        private TweenerCore<float, float, FloatOptions> _fadeTween;

        [field: SerializeField] public Camera Camera { get; private set; }

        public void FadeIn(Action callback = null, bool withoutFade = false) => StartCoroutine(Fade(0, 1, withoutFade ? 0 : _fadeInDuration, callback));

        public void FadeOut(Action callback = null) => StartCoroutine(Fade(1, 0, _fadeOutDuration, callback));

        private IEnumerator Fade(float startValue, float endValue, float duration = 0, Action callback = null)
        {
            _fadeTween?.Kill();
            _canvasGroup.alpha = startValue;
            _fadeTween = _canvasGroup.DOFade(endValue, duration).Play();
            yield return _fadeTween.WaitForCompletion();
            callback?.Invoke();
        }
    }
}
