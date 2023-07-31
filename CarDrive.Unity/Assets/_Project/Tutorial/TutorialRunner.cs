using Assets._Project.Architecture;
using Assets._Project.Architecture.DI;
using Assets._Project.Input;
using Assets._Project.SceneChange;
using System;
using System.Threading.Tasks;
using UnityEngine.Playables;

namespace Assets._Project.Tutorial
{
    public class TutorialRunner : RunnerWithAutomaticSystemsInitialization
    {
        private PlayableDirector _director;
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
            _sceneChanger = projectContainer.Get<SceneChanger>();
            _playerInput = projectContainer.Get<IPlayerInput>();
            _director = GetComponent<PlayableDirector>();

        }

        public void OnStart()
        {
            _director.Resume();
        }

        public void OnBeforeAvoid()
        {
            _playerInput.Enable();
            _playerInput.OnStear += OnStear;
        }

        private void OnStear(float value)
        {
            if (value > 0)
            {
                _playerInput.Disable();
                _playerInput.OnStear -= OnStear;
                _director.Resume();
            }
        }

        public void OnFinal()
        {
            _sceneChanger.Change("Level");
        }
    }
}
