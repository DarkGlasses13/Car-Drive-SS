using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets._Project.Helpers
{
    public class Pool<T> : IPool<T> where T : MonoBehaviour
    {
        private Func<T> _factory;
        private List<T> _elements;
        private Transform _container;

        public Pool(Func<T> factory, Transform container, int amount)
        {
            _factory = factory;
            _container = container;
            _elements = new List<T>();

            for (int i = 0; i < amount; i++)
                Create(false);
        }

        public int GetActiveCount() => _elements.Where(element => element.gameObject.activeSelf).Count();

        public T Get()
        {
            if (HasAvailable(out T availableElement))
            {
                return availableElement;
            }

            T createdElement = Create(true);
            return createdElement;
        }

        public T Get(Vector3 position)
        {
            T instance = Get();
            instance.transform.position = position;
            return instance;
        }

        public void ReleaseAll()
        {
            _elements
                .Where(element => element.gameObject.activeSelf == true).ToList()
                .ForEach(element => element.gameObject.SetActive(false));
        }

        private bool HasAvailable(out T availableElement)
        {
            for (int i = 0; i < _elements.Count; i++)
            {
                T element = _elements[Random.Range(0, _elements.Count)];

                if (element.gameObject.activeSelf == false)
                {
                    element.gameObject.SetActive(true);
                    availableElement = element;
                    return true;
                }
            }

            availableElement = null;
            return false;
        }

        protected T Create(bool isActive)
        {
            T element = _factory.Invoke();
            _elements.Add(element);
            element.transform.SetParent(_container);
            element.gameObject.SetActive(isActive);
            return element;
        }
    }
}