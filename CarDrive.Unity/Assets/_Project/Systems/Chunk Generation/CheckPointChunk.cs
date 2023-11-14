using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Systems.ChunkGeneration
{
    public class CheckPointChunk : Chunk
    {
        [SerializeField] private List<GameObject>
            _townViews,
            _japanViews,
            _beachViews,
            _snowViews;

        public bool IsTriggered { get; private set; } = false;

        public event Action<CheckPointChunk> OnEnter;

        private void OnTriggerEnter(Collider other)
        {
            IsTriggered = true;
            OnEnter?.Invoke(this);
        }

        protected override void OnTriggerExit(Collider other)
        {
            base.OnTriggerExit(other);
            IsTriggered = false;
        }

        [Button]
        public void SetEnvironment() => SetEnvironment(EnvironmentType);

        public void SetEnvironment(ChunkEnvironmentType type)
        {
            if (type == ChunkEnvironmentType.None)
                return;

            _townViews.ForEach(view => view.SetActive(false));
            _japanViews.ForEach(view => view.SetActive(false));
            _beachViews.ForEach(view => view.SetActive(false));
            _snowViews.ForEach(view => view.SetActive(false));

            switch (type)
            {
                case ChunkEnvironmentType.City:
                _townViews.ForEach(view => view.SetActive(true));
                break;
                case ChunkEnvironmentType.Japan:
                _japanViews.ForEach(view => view.SetActive(true));
                break;
                case ChunkEnvironmentType.Snow:
                _snowViews.ForEach(view => view.SetActive(true));
                break;
                case ChunkEnvironmentType.Beach:
                _beachViews.ForEach(view => view.SetActive(true));
                break;
            }
        }
    }
}
