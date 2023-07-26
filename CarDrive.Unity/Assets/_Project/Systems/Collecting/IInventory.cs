using System;
using System.Collections.Generic;

namespace Assets._Project.Systems.Collecting
{
    public interface IInventory
    {
        event Action OnChenged;
        int Length { get; }
        IEnumerable<IItem> Items { get; }

        bool TryAdd(IItem item);
        bool TryRemove(int slot);
        bool TrySwap(int from, int to);
        void Swap(int slot, IItem item);
    }
}