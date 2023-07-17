using Assets._Project.CameraControl;
using Assets._Project.DI;
using Assets._Project.Helpers;
using Assets._Project.Input;
using Assets._Project.SceneChange;
using UnityEngine;

namespace Assets._Project
{
    public class Bootstrap : Runner
    {
        protected async void Start()
        {
            Application.targetFrameRate = 60;
            DIContainer container = new GameObject("Project DI Container").AddComponent<DIContainer>();
            DontDestroyOnLoad(container);
            LocalAssetLoader assetLoader = new();
            PlayerInputConfig playerInputConfig = await assetLoader.Load<PlayerInputConfig>("Player Input Config");
            IPlayerInput playerInput = new PlayerInput(playerInputConfig);
            ISceneChanger sceneChanger = new SceneChanger();
            container.Bind(assetLoader);
            container.Bind(playerInput);
            container.Bind(sceneChanger);
            container.Bind(new Cinematographer());

            _systems = new() 
            {

            };

            sceneChanger.Change("Level");
        }
    }
}
