using UnityEngine;

namespace Assets._Project.Systems.Collecting
{
    [CreateAssetMenu(menuName = "Config/Money Control")]
    public class CollectablingConfig : ScriptableObject
    {
        [field: SerializeField] public int MoneyLimit { get; private set; }
        [field: SerializeField] public LayerMask LayerMask { get; private set; }
    }
}