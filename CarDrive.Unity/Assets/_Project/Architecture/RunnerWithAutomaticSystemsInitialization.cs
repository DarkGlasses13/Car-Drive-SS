using Assets._Project.Architecture;
using UnityEngine;

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

            _isInitialized = true;
            OnInitializationCompleted();
        }

        protected abstract void OnInitializationCompleted();
    }
}
