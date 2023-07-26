using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Systems.Collecting
{
    [CreateAssetMenu(menuName = "Item Database")]
    public class ItemDatabase : ScriptableObject, IItemDatabase
    {
        [SerializeField] private ItemReference[] _items;

        public IItem GetRandom()
        {
            IEnumerable<ItemReference> items = _items.Where(itm => itm.Type != ItemType.Money && itm.Type != ItemType.LootBox);
            return items.ElementAt(Random.Range(0, items.Count()));
        }

        public bool TryGetByID(string id, out IItem item)
        {
            item = _items.SingleOrDefault(itm => itm.ID == id);
            return item != null;
        }
    }
}
