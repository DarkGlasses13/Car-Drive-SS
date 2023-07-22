using Assets._Project.Architecture;
using Assets._Project.Architecture.UI;
using UnityEngine;

namespace Assets._Project.Systems.MoneyControl
{
    public class LevelMoneyCollectingSystem : GameSystem
    {
        private readonly MoneyControlConfig _config;
        private readonly Money _money;
        private readonly IMoneyCollector _collector;
        private readonly IUICounter _uiCounter;
        private float _collectingRadius = 2;
        private Collider[] _detecables = new Collider[50];

        public LevelMoneyCollectingSystem(MoneyControlConfig config, Money money, IMoneyCollector collector, IUICounter uICounter)
        {
            _config = config;
            _money = money;
            _collector = collector;
            _uiCounter = uICounter;
        }

        public override void Initialize()
        {
            _uiCounter.Set(_money.ToString());
        }

        public override void FixedTick()
        {
            int detectablesCount = Physics.OverlapSphereNonAlloc(_collector.Center,
                _collectingRadius, _detecables, _config.LayerMask, QueryTriggerInteraction.Collide);

            for (int i = 0; i < detectablesCount; i++)
            {
                _detecables[i].gameObject.SetActive(false);
                _money.Add();
                _uiCounter.Set(_money.ToString());
            }
        }
    }
}