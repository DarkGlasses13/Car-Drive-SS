using Assets._Project.Systems.Damage;
using Assets._Project.Systems.Driving;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Assets._Project.Entities.Character
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterCar : Entity, IDrivable, IDamageable
    {
        //private Rigidbody _rigidbody;
        private TweenerCore<Vector3, Vector3, VectorOptions> _moveTween;
        private TweenerCore<Quaternion, Vector3, QuaternionOptions> _rotationTween;
        private Sequence _changeLineSequence;

        public Vector3 Center => transform.position;
        public Quaternion Rotation => transform.rotation;

        //private void Awake()
        //{
        //    _rigidbody = GetComponent<Rigidbody>();
        //}

        public void ChangeLine(float shift, float duration, float stearAngle)
        {
            //_changeLineSequence = new Sequence();
            _moveTween?.Kill();
            _moveTween = transform.DOMoveX(transform.position.x + shift, duration);
            //_rotationTween = transform.DORotate(shift * stearAngle * Vector3.up, duration / 2).SetLoops(2, LoopType.Yoyo);
            _moveTween?.Play();
        }

        public void Accelerate(float acceleration)
        {
            transform.Translate(transform.forward * acceleration);
            //_rigidbody.MovePosition(_rigidbody.position + transform.forward * acceleration);
        }

        public void Die()
        {
            _moveTween?.Kill();
            //_rotationTween?.Kill();
            transform.DOPunchScale(Vector3.one * 2, 0.25f).Play().SetAutoKill(true);
        }
    }
}