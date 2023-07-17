using UnityEngine;

namespace Assets._Project.Helpers
{
    public struct SpawnData
    {
        public Transform Parent { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public SpawnData(Transform parent, Vector3 position, Quaternion rotation)
        {
            Parent = parent;
            Position = position;
            Rotation = rotation;
        }
    }
}
