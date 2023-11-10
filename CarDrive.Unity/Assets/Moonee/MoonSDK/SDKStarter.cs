using Assets._Project;
using Assets._Project.Architecture.UI;
using Assets._Project.Helpers;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Moonee.MoonSDK
{
    public class SDKStarter : MonoBehaviour
    {
        [SerializeField] private GameObject moonSDK;
        [SerializeField] private GameObject intro;
        [SerializeField] private OnLogoAssetLoader _assetLoader;

        private AsyncOperation asyncOperation;

        private void Start()
        {
            intro.gameObject.SetActive(false);
            _assetLoader.Initialize();
            DontDestroyOnLoad(_assetLoader);
            _assetLoader.ChunksLoader.Load();
            asyncOperation = SceneManager.LoadSceneAsync(1);
            asyncOperation.allowSceneActivation = false;
            StartCoroutine(Starter());
        }

        private IEnumerator Starter()
        {
            intro.SetActive(true);
            yield return new WaitForSeconds(4f);
            LocalAssetLoader assetLoader = new();
            yield return assetLoader.LoadAndInstantiateAsync<LoadingScreen>("Loading Screen", null);
            //DontDestroyOnLoad(FindAnyObjectByType<LoadingScreen>());
            InitializeMoonSDK();
        }

        private void InitializeMoonSDK()
        {
            moonSDK.SetActive(true);
            DontDestroyOnLoad(moonSDK);
            asyncOperation.allowSceneActivation = true;
        }
    }
}
