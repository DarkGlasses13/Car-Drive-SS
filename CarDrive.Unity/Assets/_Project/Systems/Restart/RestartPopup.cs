using Assets._Project.Architecture.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Systems.Restart
{
    public class RestartPopup : MonoBehaviour, IUIElement
    {
        [SerializeField] private AudioSource _failMusic;

        public Button RestartButton { get; private set; }

        private void Awake()
        {
            RestartButton = GetComponentInChildren<Button>();
        }

        public void Open(Action callback = null)
        {
            gameObject.SetActive(true);
            _failMusic.Play();
            callback?.Invoke();
        }

        public void Close(Action callback = null)
        {
            _failMusic.Stop();
            gameObject.SetActive(false);
            callback?.Invoke();
        }
    }
}
