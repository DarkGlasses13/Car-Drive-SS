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
            _windParticle,
            _leftFire,
            _rightFire,
            _impregnabilityAura;

        [SerializeField] private TrailRenderer 
            _leftWheelTrailRenderer,
            _rightWheelTrailRenderer;

        [SerializeField] private AudioSource
            _engineSound,
            _crashSound,
            _repairSound,
            _breakSound,
            _collectSound;

        private Rigidbody _rigidbody;
        private TweenerCore<Vector3, Vector3, VectorOptions> _moveTween;
        private Tweener _rotationTween;
        private float _stearLerp = 0;

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
            _rotationTween?.Kill();
            transform.rotation = Quaternion.identity;
            Break();
            _moveTween = transform.DOMoveX(line, duration).OnComplete(() => EndBreak());
            _rotationTween = transform.DOPunchRotation(Vector3.up * stearAngle, duration);
            _moveTween?.Play();
            _rotationTween?.Play();
        }

        public void Stear(float clampedValue, float speed, float stearAngle, Vector2 roadWidth)
        {
            //_stear = Mathf.Lerp(_stear, clampedValue * speed, _stearLerp);
            transform.position += clampedValue * speed * Time.deltaTime * Vector3.right;
            transform.Rotate(clampedValue * (speed * 5) * Time.deltaTime * Vector3.up);
            //_stearLerp += 0.1f * Time.deltaTime;

            //if (_stearLerp >= 1)
            //    _stearLerp = 0;
        }

        public void EndStear()
        {
            _stearLerp = 0;
        }

        public void ResetStear()
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, _stearLerp);
            _stearLerp += 3f * Time.deltaTime;

            if (_stearLerp >= 1)
                _stearLerp = 0;
        }

        public void EndBreak()
        {
            _leftWheelTrailRenderer.emitting = false;
            _rightWheelTrailRenderer.emitting = false;
        }

        public void Break()
        {
            _breakSound.Play();
            _leftWheelTrailRenderer.emitting = true;
            _rightWheelTrailRenderer.emitting = true;
        }

        public void Fire()
        {
            _leftFire.Play();
            _rightFire.Play();
        }

        public void FireStop()
        {
            _leftFire.Stop();
            _rightFire.Stop();
        }

        public void Accelerate(float acceleration)
        {
            if (_windParticle.isPlaying && acceleration < 0.5f)
                _windParticle.Stop();

            if (_windParticle.isPlaying == false && acceleration > 0.5f)
                _windParticle.Play();

            _engineSound.pitch = acceleration / 1.5f;
            ParticleSystem.MainModule ps = _windParticle.main;
            ps.maxParticles = (int)(acceleration * 50);
            ps.simulationSpeed = acceleration * 5;
            transform.position += Vector3.forward * acceleration;
        }

        public void OnDie()
        {
            _engineSound.Stop();
            _crashSound.Play();
            EndBreak();
            FireStop();
            _explosionParticle.Play();
            _smokeParticle.Play();
            _rigidbody.isKinematic = false;
            _moveTween?.Kill();
            _rigidbody.AddForce(Vector3.up * 10000, ForceMode.Impulse);
            transform.DOPunchScale(Vector3.one * 2, 0.25f).Play().SetAutoKill(true);
        }

        public void OnRestore()
        {
            _engineSound.Play();
            _repairSound.Play();
            _rigidbody.isKinematic = true;
            _smokeParticle.Stop();
        }

        public void OnCollect()
        {
            _collectSound.Play();
        }

        public void ShowAura()
        {
            _impregnabilityAura.Play();
        }

        public void HideAura()
        {
            _impregnabilityAura.Stop();
        }
    }
}