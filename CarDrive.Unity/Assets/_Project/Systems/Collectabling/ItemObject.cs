using UnityEngine;

namespace Assets._Project.Systems.Collectabling
{
    public class ItemObject : MonoBehaviour 
    {
        [SerializeField] private ItemReference _reference;

        public string ID => _reference.ID;
    }
}