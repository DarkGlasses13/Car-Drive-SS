using UnityEngine;

namespace Assets._Project.Systems.Collectabling
{
    [CreateAssetMenu(menuName = "Config/Money Control")]
    public class CollectablingConfig : ScriptableObject
    {
        [SerializeField] ItemReference _moneyReference;
        [field: SerializeField] public int MoneyLimit { get; private set; }
        [field: SerializeField] public LayerMask LayerMask { get; private set; }
        public string MoneyID => _moneyReference.ID;
    }
}