using UnityEngine;

namespace Assets._Project.Architecture.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaFormater : MonoBehaviour
    {
        private RectTransform _rectTRansform;

        private void Awake()
        {
            _rectTRansform = GetComponent<RectTransform>();
            Format();
        }

        public void Format()
        {
            Rect safeArea = Screen.safeArea;
            Vector2 
                anchorMin = safeArea.position,
                anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            _rectTRansform.anchorMin = anchorMin;
            _rectTRansform.anchorMax = anchorMax;
        }
    }
}