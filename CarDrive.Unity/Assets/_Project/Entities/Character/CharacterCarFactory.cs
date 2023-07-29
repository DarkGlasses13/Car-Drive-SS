using Assets._Project.Helpers;
using UnityEngine;

namespace Assets._Project.Entities.Character
{
    public class CharacterCarFactory
    {
        private GameObject _prefab;

        public CharacterCarFactory(GameObject prefab)
        {
            _prefab = prefab;
        }

        public CharacterCar Create(SpawnData spawnData)
        {
            CharacterCar instance = Object
                .Instantiate(_prefab, spawnData.Position, spawnData.Rotation, spawnData.Parent)
                .GetComponent<CharacterCar>();
            return instance;
        }
    }
}