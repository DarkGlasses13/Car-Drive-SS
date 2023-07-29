using UnityEngine;

namespace Assets._Project.Systems.Damage
{
    public interface IDamageable
    {
        Vector3 Center { get; }
        Quaternion Rotation { get; }
        void OnDie();
        void OnRestore();
    }
}