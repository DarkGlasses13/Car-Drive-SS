using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Moonee.MoonSDK
{
    public class SDKStarter : MonoBehaviour
    {
        [SerializeField] private GameObject moonSDK;
        [SerializeField] private GameObject intro;

        private AsyncOperation asyncOperation;

        private void Start()
        {
            intro.gameObject.SetActive(false);
            asyncOperation = SceneManager.LoadSceneAsync(1);
            asyncOperation.allowSceneActivation = false;
            StartCoroutine(Starter());
        }

        private IEnumerator Starter()
        {
            intro.SetActive(true);
            yield return new WaitForSeconds(4f);
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
