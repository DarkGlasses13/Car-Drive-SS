using Assets._Project.Architecture;
using Assets._Project.Systems.CheckPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Systems.Collecting
{
    public class InventorySystem : GameSystem
    {
        public event Action<int> OnMerged, OnEquiped;
        public event Action<int, int> OnSwaped;

        private readonly IInventory _inventory;
        private readonly IItemDatabase _database;
        private readonly UIInventory _uiInventory;
        private readonly CheckPointPopup _popup;
        private readonly Player _player;

        public InventorySystem(IInventory inventory, IItemDatabase database,
            UIInventory grid, CheckPointPopup popup, Player player)
        {
            _inventory = inventory;
            _database = database;
            _uiInventory = grid;
            _popup = popup;
            _player = player;
        }

        public override void OnEnable()
        {
            _popup.OnBeforeOpening += OnPopupOpening;
            _inventory.OnChenged += OnInventoryChanged;
            _uiInventory.OnLootBoxOpened += OnLootBoxOpened;
            _uiInventory.OnSwap += OnSwap;
        }

        public void Add(params string[] itemIDs)
        {
            foreach (string id in itemIDs)
            {
                _inventory.TryAdd(_database.GetByID(id));
            }
        }

        private void OnLootBoxOpened(int slot)
        {
            int mergeLevel = 1;
            int minEquipmentMergeLevel = 1;
            int minItemsMergeLevel = 1;
            IEnumerable<IItem> equipment = _inventory.Equipment.Where(item => item != null);
            IEnumerable<IItem> items = _inventory.Items.Where(item => item != null);

            if (equipment.Count() > 0)
                minEquipmentMergeLevel = equipment.Min(item => item.MergeLevel);

            if (items.Count() > 0)
                minItemsMergeLevel = items.Min(item => item.MergeLevel);

            mergeLevel = Mathf.Min(minEquipmentMergeLevel, minItemsMergeLevel);
            _inventory.Swap(slot, _database.GetRandom(mergeLevel));
        }

        private void OnSwap(UISlot from, UISlot to)
        {
            if (from == null)
                return;

            if (to == null)
                return;

            if (from == to)
                return;

            if (from.Item == null)
                return;

            int fromSlotIndex = from.transform.GetSiblingIndex();
            int toSlotIndex = to.transform.GetSiblingIndex();

            if (to.IsEquipment)
            {
                if (from.Item.Type == to.Type)
                {
                    if (to.Item != null && from.Item.ID == to.Item.ID)
                        return;

                    _popup.PlayEquipSound();
                    _inventory.Equip(fromSlotIndex, toSlotIndex);
                    _player.SetStat(to.Type, to.Item.Stat);
                    OnEquiped?.Invoke(toSlotIndex);
                }

                return;
            }

            if (from.IsEquipment)
            {
                if (to.Item == null || to.Item.Type == from.Type)
                {
                    if (to.Item != null)
                    {
                        ItemType type = to.Item.Type;
                        int mergeLevel = to.Item.MergeLevel;

                        if (mergeLevel == from.Item.MergeLevel)
                        {
                            IItem mergeResult = _database.GetByMergeLevel(type, mergeLevel + 1);

                            if (mergeResult != null)
                            {
                                _inventory.UnEquipMerge(fromSlotIndex, toSlotIndex, mergeResult);
                                _popup.PlayMergeSound();
                                _popup.EmitMergeParticle(to);
                            }
                            return;
                        }
                    }

                    _inventory.UnEquip(fromSlotIndex, toSlotIndex);
                    _player.SetStat(from.Type, 1);
                }

                return;
            }

            if (to.Item != null && from.Item.ID == to.Item.ID)
            {
                ItemType type = to.Item.Type;
                int mergeLevel = to.Item.MergeLevel;

                if (mergeLevel > 0)
                {
                    IItem mergeResult = _database.GetByMergeLevel(type, mergeLevel + 1);

                    if (mergeResult != null)
                    {
                        _inventory.Swap(fromSlotIndex, null);
                        _inventory.Swap(toSlotIndex, mergeResult);
                        _popup.PlayMergeSound();
                        _popup.EmitMergeParticle(to);
                        OnMerged?.Invoke(toSlotIndex);
                    }
                    return;
                }
            }

            _inventory.Swap(fromSlotIndex, toSlotIndex);
            OnSwaped?.Invoke(fromSlotIndex, toSlotIndex);
        }

        private void OnPopupOpening()
        {
            _uiInventory.UpdateView(_inventory.Items, _inventory.Equipment);
        }

        private void OnInventoryChanged()
        {
            _uiInventory.UpdateView(_inventory.Items, _inventory.Equipment);
            _player.Equipment = _inventory.Equipment;
            _player.Items = _inventory.Items;
        }

        public override void OnDisable()
        {
            _popup.OnBeforeOpening -= OnPopupOpening;
            _inventory.OnChenged -= OnInventoryChanged;
            _uiInventory.OnLootBoxOpened -= OnLootBoxOpened;
            _uiInventory.OnSwap -= OnSwap;
        }

        public void Clear()
        {
            _inventory?.Clear();
        }
    }
}