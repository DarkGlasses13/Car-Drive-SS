using Assets._Project.Entities.Obstacles;
using Assets._Project.Systems.Collecting;
using System;
using UnityEngine;

namespace Assets._Project.Systems.ChunkGeneration
{
    public class Chunk : MonoBehaviour
    {
        public Action<Chunk> OnPassed;
        [SerializeField] private MeshRenderer _roadMeshRenderer;
        private ItemObject[] _collectables;
        private Obstacle[] _obstacles;

        public Bounds Bounds => _roadMeshRenderer.bounds;

        private void Awake()
        {
            _collectables = GetComponentsInChildren<ItemObject>();
            _obstacles = GetComponentsInChildren<Obstacle>();
        }

        public void ShowCollectables()
        {
            if (_collectables != null)
            {
                Array.ForEach(_collectables, money => money.gameObject.SetActive(true));
            }
        }

        public void ShowObstacles()
        {
            if (_obstacles != null)
                Array.ForEach(_obstacles, obstacle => obstacle.gameObject.SetActive(true));
        }

        public Vector3 GetConnectPosition(Chunk connecting)
        {
            return transform.position + Vector3.forward * ((Bounds.size.z + connecting.Bounds.size.z) / 2);
        }

        private void OnTriggerExit(Collider other)
        {
            OnPassed?.Invoke(this);
        }
    }
}
