using Assets._Project.Systems.Collecting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets._Project.Systems.Merge
{
    public class UISlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField] private Image _itemIcon;
        private MergeGrid _grid;
        private Image _dragableImage;
        private CanvasScaler _canvasScaler;

        public IItem Item { get; private set; }

        public void Construct(MergeGrid grid, Image dragableImage, CanvasScaler canvasScaler)
        {
            _grid = grid;
            _dragableImage = dragableImage;
            _canvasScaler = canvasScaler;
        }

        public void SetSlot(IItem item)
        {
            Item = item;

            if (item != null)
            {
                _itemIcon.gameObject.SetActive(true);
                _itemIcon.sprite = item.Icon;
                return;
            }

            _itemIcon.gameObject.SetActive(false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (Item.Type == ItemType.LootBox)
            {
                _grid.OpenLootBox(transform.GetSiblingIndex());
            }

            _grid.FromSlot = this;
            _dragableImage.sprite = Item.Icon;
            _dragableImage.rectTransform.position = _itemIcon.rectTransform.position;
            _itemIcon.gameObject.SetActive(false);
            _dragableImage.gameObject.SetActive(true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _dragableImage.rectTransform.anchoredPosition += eventData.delta / _canvasScaler.scaleFactor;
        }

        public void OnDrop(PointerEventData eventData)
        {
            _grid.ToSlot = this;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _dragableImage.gameObject.SetActive(false);
            _itemIcon.gameObject.SetActive(true);

            if (_grid.ToSlot != null)
            {
                _grid.Swap();
            }
        }
    }
}