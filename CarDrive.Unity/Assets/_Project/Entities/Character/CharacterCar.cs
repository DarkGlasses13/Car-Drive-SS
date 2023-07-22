using Assets._Project.Systems.Collectabling;
using Assets._Project.Systems.Damage;
using Assets._Project.Systems.Driving;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Assets._Project.Entities.Character
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterCar : Entity, IDrivable, IDamageable, ICanCollectItems
    {
        private Rigidbody _rigidbody;
        private TweenerCore<Vector3, Vector3, VectorOptions> _moveTween;

        public Vector3 Center => transform.position;
        public Quaternion Rotation => transform.rotation;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void SetToLine(float position)
        {
            transform.position = new(position, transform.position.y, transform.position.z);
        }

        public void ChangeLine(float line, float duration, float stearAngle)
        {
            _moveTween?.Kill();
            _moveTween = transform.DOMoveX(line, duration);
            _moveTween?.Play();
        }

        public void Accelerate(float acceleration)
        {
            transform.position += transform.forward * acceleration;
        }

        public void Die()
        {
            _rigidbody.isKinematic = false;
            _moveTween?.Kill();
            transform.DOPunchScale(Vector3.one * 2, 0.25f).Play().SetAutoKill(true);
        }
    }
}