using UnityEngine;

namespace Assets._Project.Systems.MoneyControl
{
    [CreateAssetMenu(menuName = "Config/Money Control")]
    public class MoneyControlConfig : ScriptableObject
    {
        [field: SerializeField] public int Limit { get; private set; }
        [field: SerializeField] public LayerMask LayerMask { get; private set; }
    }
}