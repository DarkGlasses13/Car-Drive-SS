using Assets._Project.Systems.Driving;
using UnityEngine;

namespace Assets._Project.Entities.Character
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterCar : Entity, IDrivable
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