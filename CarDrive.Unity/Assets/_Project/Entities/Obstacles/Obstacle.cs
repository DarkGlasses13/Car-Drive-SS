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
            Array.ForEach(_viewVariants, view => view.SetActive(false));

            if (_viewVariants.Length > 0)
            {
                _viewVariants[UnityEngine.Random.Range(0, _viewVariants.Length)].SetActive(true);
            }
        }
    }
}
