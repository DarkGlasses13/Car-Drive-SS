using System.Linq;
using UnityEngine;

namespace Assets._Project.Systems.Collecting
{
    [CreateAssetMenu(menuName = "Item Database")]
    public class ItemDatabase : ScriptableObject, IItemDatabase
    {
        [SerializeField] private ItemReference[] _items;

        public bool TryGetByID(string id, out IItem item)
        {
            item = _items.SingleOrDefault(itm => itm.ID == id);
            return item != null;
        }
    }
}
