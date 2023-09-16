using System;
using UnityEngine;

namespace Assets._Project.Entities.Obstacles
{
    public class Obstacle : Entity
    {
        [SerializeField]
        private GameObject[] _viewVariants;

        public void SetRandomView()
        {
            if (_viewVariants == null)
                return;

            if (_viewVariants.Length == 0)
                return;

            Array.ForEach(_viewVariants, view => view.SetActive(false));
            _viewVariants[UnityEngine.Random.Range(0, _viewVariants.Length)].SetActive(true);
        }
    }
}
