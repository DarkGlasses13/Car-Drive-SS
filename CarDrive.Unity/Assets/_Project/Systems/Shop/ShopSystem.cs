using Assets._Project.Architecture;
using Assets._Project.Systems.Collecting;
using UnityEngine.UI;

namespace Assets._Project.Systems.Shop
{
    public class ShopSystem : GameSystem
    {
        private readonly IInventory _inventory;
        private readonly IItemDatabase _database;
        private readonly Button _buyButton;

        public ShopSystem(IInventory inventory, IItemDatabase database, Button buyButton)
        {
            _inventory = inventory;
            _database = database;
            _buyButton = buyButton;
        }

        public override void Enable()
        {
            _buyButton.onClick.AddListener(OnBuyButtonCkick);
        }

        private void OnBuyButtonCkick()
        {
            if (_database.TryGetByID("it_Lbx", out IItem lootBox))
            {
                _inventory.TryAdd(lootBox);
            }
        }

        public override void Disable()
        {
            _buyButton.onClick.RemoveListener(OnBuyButtonCkick);
        }
    }
}