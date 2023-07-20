using Assets._Project.Architecture;

namespace Assets._Project
{
    public abstract class RunnerWithAutomaticSystemsInitialization : Runner
    {
        protected override async void Start()
        {
            await CreateSystems();

            foreach (IGameSystem system in _systems)
            {
                await system.InitializeAsync();
                system.Initialize();
                system.Enable();
            }
        }
    }
}
