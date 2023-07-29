using System.Threading.Tasks;

namespace Assets._Project.Architecture
{
    public interface IGameSystem : IRestartable
    {
        void Initialize();
        Task InitializeAsync();
        void Enable();
        void Tick();
        void FixedTick();
        void Disable();
    }
}