using Assets._Project.Architecture;
using Assets._Project.Helpers;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Systems.Sound
{
    public class SoundSystem : GameSystem
    {
        private readonly LocalAssetLoader _assetLoader;
        private readonly Transform _container;
        private readonly AudioListener _listener;
        private Toggle _toggle;

        public SoundSystem(LocalAssetLoader assetLoader, Transform container, AudioListener listener)
        {
            _assetLoader = assetLoader;
            _container = container;
            _listener = listener;
        }

        public override async Task InitializeAsync()
        {
            _toggle = await _assetLoader.LoadAndInstantiateAsync<Toggle>("Sound Toggle", _container);
        }

        public override void OnEnable()
        {
            _toggle.onValueChanged.AddListener(OnSwitched);
        }

        private void OnSwitched(bool isOn)
        {
            AudioListener.volume = isOn ? 1 : 0;
        }

        public override void OnDisable()
        {
            _toggle.onValueChanged.RemoveListener(OnSwitched);
        }
    }
}
