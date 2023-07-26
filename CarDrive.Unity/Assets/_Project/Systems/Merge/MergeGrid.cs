using Assets._Project.Systems.Collecting;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Systems.Merge
{
    public class MergeGrid : MonoBehaviour
    {
        public event Action<int> OnAddNewItem;
        public event Action<int, int> OnSwap;
        public event Action<int, int, IItem> OnMerge;

        [SerializeField] private Image _dragableImage;
        private GridLayoutGroup _grid;
        private UISlot[] _slots;

        public int SlotsCount => _grid.transform.childCount;
        public UISlot FromSlot { get; set; }
        public UISlot ToSlot { get; set; }

        public void Construct(Canvas canvas, IItemDatabase database)
        {
            _grid = GetComponentInChildren<GridLayoutGroup>();
            _slots = GetComponentsInChildren<UISlot>();
            CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();
            Array.ForEach(_slots, slot => slot.Construct(this, _dragableImage, canvasScaler));
        }

        public void UpdateView(IEnumerable<IItem> items)
        {
            for (int i = 0; i < items.Count(); i++)
            {
                _slots[i].SetSlot(items.ElementAtOrDefault(i));
            }
        }

        public void Swap()
        {
            if (FromSlot.Item == null)
                return;

            int fromSlotIndex = FromSlot.transform.GetSiblingIndex();
            int toSlotIndex = ToSlot.transform.GetSiblingIndex();

            if (ToSlot.Item != null)
            {
                if (FromSlot.Item.ID == ToSlot.Item.ID)
                {
                    IItem mergeResult = ToSlot.Item.MergeResult;

                    if (mergeResult != null)
                    {
                        OnMerge?.Invoke(fromSlotIndex, toSlotIndex, mergeResult);
                        return;
                    }
                }
            }

            OnSwap?.Invoke(fromSlotIndex, toSlotIndex);
        }

        public void OpenLootBox(int slot)
        {
            OnAddNewItem?.Invoke(slot);
        }
    }
}