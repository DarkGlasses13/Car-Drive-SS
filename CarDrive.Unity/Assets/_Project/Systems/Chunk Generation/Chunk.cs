using Assets._Project.Entities.Obstacles;
using Assets._Project.Systems.Collecting;
using System;
using UnityEngine;

namespace Assets._Project.Systems.ChunkGeneration
{
    public class Chunk : MonoBehaviour
    {
        public Action OnSpawned;
        public Action<Chunk> OnPassed;
        [SerializeField] private MeshRenderer _roadMeshRenderer;
        private ItemObject[] _collectables;
        private Obstacle[] _obstacles;

        public Bounds Bounds => _roadMeshRenderer.bounds;

        public void Init()
        {
            _collectables = GetComponentsInChildren<ItemObject>();
            _obstacles = GetComponentsInChildren<Obstacle>();
        }

        public void ShowCollectables()
        {
            if (_collectables != null)
                for (int i = 0; i < _collectables.Length; i++)
                    _collectables[i].gameObject.SetActive(true);
        }

        public void ShowObstacles()
        {
            if (_obstacles != null)
                for (int i = 0; i < _obstacles.Length; i++)
                    _obstacles[i].gameObject.SetActive(true);
        }

        public Vector3 GetConnectPosition(Chunk connecting)
        {
            return transform.position + Vector3.forward * ((Bounds.size.z + connecting.Bounds.size.z) / 2);
        }

        private void OnTriggerExit(Collider other)
        {
            OnPassed?.Invoke(this);
        }

        public void HideAll()
        {
            if (_collectables != null)
                for (int i = 0; i < _collectables.Length; i++)
                    _collectables[i].gameObject.SetActive(false);

            if (_obstacles != null)
                for (int i = 0; i < _obstacles.Length; i++)
                    _obstacles[i].gameObject.SetActive(false);
        }

        public void InvokeOnSpawn()
        {
            OnSpawned?.Invoke();
        }
    }
}
