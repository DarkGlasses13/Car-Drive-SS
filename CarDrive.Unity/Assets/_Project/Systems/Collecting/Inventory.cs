using System;

namespace Assets._Project.Systems.Collecting
{
    public class Inventory : IInventory
    {
        private IItem[] _items;

        public int Capacity { get; }
        public int Length => _items.Length;

        public Inventory(int capacity)
        {
            Capacity = capacity;
            _items = new IItem[Capacity];
        }

        public bool TryAdd(IItem item)
        {
            if (_items.Length < Capacity)
            {
                _items[Array.IndexOf(_items, null)] = item;
                return true;
            }

            return false;
        }

        public bool TrySwap(int from, int to)
        {
            if (_items[from] == null)
                return false;

            IItem fromItem = _items[from];
            IItem toItem = _items[to];
            _items[from] = toItem;
            _items[to] = fromItem;
            return true;
        }

        public bool TryRemove(int slot)
        {
            if (_items[slot] != null)
            {
                _items[slot] = null;
                return true;
            }

            return false;
        }
    }
}