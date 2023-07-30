using Assets._Project.Architecture.UI;
using System;
using UnityEngine;

namespace Assets._Project.Systems.CheckPoint
{
    public class CheckPointPopup : MonoBehaviour, IUIElement
    {
        public event Action OnBeforeOpening;
        [SerializeField] private AudioSource 
            _music,
            _equipSound,
            _mergeSound;

        [field: SerializeField] public RectTransform BalanceAndPlayButtonSection { get; private set; }
        [field: SerializeField] public RectTransform EquipmentSection { get; private set; }
        [field: SerializeField] public RectTransform MergeAndBuyButtonSection { get; private set; }

        public void Open(Action callback = null)
        {
            OnBeforeOpening?.Invoke();
            gameObject.SetActive(true);
            _music.Play();
            callback?.Invoke();
        }

        public void Close(Action callback = null)
        {
            gameObject.SetActive(false);
            _music.Stop();
            callback?.Invoke();
        }

        public void PlayEquipSound()
        {
            _equipSound.Play();
        }

        public void PlayMergeSound()
        {
            _mergeSound.Play();
        }
    }
}
