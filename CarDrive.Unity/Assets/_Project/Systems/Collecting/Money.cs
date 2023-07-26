using System;

namespace Assets._Project.Systems.Collecting
{
    public class Money
    {
        private readonly CollectablesConfig _config;

        public event Action<int> OnChanged;
        public int Value { get; private set; }

        public Money(CollectablesConfig config, int initialAmount = 0)
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
            if (Value - amount >= 0)
            {
                Value -= amount;
                OnChanged?.Invoke(Value);
                return true;
            }

            return false;
        }

        public void Add(int amount = 1)
        {
            Value += amount;
            OnChanged?.Invoke(Value);
        }
    }
}
