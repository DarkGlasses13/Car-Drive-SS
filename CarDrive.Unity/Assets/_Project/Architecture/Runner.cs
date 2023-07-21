using Assets._Project.Architecture;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project
{
    public abstract class Runner : MonoBehaviour
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
                _systems?.ForEach(system => system.Tick());
        }

        private void FixedUpdate()
        {
            if (_isInitialized)
                _systems?.ForEach(system => system.FixedTick());
        }

        private void OnDisable()
        {
            if (_isInitialized)
                _systems?.ForEach(system => system.Disable());
        }
    }
}
