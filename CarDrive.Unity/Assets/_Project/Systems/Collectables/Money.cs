using System;
using UnityEngine;

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
            string formatedValue = "";

            if (Value > 1000000)
                formatedValue += Math.Round((decimal)Value / 1000000, 1) + "M";

            if (Value < 1000000)
                formatedValue += (Value % 1000 >= 100 
                    ? Math.Round((decimal)Value / 1000, 1) 
                    : Mathf.RoundToInt(Value / 1000)) + "k";

            if (Value < 1000000 && Value < 1000)
                formatedValue = Value.ToString();

            //float m = Value / 1000000;
            //int k = Value % 1000000 / 1000;
            //int u = Value % 1000;
            //formatedValue += m > 0 ? m + "M" : "";
            //formatedValue += k > 0 ? "." + k + "k" : "";
            //formatedValue += u > 0 ? "." + u : "";

            return Value > _config.MoneyLimit
                ? "OVER"
                : formatedValue;
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

        public void SpendAll() => TrySpend(Value);
    }
}
