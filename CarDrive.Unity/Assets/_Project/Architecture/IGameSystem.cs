using System.Threading.Tasks;

namespace Assets._Project.Architecture
{
    public interface IGameSystem : IRestartable
    {
        bool Enabled { get; }
        void Initialize();
        Task InitializeAsync();
        void Enable();
        void OnEnable();
        void Tick();
        void FixedTick();
        void Disable();
        void OnDisable();
    }
}