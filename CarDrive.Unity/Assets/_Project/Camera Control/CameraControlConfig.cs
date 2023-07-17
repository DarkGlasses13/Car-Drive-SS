using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.CameraControl
{
    [CreateAssetMenu(menuName = "Config/Camera Control")]
    public class CameraControlConfig : ScriptableObject
    {
        [SerializeField] private List<TypedCamera> _cameras;

        public IEnumerable<TypedCamera> Cameras => _cameras;
    }
}