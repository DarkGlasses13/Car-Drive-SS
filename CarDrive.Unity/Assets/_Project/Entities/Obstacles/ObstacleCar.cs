using DG.Tweening;
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
        private bool _isMove;
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
            _isMove = true;

            transform
                .DOPath(_waypoints.Select(waypoint => waypoint.position).ToArray(), duration, PathType.CatmullRom)
                .SetLookAt(0).Play()
                .SetAutoKill(true);
        }

        private void ResetPosition()
        {
            _isMove = false;

            if (_waypoints != null)
            {
                transform.SetPositionAndRotation(_waypoints[0].position, _startRotation);
            }
        }

        private void Update()
        {
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
