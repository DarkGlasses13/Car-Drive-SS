using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Systems.ChunkGeneration
{
    public class Chunk : MonoBehaviour
    {
        public Action<Chunk> OnPassed;
        [SerializeField] private MeshRenderer _roadMeshRenderer;
        [SerializeField] private bool _isContainsCollectables, _isContainsObstacles;
        [SerializeField, ShowIf("_isContainsCollectables")] private GameObject[] _collectables;
        [SerializeField, ShowIf("_isContainsObstacles")] private GameObject[] _obstacles;

        public Bounds Bounds => _roadMeshRenderer.bounds;
        public IEnumerable<GameObject> Obstacles => _obstacles.AsEnumerable();

        private void OnEnable()
        {
            if (_isContainsCollectables)
                Array.ForEach(_collectables, money => money.gameObject.SetActive(false));

            if (_isContainsObstacles)
                Array.ForEach(_obstacles, obstacle => obstacle.gameObject.SetActive(false));
        }

        public void ShowCollectables()
        {
            if (_isContainsCollectables)
            {
                Array.ForEach(_collectables, money => money.gameObject.SetActive(true));
            }
        }

        public void ShowObstacles()
        {
            if (_isContainsObstacles)
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
