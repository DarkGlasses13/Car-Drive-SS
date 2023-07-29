using Assets._Project.Systems.Collecting;
using Assets._Project.Systems.Damage;
using Assets._Project.Systems.Driving;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Assets._Project.Entities.Character
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class CharacterCar : Entity, IDrivable, IDamageable, ICanCollectItems
    {
        [SerializeField] private ParticleSystem 
            _explosionParticle,
            _smokeParticle,
            _windParticle;

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
            Vector3 linePosition = new(position, 0.026f, transform.position.z);
            Quaternion rotation = Quaternion.identity;
            transform.SetPositionAndRotation(linePosition, rotation);
        }

        public void ChangeLine(float line, float duration, float stearAngle)
        {
            _moveTween?.Kill();
            _moveTween = transform.DOMoveX(line, duration);
            _moveTween?.Play();
        }

        public void Accelerate(float acceleration)
        {
            if (_windParticle.isPlaying && acceleration < 0.5f)
                _windParticle.Stop();

            if (_windParticle.isPlaying == false && acceleration > 0.5f)
                _windParticle.Play();

            ParticleSystem.MainModule ps = _windParticle.main;
            ps.maxParticles = (int)(acceleration * 50);
            ps.simulationSpeed = acceleration * 5;
            transform.position += transform.forward * acceleration;
        }

        public void OnDie()
        {
            _explosionParticle.Play();
            _smokeParticle.Play();
            _rigidbody.isKinematic = false;
            _moveTween?.Kill();
            _rigidbody.AddForce(Vector3.up * 10000, ForceMode.Impulse);
            transform.DOPunchScale(Vector3.one * 2, 0.25f).Play().SetAutoKill(true);
        }

        public void OnRestore()
        {
            _rigidbody.isKinematic = true;
            _smokeParticle.Stop();
        }
    }
}