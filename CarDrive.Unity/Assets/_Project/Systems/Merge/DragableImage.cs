using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets._Project.Systems.Merge
{
    public class DragableImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler
    {
        private Image _image;
        private CanvasScaler _canvasScaler;

        public Sprite Sprite { get => _image.sprite; set => _image.sprite = value; }
        public RectTransform RectTransform => _image.rectTransform;
        public bool RaycastTarget { get => _image.raycastTarget; set => _image.raycastTarget = value; }

        public void Construct(CanvasScaler canvasScaler)
        {
            _image = GetComponent<Image>();
            _canvasScaler = canvasScaler;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("Drag image");
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransform.anchoredPosition += eventData.delta / _canvasScaler.scaleFactor;
        }

        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log("Drop image");
        }
    }
}