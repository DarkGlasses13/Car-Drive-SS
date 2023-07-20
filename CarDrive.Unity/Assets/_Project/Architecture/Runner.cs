using Assets._Project.Architecture;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project
{
    public abstract class Runner : MonoBehaviour
    {
        protected List<IGameSystem> _systems;

        protected abstract Task CreateSystems();

        protected async virtual void Start()
        {
            await CreateSystems();
        }

        private void Update()
        {
            _systems?.ForEach(system => system.Tick());
        }

        private void FixedUpdate()
        {
            _systems?.ForEach(system => system.FixedTick());
        }

        private void OnDisable()
        {
            _systems?.ForEach(system => system.Disable());
        }
    }
}
