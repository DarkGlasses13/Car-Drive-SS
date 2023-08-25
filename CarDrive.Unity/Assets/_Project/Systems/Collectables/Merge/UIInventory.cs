using Assets._Project.Systems.Collectables;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Systems.Collecting
{
    public class UIInventory : MonoBehaviour
    {
        public event Action<int> OnLootBoxOpened;
        public event Action<UISlot, UISlot> OnSwap;

        [SerializeField] private Image _dragableImage;
        [SerializeField] private AudioSource
            _lootBoxOpenSound;

        private GridLayoutGroup _grid;
        private UISlot[] _slots;
        private UISlot[] _equipmentSlots;

        public int SlotsCount => _grid.transform.childCount;
        public UISlot FromSlot { get; set; }
        public UISlot ToSlot { get; set; }

        public void Construct(Canvas canvas, UIEquipment equipment)
        {
            _grid = GetComponentInChildren<GridLayoutGroup>();
            _slots = GetComponentsInChildren<UISlot>();
            _equipmentSlots = equipment.GetComponentsInChildren<UISlot>();
            Array.ForEach(_slots, slot => slot.Construct(this, _dragableImage, canvas));
            Array.ForEach(_equipmentSlots, slot => slot.Construct(this, _dragableImage, canvas));
        }

        public void UpdateView(IEnumerable<IItem> items, IEnumerable<IItem> equipment)
        {
            for (int i = 0; i < items.Count(); i++)
            {
                _slots[i].SetSlot(items.ElementAtOrDefault(i));
            }

            for (int i = 0; i < equipment.Count(); i++)
            {
                _equipmentSlots[i].SetSlot(equipment.ElementAtOrDefault(i));
            }
        }

        public void Swap()
        {
            OnSwap?.Invoke(FromSlot, ToSlot);
            FromSlot = ToSlot = null;
        }

        public void OpenLootBox(int slot)
        {
            OnLootBoxOpened?.Invoke(slot);
            _lootBoxOpenSound.Play();
        }

        public Rect GetFiledSlotsRect()
        {
            RectTransform rectTransform = _slots[0].GetRectTransform();
            return new(rectTransform.position, new(rectTransform.rect.width, rectTransform.rect.height));
        }

        public Vector3 GetSlotPosition(int index)
        {
            return _slots[index].GetRectTransform().position;
        }

        public int GetEquipmentSlot(int index)
        {
            return _equipmentSlots[index].transform.GetSiblingIndex();
        }

        public Vector3 GetEquipmentSlotPosition(int index)
        {
            return _equipmentSlots[index].GetRectTransform().position;
        }

        public Vector3 GetFirstSlotPosition()
        {
            return _slots.First(slot => slot.Item != null).GetRectTransform().position;
        }

        public Vector3 GetLastSlotPosition()
        {
            return _slots.Last(slot => slot.Item != null).GetRectTransform().position;
        }

        public int GetFirstSlot()
        {
            return _slots.First(slot => slot.Item != null).transform.GetSiblingIndex();
        }

        public int GetLastSlot()
        {
            return _slots.Last(slot => slot.Item != null).transform.GetSiblingIndex();
        }
    }
}