using UnityEngine.AddressableAssets;

namespace Assets._Project.SceneChange
{
    public class SceneChanger : ISceneChanger
    {
        public async void Change(object key)
        {
            await Addressables.LoadSceneAsync("Empty").Task;
            await Addressables.LoadSceneAsync(key).Task;
        }
    }
}
