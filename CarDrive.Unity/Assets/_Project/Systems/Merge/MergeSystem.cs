using Assets._Project.Architecture;
using Assets._Project.Systems.CheckPoint;
using Assets._Project.Systems.Collecting;
using System;
using UnityEditor.Graphs;

namespace Assets._Project.Systems.Merge
{
    public class MergeSystem : GameSystem
    {
        private readonly IInventory _inventory;
        private readonly IItemDatabase _database;
        private readonly MergeGrid _grid;
        private readonly CheckPointPopup _popup;

        public MergeSystem(IInventory inventory, IItemDatabase database, MergeGrid grid, CheckPointPopup popup)
        {
            _inventory = inventory;
            _database = database;
            _grid = grid;
            _popup = popup;
        }

        public override void Enable()
        {
            _popup.OnBeforeOpening += OnPopupOpening;
            _inventory.OnChenged += OnInventoryChanged;
            _grid.OnAddNewItem += OnAddNewItem;
            _grid.OnSwap += OnSwap;
            _grid.OnMerge += OnMerge;
        }

        private void OnMerge(int from, int to, IItem mergeResult)
        {
            _inventory.Swap(from, null);
            _inventory.Swap(to, mergeResult);
        }

        private void OnAddNewItem(int slot)
        {
            _inventory.Swap(slot, _database.GetRandom());
        }

        private void OnSwap(int from, int to)
        {
            _inventory.TrySwap(from, to);
        }

        private void OnPopupOpening()
        {
            _grid.UpdateView(_inventory.Items);
        }

        private void OnInventoryChanged()
        {
            _grid.UpdateView(_inventory.Items);
        }

        public override void Disable()
        {
            _popup.OnBeforeOpening -= OnPopupOpening;
            _inventory.OnChenged -= OnInventoryChanged;
            _grid.OnAddNewItem -= OnAddNewItem;
            _grid.OnSwap -= OnSwap;
            _grid.OnMerge -= OnMerge;
        }
    }
}