using Cinemachine;
using System;
using UnityEngine;

namespace Assets._Project.CameraControl
{
    [Serializable]
    public class TypedCamera
    {
        [field: SerializeField] public GameCamera Type { get; private set; }
        [field: SerializeField] public CinemachineVirtualCamera Instance { get; private set; }

        public TypedCamera(GameCamera type, CinemachineVirtualCamera instance)
        {
            Type = type;
            Instance = instance;
        }
    }
}