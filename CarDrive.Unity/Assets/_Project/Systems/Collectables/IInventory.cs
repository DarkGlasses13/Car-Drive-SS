using System;
using System.Collections.Generic;

namespace Assets._Project.Systems.Collecting
{
    public interface IInventory
    {
        event Action OnChenged;
        int SlotsCount { get; }
        int EquipmentCount { get; }
        IEnumerable<IItem> Items { get; }
        IEnumerable<IItem> Equipment { get; }
        bool HasEmptySlots { get; }
        bool TryAdd(IItem item);
        bool TryRemove(int slot);
        void Swap(int from, int to);
        void Swap(int slot, IItem item);
        void Equip(int from, int to);
        void UnEquip(int fromSlotIndex, int toSlotIndex);
        void Clear();
        void UnEquipMerge(int fromSlotIndex, int toSlotIndex, IItem item);
    }
}