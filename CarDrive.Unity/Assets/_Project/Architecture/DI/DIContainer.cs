using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.DI
{
    public class DIContainer : MonoBehaviour
    {
        private List<object> _services = new();

        public void Bind<T>(T service)
        {
            if (service != null && _services.Contains(service) == false)
            {
                _services.Add(service);
            }
        }

        public T Get<T>()
        {
            return (T)_services.SingleOrDefault(service => service is T);
        }

        public void Unbind<T>(T service)
        {
            _services.Remove(service);
        }
    }
}
