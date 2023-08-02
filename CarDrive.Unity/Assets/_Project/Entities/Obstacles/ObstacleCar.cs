using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using System;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Entities.Obstacles
{
    public class ObstacleCar : Obstacle, ITranslatable
    {
        [SerializeField] private Transform[] _waypoints;
        [SerializeField] private float _detectionDistance;
        [SerializeField] private float _duration;
        [SerializeField]private AudioSource
            _engineSound;

        private bool _isMove;
        private TweenerCore<Vector3, Path, PathOptions> _pathTween;
        private Quaternion _startRotation;

        private void Awake()
        {
            _startRotation = transform.rotation;
        }

        private void OnEnable()
        {
            ResetPosition();
        }

        public void Translate(float duration)
        {
            ResetPosition();
            _isMove = true;
            _pathTween?.Kill();
            _pathTween = transform
                .DOPath(_waypoints.Select(waypoint => waypoint.position).ToArray(), duration, PathType.CatmullRom)
                .SetLookAt(0)
                .Play();
        }

        private void ResetPosition()
        {
            if (_waypoints == null || _waypoints.Length == 0)
                return;

            _isMove = false;
            transform.SetPositionAndRotation(_waypoints[0].position, _startRotation);
        }

        private void Update()
        {
            if (_waypoints == null || _waypoints.Length == 0)
                return;

            if (Vector3.Distance(Camera.main.transform.position, transform.position) <= _detectionDistance && _isMove == false)
            {
                Translate(_duration);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectionDistance);

            if (_waypoints == null)
                return;

            Gizmos.color = Color.yellow;
            Array.ForEach(_waypoints, waypoint => Gizmos.DrawWireSphere(waypoint.transform.position, 1));
        }
    }
}
