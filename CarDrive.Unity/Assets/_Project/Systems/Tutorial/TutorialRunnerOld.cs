using Assets._Project.Architecture;
using Assets._Project.Architecture.DI;
using Assets._Project.Architecture.UI;
using Assets._Project.Input;
using Assets._Project.SceneChange;
using Cinemachine;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.Universal;
using UnityEngine.Timeline;

namespace Assets._Project.Systems.Tutorial
{
    public class TutorialRunnerOld : RunnerWithAutomaticSystemsInitialization
    {
        [SerializeField] private Camera _uiCamera;
        private PlayableDirector _director;
        private Player _player;
        private UniversalAdditionalCameraData _additionalCameraData;
        private LoadingScreen _loadingScreen;
        private SceneChanger _sceneChanger;
        private IPlayerInput _playerInput;

        protected override Task CreateSystems()
        {
            _systems = new();
            return Task.CompletedTask;
        }

        protected override void OnInitializationCompleted()
        {
            DIContainer projectContainer = FindObjectOfType<DIContainer>();
            Camera playerCamera = projectContainer.Get<Camera>();
            _player = projectContainer.Get<Player>();
            _additionalCameraData = playerCamera.GetComponent<UniversalAdditionalCameraData>();
            _additionalCameraData.cameraStack.Add(_uiCamera);
            _loadingScreen = projectContainer.Get<LoadingScreen>();
            _sceneChanger = projectContainer.Get<SceneChanger>();
            _playerInput = projectContainer.Get<IPlayerInput>();
            _director = GetComponent<PlayableDirector>();
            TrackAsset cinemachineTrack = ((TimelineAsset)_director.playableAsset).GetOutputTracks().SingleOrDefault(track => track is CinemachineTrack);
            _director.SetGenericBinding(cinemachineTrack, playerCamera.GetComponent<CinemachineBrain>());
            _loadingScreen.FadeOut(OnFadeOut);
        }

        private void OnFadeOut()
        {
            _director.Play();
        }

        public void OnStart()
        {
            _director.Resume();
        }

        public void OnBeforeAvoid()
        {
            _playerInput.Enable();
            //_playerInput.OnSwipe += OnStear;
        }

        private void OnStear(float value)
        {
            if (value > 0)
            {
                _playerInput.Disable();
                //_playerInput.OnSwipe -= OnStear;
                _director.Resume();
            }
        }

        public void OnBeforeAcceleration()
        {
            _playerInput.Enable();
            //_playerInput.OnSwipeEnded += OnAccelerate;
        }

        private void OnAccelerate(float value)
        {
            if (value > 0)
            {
                _playerInput.Disable();
                //_playerInput.OnSwipeEnded -= OnAccelerate;
                _director.Resume();
            }
        }

        public void OnBeforeBreak()
        {
            _playerInput.Enable();
            //_playerInput.OnSwipeEnded += OnBreak;
        }

        private void OnBreak(float value)
        {
            if (value < 0)
            {
                _playerInput.Disable();
                //_playerInput.OnSwipeEnded -= OnBreak;
                _director.Resume();
            }
        }

        public void OnFinal()
        {
            _player.IsTutorialCompleted = true;
            _loadingScreen.FadeIn(OnFadeIn);
        }

        private void OnFadeIn()
        {
            _additionalCameraData.cameraStack.Remove(_uiCamera);
            _sceneChanger.Change("Level");
        }

        protected override void OnForceRestart()
        {
            return;
        }
    }
}
