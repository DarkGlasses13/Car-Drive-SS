using System.Collections.Generic;

namespace Assets._Project.Entities
{
    public interface IEntityContainer
    {
        T GetFirstOrDefault<T>() where T : IEntity;
        IEnumerable<T> Get<T>() where T : IEntity;
        void Add(IEntity entity);
        void Remove(IEntity entity);
        void Clear();
    }
}
