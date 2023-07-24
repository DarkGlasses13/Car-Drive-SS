using UnityEngine;

namespace Assets._Project.Systems.Collecting
{
    public class ItemObject : MonoBehaviour
    {
        [SerializeField] private ItemReference _reference;

        public string ID => _reference.ID;
        public ItemType Type => _reference.Type;
    }
}