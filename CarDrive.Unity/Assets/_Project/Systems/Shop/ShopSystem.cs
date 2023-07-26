using Assets._Project.Architecture;
using Assets._Project.Systems.Collecting;

namespace Assets._Project.Systems.Shop
{
    public class ShopSystem : GameSystem
    {
        private readonly IInventory _inventory;
        private readonly IItemDatabase _database;
        private readonly PriceTagButton _buyButton;
        private readonly Money _money;
        private readonly CollectablesConfig _config;

        public ShopSystem(IInventory inventory, IItemDatabase database,
            PriceTagButton buyButton, Money money, CollectablesConfig config)
        {
            _inventory = inventory;
            _database = database;
            _buyButton = buyButton;
            _money = money;
            _config = config;
            _buyButton.Price = config.LootBoxPrice.ToString();
        }

        public override void Enable()
        {
            _buyButton.Button.onClick.AddListener(OnBuyButtonCkick);
        }

        private void OnBuyButtonCkick()
        {
            if (_database.TryGetByID("it_Lbx", out IItem lootBox))
            {
                if (_money.TrySpend(_config.LootBoxPrice))
                {
                    _inventory.TryAdd(lootBox);
                }
            }
        }

        public override void Disable()
        {
            _buyButton.Button.onClick.RemoveListener(OnBuyButtonCkick);
        }
    }
}