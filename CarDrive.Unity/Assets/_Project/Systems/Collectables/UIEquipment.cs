using Assets._Project.Systems.Collecting;
using UnityEngine;

namespace Assets._Project.Systems.Collectables
{
    public class UIEquipment : MonoBehaviour
    {
        public int SlotsCount {  get; private set; }

        private void Awake()
        {
            SlotsCount = GetComponentsInChildren<UISlot>().Length;
        }
    }
}