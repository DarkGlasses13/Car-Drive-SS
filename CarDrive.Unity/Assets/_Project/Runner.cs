using Assets._Project.Architecture;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project
{
    public abstract class Runner : MonoBehaviour
    {
        protected List<IGameSystem> _systems;

        private void Update()
        {
            _systems?.ForEach(system => system?.Tick());
        }

        private void FixedUpdate()
        {
            _systems?.ForEach(system => system?.FixedTick());
        }
    }
}
