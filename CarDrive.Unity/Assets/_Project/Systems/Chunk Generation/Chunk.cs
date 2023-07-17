using System;
using UnityEngine;

namespace Assets._Project.Systems.ChunkGeneration
{
    public class Chunk : MonoBehaviour
    {
        public Action<Chunk> OnPassed;
        [SerializeField] private MeshRenderer _roadMeshRenderer;

        public Bounds Bounds => _roadMeshRenderer.bounds;

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
