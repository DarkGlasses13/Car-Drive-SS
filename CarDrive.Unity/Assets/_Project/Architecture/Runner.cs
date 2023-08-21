using Assets._Project.Architecture;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project
{
    public abstract class Runner : MonoBehaviour, IRestartable
    {
        protected bool _isInitialized = false;
        protected List<IGameSystem> _systems;

        protected abstract Task CreateSystems();

        protected async virtual void Start()
        {
            await CreateSystems();
            _isInitialized = true;
        }

        private void Update()
        {
            if (_isInitialized)
                _systems?.ForEach(system => 
                {
                    if (system.Enabled)
                        system?.Tick();
                });
        }

        private void FixedUpdate()
        {
            if (_isInitialized)
                _systems?.ForEach(system =>
                {
                    if (system.Enabled)
                        system?.FixedTick();
                });
        }

        private void OnDisable()
        {
            if (_isInitialized)
                _systems?.ForEach(system => 
                {
                    if (system.Enabled) 
                        system?.OnDisable(); 
                });
        }

        public void Restart()
        {
            _systems.ForEach(system => 
            {
                if (system.Enabled) 
                    system?.Restart(); 
            });
        }
    }
}
