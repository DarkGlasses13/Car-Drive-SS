using Assets._Project.Architecture;
using Assets._Project.Systems.ChunkGeneration;
using DG.Tweening;
using UnityEngine;

namespace Assets._Project.Systems.Progress
{
    public class ProgressSystem : GameSystem
    {
        private readonly ProgressBar _bar;
        private readonly CanvasGroup _barCanvasGroup;
        private readonly CheckPointChunk _checkPoint;
        private readonly Transform _characterTransform;
        private readonly Player _player;
        private float _totalDistance;

        public ProgressSystem(ProgressBar bar, CheckPointChunk checkPoint, Transform characterTransform, Player player) 
        {
            _bar = bar;
            _barCanvasGroup = bar.GetComponent<CanvasGroup>();
            _checkPoint = checkPoint;
            _characterTransform = characterTransform;
            _player = player;
        }

        public override void OnEnable()
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
            _bar.Value = distance;
        }

        private void OnCheckpointSpawned()
        {
            _bar.CurrentLevel = _player.Level;
            _bar.NextLevel = _player.Level + 1;
            MoonSDK.TrackLevelEvents(MoonSDK.LevelEvents.Start, _player.Level + 1);
            _totalDistance = Vector3.Distance(_checkPoint.transform.position, _characterTransform.position);
            _bar.gameObject.SetActive(true);
            _barCanvasGroup.DOFade(1, 0.25f).Play().SetAutoKill(true);
        }

        public override void OnDisable()
        {
            _checkPoint.OnSpawned -= OnCheckpointSpawned;
            _checkPoint.OnEnter -= OnCheckPointEnter;
        }
    }
}
