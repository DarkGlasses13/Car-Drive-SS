using System.Collections.Generic;
using System.Linq;

namespace Assets._Project.Entities
{
    public class EntityContainer : IEntityContainer
    {
        private readonly List<IEntity> _entities = new();

        public void Add(IEntity entity) => _entities.Add(entity);

        public void Clear() => _entities.Clear();

        public IEnumerable<T> Get<T>() where T : IEntity => (IEnumerable<T>)_entities.Where(entity => entity is T);

        public T GetFirstOrDefault<T>() where T : IEntity => (T)_entities.FirstOrDefault(entity => entity is T);

        public void Remove(IEntity entity) => _entities.Remove(entity);
    }
}
