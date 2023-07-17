using System.Threading.Tasks;

namespace Assets._Project.Architecture
{
    public abstract class GameSystem : IGameSystem
    {
        public virtual void Initialize() { }
        public virtual async Task InitializeAsync() { await Task.Yield(); }
        public virtual void FixedTick() { }
        public virtual void Tick() { }
    }
}