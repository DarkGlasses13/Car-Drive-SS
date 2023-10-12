using Assets._Project.Systems.Chunk_Generation;
using UnityEngine;

namespace Assets._Project
{
    public class OnLogoAssetLoader : MonoBehaviour
    {
        public ChunksLoader ChunksLoader {  get; private set; }

        public void Initialize()
        {
            DontDestroyOnLoad(this);
            ChunksLoader = new ChunksLoader();
        }
    }
}