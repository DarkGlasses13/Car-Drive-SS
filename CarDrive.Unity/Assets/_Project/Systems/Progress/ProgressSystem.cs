using Assets._Project.Architecture;
using Assets._Project.Systems.Chunk_Generation;
using Assets._Project.Systems.ChunkGeneration;
using DG.Tweening;
using UnityEngine;

namespace Assets._Project.Systems.Progress
{
    public class ProgressSystem : GameSystem
    {
        private readonly ProgressBar _bar;
        private readonly CanvasGroup _barCanvasGroup;
        private readonly ChunksEvents _chunksEvents;
        private readonly Player _player;
        private readonly ChunkGenerationConfig _config;
        private float _totalDistance;
        private int _passed;
        private float _distance;

        public ProgressSystem(ProgressBar bar, ChunksEvents chunksEvents,
            Player player, ChunkGenerationConfig config) 
        {
            _bar = bar;
            _barCanvasGroup = bar.GetComponent<CanvasGroup>();
            _chunksEvents = chunksEvents;
            _player = player;
            _config = config;
        }

        public override void OnEnable()
        {
            _chunksEvents.OnCheckPointEnter += OnCheckPointEnter;
            _chunksEvents.OnAnyPass += OnChunkPassed;
            _bar.CurrentLevel = _player.Level;
            _bar.NextLevel = _player.Level + 1;
        }

        private void OnChunkPassed(Chunk chunk)
        {
            _passed++;
            _distance = (float)_passed / _config.ChunksBetweenCheckPoints;
        }

        public override void Tick()
        {
            _bar.Value = Mathf.Lerp(_bar.Value, _distance, 5 * Time.deltaTime);
        }

        private void OnCheckPointEnter(CheckPointChunk chunk)
        {
            _passed = 0;
            _barCanvasGroup.alpha = 0;
            _bar.gameObject.SetActive(false);
            MoonSDK.TrackLevelEvents(MoonSDK.LevelEvents.Start, _player.Level + 1);
            _bar.CurrentLevel = _player.Level;
            _bar.NextLevel = _player.Level + 1;
            _bar.gameObject.SetActive(true);
            _barCanvasGroup.DOFade(1, 0.25f).Play().SetAutoKill(true);
        }

        public override void OnDisable()
        {
            _chunksEvents.OnCheckPointEnter -= OnCheckPointEnter;
        }
    }
}
