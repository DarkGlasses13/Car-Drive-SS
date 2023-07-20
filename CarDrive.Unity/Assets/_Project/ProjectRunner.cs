using Assets._Project.CameraControl;
using Assets._Project.DI;
using Assets._Project.Helpers;
using Assets._Project.Input;
using Assets._Project.SceneChange;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Project
{
    public class ProjectRunner : Runner
    {
        protected override async Task CreateSystems()
        {
            DontDestroyOnLoad(this);
            Application.targetFrameRate = 60;
            DIContainer container = new GameObject("Project DI Container").AddComponent<DIContainer>();
            Coroutiner coroutiner = new GameObject("Coroutiner").AddComponent<Coroutiner>();
            DontDestroyOnLoad(container);
            DontDestroyOnLoad(coroutiner);
            LocalAssetLoader assetLoader = new();
            PlayerInputConfig playerInputConfig = await assetLoader.Load<PlayerInputConfig>("Player Input Config");
            IPlayerInput playerInput = new UniversalPlayerInput(playerInputConfig);
            ISceneChanger sceneChanger = new SceneChanger();
            container.Bind(assetLoader);
            container.Bind(playerInput);
            container.Bind(sceneChanger);
            container.Bind(coroutiner);
            container.Bind(new Cinematographer());

            _systems = new()
            {

            };

            sceneChanger.Change("Level");
        }
    }
}
