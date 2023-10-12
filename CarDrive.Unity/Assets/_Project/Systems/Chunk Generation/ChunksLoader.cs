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

        public void Load()
        {
            _loading = Addressables.LoadAssetsAsync<GameObject>("In Game Chunk", null);
        }

        public async Task<IEnumerable<Chunk>> GetAsync()
        {
            if (_loading.Equals(default))
                return null;

            await _loading.Task;
            _chunks = new(_loading.Result.Select(chunk => chunk.GetComponent<Chunk>()));
            return _chunks.AsEnumerable();
        }
    }
}
