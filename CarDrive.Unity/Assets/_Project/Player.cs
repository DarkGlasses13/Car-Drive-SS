using Assets._Project.Systems.Collecting;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Project
{
    public class Player
    {
        private readonly Dictionary<ItemType, float> _stats;
        private readonly IItemDatabase _database;

        public int Level { get; set; }
        public bool IsTutorialCompleted { get; set; }
        public int Money { get; set; }
        public IEnumerable<IItem> Equipment { get; set; }
        public IEnumerable<IItem> Items { get; set; }
        public bool IsSound { get; set; }

        public Player(IItemDatabase database)
        {
            _database = database;
            Level = 1;
            IsTutorialCompleted = false;
            Equipment = new List<IItem>();
            Items = new List<IItem>();

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
            foreach (ItemType key in _stats.Keys.ToList())
            {
                _stats[key] = 1;
            }

            Level = 1;
        }

        public PlayerSave GetSave()
        {
            PlayerSave playerSave = new()
            {
                Level = Level,
                IsTutorialCompleted = IsTutorialCompleted,
                Money = Money,
                Equipment = Equipment.Select(equipment => equipment?.ID).ToArray(),
                Items = Items.Select(item => item?.ID).ToArray(),
                Stats = _stats.Values.ToArray(),
                IsSound = IsSound
            };

            return playerSave;
        }

        public void Update(PlayerSave save)
        {
            Level = save.Level;
            IsTutorialCompleted = save.IsTutorialCompleted;
            Money = save.Money;
            Equipment = save.Equipment.Select(id => _database.GetByID(id));
            Items = save.Items.Select(id => _database.GetByID(id));

            int i = 0;
            foreach (ItemType key in _stats.Keys.ToList())
            {
                _stats[key] = save.Stats[i];
                i++;
            }
        }
    }
}
