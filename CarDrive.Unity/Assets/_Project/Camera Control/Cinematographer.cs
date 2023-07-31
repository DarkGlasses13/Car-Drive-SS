using Cinemachine;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.CameraControl
{
    public class Cinematographer
    {
        private List<TypedCamera> _cameras = new();


        public TypedCamera ActiveCamera { get; private set; }
        public int ActiveCameraIndex { get; private set; }

        public Cinematographer() { }

        public Cinematographer(IEnumerable<TypedCamera> cameras)
        {
            AddCameras(cameras);
        }

        public void AddCameras(IEnumerable<TypedCamera> cameras) => _cameras.AddRange(cameras);

        public void Clear() => _cameras.Clear();

        public void AddCamera(GameCamera type, CinemachineVirtualCamera camera) => _cameras.Add(new(type, camera));

        public void RemoveCamera(GameCamera type) => _cameras.Remove(_cameras.SingleOrDefault(camera => camera.Type == type));

        public void SetCameraTarget(GameCamera type, Transform followTarget = null, Transform lookTarget = null)
        {
            CinemachineVirtualCamera camera = _cameras.SingleOrDefault(camera => camera.Type == type).Instance;

            if (camera)
            {
                if (followTarget != null)
                    camera.Follow = followTarget;

                if (lookTarget != null)
                    camera.LookAt = lookTarget;
            }
        }

        [Button("Switch")]
        private void SwitchCamera()
        {
            ActiveCameraIndex++;

            if (ActiveCameraIndex >= _cameras.Count)
                ActiveCameraIndex = 0;

            SwitchCamera(_cameras[ActiveCameraIndex].Type);
        }

        public void SwitchCamera(GameCamera type, bool isReset = false, Transform followTarget = null, Transform lookTarget = null)
        {
            if (TrySwitchCamera(type, isReset))
            {
                SetCameraTarget(type, followTarget, lookTarget);
            }
        }

        public bool TrySwitchCamera(GameCamera type, bool isReset = false)
        {
            TypedCamera camera = _cameras.SingleOrDefault(camera => camera.Type == type);

            if (camera != null)
            {
                ActiveCamera = camera;
                ActiveCamera.Instance.MoveToTopOfPrioritySubqueue();

                if (isReset)
                {
                    ActiveCamera.Instance.enabled = false;
                    ActiveCamera.Instance.enabled = true;
                }

                ActiveCameraIndex = _cameras.IndexOf(ActiveCamera);
                return true;
            }

            return false;
        }

        public TypedCamera GetCamera(GameCamera type) => _cameras.SingleOrDefault(_camera => _camera.Type == type);
    }
}