using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Project.Systems.Collecting
{
    public class Inventory : IInventory
    {
        public event Action OnChenged;

        private readonly IItem[] _items, _equipment;

        public int SlotsCount => _items.Length;
        public int EquipmentCount => _equipment.Length;

        public IEnumerable<IItem> Items => _items.AsEnumerable();
        public IEnumerable<IItem> Equipment => _equipment.AsEnumerable();

        public Inventory(int capacity, int equipmentCapacity)
        {
            _items = new IItem[capacity];
            _equipment = new IItem[equipmentCapacity];
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

        public void Swap(int from, int to)
        {
            IItem fromItem = _items[from];
            IItem toItem = _items[to];
            _items[from] = toItem;
            _items[to] = fromItem;
            OnChenged?.Invoke();
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

        public void Equip(int from, int to)
        {
            IItem fromItem = _items[from];
            IItem equipment = _equipment[to];
            _equipment[to] = fromItem;
            _items[from] = equipment;
            OnChenged?.Invoke();
        }

        public void UnEquip(int from, int to)
        {
            IItem equipment = _equipment[from];
            IItem toItem = _items[to];
            _items[to] = equipment;
            _equipment[from] = toItem;
            OnChenged?.Invoke();
        }
    }
}