namespace Assets._Project.Systems.MoneyControl
{
    public class Money
    {
        private readonly MoneyControlConfig _config;

        public int Value { get; private set; }

        public Money(MoneyControlConfig config, int initialAmount = 0)
        {
            _config = config;
            Value = initialAmount;
        }

        public override string ToString()
        {
            return Value > _config.Limit 
                ? _config.Limit.ToString() + "+" 
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
