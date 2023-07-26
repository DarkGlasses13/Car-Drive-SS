using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Systems.Shop
{
    public class PriceTagButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text _price;
        public Button Button { get; private set; }
        public string Price { get => _price.text; set => _price.text = value; }

        private void Awake()
        {
            Button = GetComponent<Button>();
        }
    }
}