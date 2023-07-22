using Assets._Project.Architecture;
using Assets._Project.Architecture.UI;
using UnityEngine;

namespace Assets._Project.Systems.Collectabling
{
    public class CollectingSystem : GameSystem
    {
        private readonly CollectablingConfig _config;
        private readonly Money _money;
        private readonly ICanCollectItems _collector;
        private readonly IUICounter _uiCounter;
        private float _collectingRadius = 2;
        private Collider[] _detecables = new Collider[50];

        public CollectingSystem(CollectablingConfig config, Money money, ICanCollectItems collector, IUICounter uICounter)
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
                if (_detecables[i].TryGetComponent(out ItemObject item))
                {
                    item.gameObject.SetActive(false);

                    if (item.ID == _config.MoneyID)
                    {
                        _money.Add();
                        _uiCounter.Set(_money.ToString());
                        continue;
                    }

                    
                }
            }
        }
    }
}