using Assets._Project.Architecture;
using Assets._Project.Systems.CheckPoint;

namespace Assets._Project.Systems.Collecting
{
    public class InventorySystem : GameSystem
    {
        private readonly IInventory _inventory;
        private readonly IItemDatabase _database;
        private readonly UIInventory _uiInventory;
        private readonly CheckPointPopup _popup;
        private readonly Player _player;

        public InventorySystem(IInventory inventory, IItemDatabase database, UIInventory grid, CheckPointPopup popup, Player player)
        {
            _inventory = inventory;
            _database = database;
            _uiInventory = grid;
            _popup = popup;
            _player = player;
        }

        public override void Enable()
        {
            _popup.OnBeforeOpening += OnPopupOpening;
            _inventory.OnChenged += OnInventoryChanged;
            _uiInventory.OnLootBoxOpened += OnLootBoxOpened;
            _uiInventory.OnSwap += OnSwap;
        }

        private void OnLootBoxOpened(int slot)
        {
            _inventory.Swap(slot, _database.GetRandom(1));
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
                        IItem mergeResult = _database.GetByMergeLevel(type, mergeLevel + 1);

                        if (mergeLevel > 0)
                        {
                            _inventory.UnEquipMerge(fromSlotIndex, toSlotIndex, mergeResult);
                            _popup.PlayMergeSound();
                            _popup.EmitMergeParticle(to);
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
                    }
                    return;
                }
            }

            _inventory.Swap(fromSlotIndex, toSlotIndex);
        }

        private void OnPopupOpening()
        {
            _uiInventory.UpdateView(_inventory.Items, _inventory.Equipment);
        }

        private void OnInventoryChanged()
        {
            _uiInventory.UpdateView(_inventory.Items, _inventory.Equipment);
        }

        public override void Disable()
        {
            _popup.OnBeforeOpening -= OnPopupOpening;
            _inventory.OnChenged -= OnInventoryChanged;
            _uiInventory.OnLootBoxOpened -= OnLootBoxOpened;
            _uiInventory.OnSwap -= OnSwap;
        }
    }
}