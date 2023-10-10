using Assets._Project.Systems.ChunkGeneration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Assets._Project.Systems.Chunk_Generation
{
    public class ChunksLoader
    {
        private List<Chunk> _chunks;
        private AsyncOperationHandle<IList<GameObject>> _loading;

        public async Task<IEnumerable<Chunk>> LoadAsync()
        {
            if (_chunks == null)
            {
                if (_loading.Equals(default))
                {
                    _loading = Addressables.LoadAssetsAsync<GameObject>("In Game Chunk", null);
                }

                await _loading.Task;
                _chunks = new(_loading.Result.Select(chunk => chunk.GetComponent<Chunk>()));
            }

            return _chunks.AsEnumerable();
        }
    }
}
