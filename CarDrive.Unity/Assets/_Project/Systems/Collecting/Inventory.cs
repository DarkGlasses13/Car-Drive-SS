using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Project.Systems.Collecting
{
    public class Inventory : IInventory
    {
        public event Action OnChenged;

        private IItem[] _items;

        public int Length => _items.Length;

        public IEnumerable<IItem> Items => _items.AsEnumerable();

        public Inventory(int capacity)
        {
            _items = new IItem[capacity];
        }

        public bool TryAdd(IItem item)
        {
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] == null)
                {
                    _items[i] = item;
                    OnChenged?.Invoke();
                    return true;
                }
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
            OnChenged?.Invoke();
            return true;
        }

        public void Swap(int slot, IItem item) 
        {
            _items[slot] = item;
            OnChenged?.Invoke();
        }

        public bool TryRemove(int slot)
        {
            if (_items[slot] != null)
            {
                _items[slot] = null;
                OnChenged?.Invoke();
                return true;
            }

            return false;
        }
    }
}