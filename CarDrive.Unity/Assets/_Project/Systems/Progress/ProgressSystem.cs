using Assets._Project.Architecture;
using Assets._Project.Systems.ChunkGeneration;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Systems.Progress
{
    public class ProgressSystem : GameSystem
    {
        private readonly Slider _bar;
        private readonly CanvasGroup _barCanvasGroup;
        private readonly CheckPointChunk _checkPoint;
        private readonly Transform _characterTransform;
        private float _totalDistance;

        public ProgressSystem(Slider bar, CheckPointChunk checkPoint, Transform characterTransform) 
        {
            _bar = bar;
            _barCanvasGroup = bar.GetComponent<CanvasGroup>();
            _checkPoint = checkPoint;
            _characterTransform = characterTransform;
        }

        public override void Enable()
        {
            _checkPoint.OnSpawned += OnCheckpointSpawned;
            _checkPoint.OnEnter += OnCheckPointEnter;
            OnCheckpointSpawned();
        }

        private void OnCheckPointEnter(CheckPointChunk chunk)
        {
            _barCanvasGroup.alpha = 0;
            _bar.gameObject.SetActive(false);
        }

        public override void Tick()
        {
            float distance = Mathf.Abs(Vector3.Distance(_checkPoint.transform.position, _characterTransform.position) / _totalDistance - 1);
            _bar.value = distance;
        }

        private void OnCheckpointSpawned()
        {
            _totalDistance = Vector3.Distance(_checkPoint.transform.position, _characterTransform.position);
            _bar.gameObject.SetActive(true);
            _barCanvasGroup.DOFade(1, 0.25f).Play().SetAutoKill(true);
        }

        public override void Disable()
        {
            _checkPoint.OnSpawned -= OnCheckpointSpawned;
            _checkPoint.OnEnter -= OnCheckPointEnter;
        }
    }
}
