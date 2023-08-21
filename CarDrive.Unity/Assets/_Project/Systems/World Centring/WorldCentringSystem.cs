using Assets._Project.Architecture;
using Assets._Project.Systems.ChunkGeneration;
using Cinemachine;
using UnityEngine;

namespace Assets._Project.Systems.WorldCentring
{
    public class WorldCentringSystem : GameSystem
    {
        private readonly Transform _referens;
        private readonly CheckPointChunk _checkPoint;
        private readonly Transform[] _containers;
        private CinemachineBrain _cameraBrain;

        public WorldCentringSystem(Transform referensTransform, CheckPointChunk checkPointChunk, params Transform[] containers) 
        {
            _referens = referensTransform;
            _checkPoint = checkPointChunk;
            _containers = containers;
        }

        public override void OnEnable()
        {
            _cameraBrain = Camera.main.GetComponent<CinemachineBrain>();
            _checkPoint.OnEnter += OnCheckPointEnter;
        }

        private void OnCheckPointEnter(CheckPointChunk chunk)
        {
            float shift = _referens.position.z;

            _cameraBrain.enabled = false;

            for (int i = 0; i < _containers.Length; i++)
            {
                for (int c = 0; c < _containers[i].childCount; c++)
                {
                    _containers[i].GetChild(c).position += Vector3.back * shift;
                }
            }

            _cameraBrain.enabled = true;
        }

        public override void OnDisable()
        {
            _checkPoint.OnEnter -= OnCheckPointEnter;
        }
    }
}
