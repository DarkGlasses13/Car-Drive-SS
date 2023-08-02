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

        public bool HasEmptySlots => _items.Contains(null);

        public Inventory(int capacity, int equipmentCapacity, IEnumerable<IItem> equipment, IEnumerable<IItem> items)
        {
            _items = new IItem[capacity];
            _equipment = new IItem[equipmentCapacity];

            for (int i = 0; i < equipment.Count(); i++)
                _equipment[i] = equipment.ElementAt(i);

            for (int i = 0; i < items.Count(); i++)
                _items[i] = items.ElementAt(i);
        }

        public bool TryAdd(IItem item)
        {
            if (_items.Contains(null) == false)
                return false;

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

        public void UnEquipMerge(int from, int to, IItem mergeResult)
        {
            _equipment[from] = null;
            _items[to] = mergeResult;
            OnChenged?.Invoke();
        }

        public void Clear()
        {
            for (int i = 0; i < _equipment.Length; i++)
            {
                _equipment[i] = null;
            }

            for (int i = 0; i < _items.Length; i++)
            {
                _items[i] = null;
            }

            OnChenged?.Invoke();
        }

        public void Add(IItem[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                TryAdd(items[i]);
            }
        }
    }
}