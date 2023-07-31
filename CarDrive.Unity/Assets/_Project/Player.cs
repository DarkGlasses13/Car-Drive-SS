using Assets._Project.Systems.Collecting;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Project
{
    public class Player
    {
        private readonly Dictionary<ItemType, float> _stats;

        public int Level { get; set; }
        public bool IsTutorialCompleted { get; set; }

        public Player()
        {
            _stats = new Dictionary<ItemType, float>()
            {
                { ItemType.Wheel, 1 },
                { ItemType.Brakes, 1 },
                { ItemType.Engine, 1 },
                { ItemType.Accelerator, 1 }
            };
        }

        public void SetStat(ItemType type, float value) => _stats[type] = value;

        public float GetStat(ItemType type) => _stats[type];

        public void ResetSats()
        {
            foreach (var key in _stats.Keys.ToList())
            {
                _stats[key] = 1;
            }
        }
    }
}
