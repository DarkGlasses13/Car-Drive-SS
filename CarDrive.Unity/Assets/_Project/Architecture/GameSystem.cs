using System.Threading.Tasks;

namespace Assets._Project.Architecture
{
    public abstract class GameSystem : IGameSystem
    {
        public bool Enabled { get; private set; }

        public void Enable()
        {
            Enabled = true;
            OnEnable();
        }
        public virtual void Initialize() { }
        public virtual async Task InitializeAsync() { await Task.CompletedTask; }
        public virtual void OnEnable() { }
        public virtual void FixedTick() { }
        public virtual void Tick() { }
        public virtual void Restart() { }
        public virtual void OnDisable() { }
        public void Disable()
        {
            OnDisable();
            Enabled = false;
        }
    }
}