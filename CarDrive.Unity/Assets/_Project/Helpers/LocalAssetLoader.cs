using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets._Project.Helpers
{
    public class LocalAssetLoader
    {
        public async Task<T> Load<T>(object key)
        {
            T asset = await Addressables.LoadAssetAsync<T>(key).Task;
            return asset;
        }

        public async Task<IList<T>> LoadAll<T>(AssetLabelReference label, Action<T> callback = null)
        {
            IList<T> assets = await Addressables.LoadAssetsAsync<T>(label, callback).Task;
            return assets;
        }

        public async Task<C> LoadAndInstantiateAsync<C>(object key, Transform parent) where C : Component
        {
            GameObject instance = await Addressables.InstantiateAsync(key).Task;

            if (instance.TryGetComponent(out C component) == false)
            {
                throw new NullReferenceException(
                    $"There is no such component on the {instance.name}");
            }

            instance.transform.SetParent(parent);
            return component;
        }

        public async Task<IList<C>> LoadAllAndInstantiate<C>(object[] keys, Transform parent) where C : Component
        {
            IList<C> instances = new List<C>();

            foreach (object key in keys)
            {
                instances.Add(await LoadAndInstantiateAsync<C>(key, parent));
            }

            return instances;
        }

        public void UnloadInstance(GameObject instance)
        {
            instance.SetActive(false);
            Addressables.ReleaseInstance(instance);

        }

        public void Unload(object asset)
        {
            Addressables.Release(asset);
        }
    }
}
