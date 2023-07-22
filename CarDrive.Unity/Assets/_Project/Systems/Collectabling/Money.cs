namespace Assets._Project.Systems.Collectabling
{
    public class Money
    {
        private readonly CollectablingConfig _config;

        public int Value { get; private set; }

        public Money(CollectablingConfig config, int initialAmount = 0)
        {
            _config = config;
            Value = initialAmount;
        }

        public override string ToString()
        {
            return Value > _config.MoneyLimit
                ? _config.MoneyLimit.ToString() + "+"
                : Value.ToString();
        }

        public bool TrySpend(int amount = 1)
        {
            if (Value - amount > 0)
            {
                Value -= amount;
                return true;
            }

            return false;
        }

        public void Add(int amount = 1)
        {
            Value += amount;
        }
    }
}
