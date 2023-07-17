using Assets._Project.Systems.Driving;
using UnityEngine;

namespace Assets._Project.Entities
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterCar : MonoBehaviour, IDrivable
    {
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void RegulateGas(float value)
        {
            _rigidbody.MovePosition(transform.position + Vector3.forward * value);
        }

        public void Stear(float value)
        {
            
        }
    }
}