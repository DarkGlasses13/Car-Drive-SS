using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Architecture.UI
{
    public class LoadingScreen : FadeScreen
    {
        [SerializeField] private Slider _slider;

        public void SetProgress(float progress)
        {
            if (_slider == null)
                return;

            _slider.value = progress;
        }
    }
}
