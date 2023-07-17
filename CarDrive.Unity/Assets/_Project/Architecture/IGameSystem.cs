namespace Assets._Project.Architecture
{
    public interface IGameSystem
    {
        void Initialize();
        void Tick();
        void FixedTick();
    }
}