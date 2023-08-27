using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Systems.Progress
{
    [RequireComponent(typeof(Slider))]
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private TMP_Text _currentLevelNumber, _nextLevelNumber;
        private Slider _slider;

        public float Value { get => _slider.value; set => _slider.value = value; }
        public int CurrentLevel { get 
                => int.Parse(_currentLevelNumber.text); set 
                => _currentLevelNumber.text = value.ToString(); }
        public int NextLevel { get
                => int.Parse(_nextLevelNumber.text); set
                => _nextLevelNumber.text = value.ToString(); }

        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }
    }
}