using Assets._Project.Architecture;
using Assets._Project.Architecture.DI;
using Assets._Project.Architecture.UI;
using Assets._Project.CameraControl;
using Assets._Project.Helpers;
using Assets._Project.Input;
using Assets._Project.SaveLoad;
using Assets._Project.SceneChange;
using Assets._Project.Systems.Collecting;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Assets._Project
{
    public class ProjectRunner : RunnerWithAutomaticSystemsInitialization
    {
        [SerializeField] private string _saveVersion;
        private DIContainer _container;
        private ISceneChanger _sceneChanger;
        private CollectablesConfig _collectablesConfig;
        private Storage _storage;
        private Player _player;
        private PlayerSave _defaultData;
        private LoadingScreen _loadingScreen;

        protected override async Task CreateSystems()
        {
            DontDestroyOnLoad(this);
            Application.targetFrameRate = 90;
            _container = new GameObject("Project DI Container").AddComponent<DIContainer>();
            Coroutiner coroutiner = new GameObject("Coroutiner").AddComponent<Coroutiner>();
            _storage = new();
            DontDestroyOnLoad(_container);
            DontDestroyOnLoad(coroutiner);
            LocalAssetLoader assetLoader = new();
            Camera playerCamera = await assetLoader.LoadAndInstantiateAsync<Camera>("Player Camera", null);
            IItemDatabase itemDatabase = await assetLoader.Load<ItemDatabase>("Item Database");
            _player = new(itemDatabase);
            _defaultData = _player.GetSave();
            DontDestroyOnLoad(playerCamera);
            _loadingScreen = await assetLoader.LoadAndInstantiateAsync<LoadingScreen>("Loading Screen", null);
            DontDestroyOnLoad(_loadingScreen);
            playerCamera.GetComponent<UniversalAdditionalCameraData>().cameraStack.Add(_loadingScreen.Camera);
            PlayerInputConfig playerInputConfig = await assetLoader.Load<PlayerInputConfig>("Player Input Config");
            IPlayerInput playerInput = new UniversalPlayerInput(playerInputConfig);
            _sceneChanger = new SceneChanger();
            _collectablesConfig = await assetLoader.Load<CollectablesConfig>("Collectables Config");
            _container.Bind(_storage);
            _container.Bind(playerCamera);
            _container.Bind(_loadingScreen);
            _container.Bind(assetLoader);
            _container.Bind(playerInput);
            _container.Bind(_sceneChanger);
            _container.Bind(_player);
            _container.Bind(coroutiner);
            _container.Bind(itemDatabase);
            _container.Bind(_collectablesConfig);
            _container.Bind(new Cinematographer());

            _systems = new()
            {
                new InputSystem(playerInput as IPlayerInputHandler)
            };
        }

        protected override void OnInitializationCompleted()
        {
            _storage.Load(_saveVersion, _defaultData, OnLoaded);
        }

        private void OnLoaded(string save)
        {
            _player.Update(JsonUtility.FromJson<PlayerSave>(save));
            _container.Bind(new Money(_collectablesConfig, _player.Money));
            _sceneChanger.Change("Level");
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                _storage.Save(_saveVersion, _player.GetSave());
            }
        }

        private void OnApplicationQuit()
        {
            _storage.Save(_saveVersion, _player.GetSave());
        }

        protected override void OnForceRestart()
        {
            return;
        }
    }
}
