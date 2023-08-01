using Assets._Project.Architecture;
using Assets._Project.Architecture.UI;
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
        private readonly Player _player;

        public ShopSystem(IInventory inventory, IItemDatabase database,
            PriceTagButton buyButton, Money money, CollectablesConfig config,
            UICounter lootBoxPrice, Player player)
        {
            _inventory = inventory;
            _database = database;
            _buyButton = buyButton;
            _money = money;
            _config = config;
            _player = player;
            lootBoxPrice.Set(config.LootBoxPrice.ToString());
        }

        public override void Enable()
        {
            _buyButton.Button.onClick.AddListener(OnBuyButtonCkick);
        }

        private void OnBuyButtonCkick()
        {
            if (_inventory.HasEmptySlots)
            {
                if (_money.TrySpend(_config.LootBoxPrice))
                {
                    _buyButton.OnDeal();
                    _player.Money = _money.Value;
                    _inventory.TryAdd(_database.GetByID("it_Lbx"));
                    return;
                }
            }

            _buyButton.OnFail();
        }

        public override void Disable()
        {
            _buyButton.Button.onClick.RemoveListener(OnBuyButtonCkick);
        }
    }
}