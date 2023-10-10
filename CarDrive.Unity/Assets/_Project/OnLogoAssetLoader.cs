using Assets._Project.Systems.Chunk_Generation;
using UnityEngine;

namespace Assets._Project
{
    public class OnLogoAssetLoader : MonoBehaviour
    {
        public ChunksLoader ChunksLoader {  get; private set; }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            ChunksLoader = new ChunksLoader();
        }

        private async void Start()
        {
            await ChunksLoader.LoadAsync();
        }
    }
}