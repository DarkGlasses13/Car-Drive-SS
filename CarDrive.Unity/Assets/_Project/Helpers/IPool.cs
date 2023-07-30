using UnityEngine;

namespace Assets._Project.Helpers
{
    public interface IPool<T>
    {
        int GetActiveCount();
        T Get();
        T Get(Vector3 spawnPosition);
        void ReleaseAll();
    }
}
