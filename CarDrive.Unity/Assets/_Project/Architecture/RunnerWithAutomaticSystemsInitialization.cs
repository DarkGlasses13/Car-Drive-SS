using UnityEngine;

namespace Assets._Project.Architecture
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
                Debug.Log(system + "was initialized");
                system.Enable();
            }

            _isInitialized = true;
            OnInitializationCompleted();
        }

        protected abstract void OnInitializationCompleted();
    }
}
