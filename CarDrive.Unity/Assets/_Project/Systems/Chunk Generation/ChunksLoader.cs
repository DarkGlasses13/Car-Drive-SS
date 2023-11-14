using Assets._Project.Systems.ChunkGeneration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets._Project.Systems.Chunk_Generation
{
    public class ChunksLoader
    {
        private readonly Dictionary<ChunkEnvironmentType, List<Chunk>> _chunks = new();

        public async Task<IEnumerable<Chunk>> LoadAsync(ChunkEnvironmentType type)
        {
            if (_chunks.ContainsKey(type) == false)
            {
                IList<GameObject> loaded = await Addressables.LoadAssetsAsync<GameObject>(type.ToString(), OnLoaded).Task;
                IEnumerable<Chunk> loadedChunks = loaded.Select(gameObject => gameObject.GetComponent<Chunk>());
                _chunks.Add(type, new List<Chunk>(loadedChunks));
            }

            return _chunks[type];
        }

        private void OnLoaded(GameObject chunk)
        {
            //Debug.Log(chunk.name);
        }
    }
}
