using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets._Project.Systems.Collecting
{
    public class UISlot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField] private Image _itemIcon;
        [SerializeField] private ParticleSystem _confetiParticle;
        private UIInventory _uiInventory;
        private Image _dragableImage;
        private CanvasScaler _canvasScaler;

        [field: SerializeField] public bool IsEquipment { get; private set; }
        [field: SerializeField, ShowIf("IsEquipment")]public ItemType Type { get; private set; }
        public IItem Item { get; private set; }

        public void Construct(UIInventory uiInventory, Image dragableImage, CanvasScaler canvasScaler)
        {
            _uiInventory = uiInventory;
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

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Item == null)
                return;

            if (Item.Type == ItemType.LootBox)
            {
                _uiInventory.OpenLootBox(transform.GetSiblingIndex());
                EmitMergeparticle();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (Item == null)
                return;

            _uiInventory.FromSlot = this;
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
            _uiInventory.ToSlot = this;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _dragableImage.gameObject.SetActive(false);

            if (Item != null)
                _itemIcon.gameObject.SetActive(true);

            if (_uiInventory.ToSlot != null)
            {
                _uiInventory.Swap();
            }
        }

        public void EmitMergeparticle() => _confetiParticle.Play();
    }
}