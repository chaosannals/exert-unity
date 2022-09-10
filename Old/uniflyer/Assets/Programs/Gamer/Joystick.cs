using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Uniflyer
{
    public class Joystick : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField]
        private Cabin cabin = null;
        [SerializeField]
        private float radii = 0.0f;
        [SerializeField]
        private RectTransform cursor = null;
        [SerializeField]
        private RectTransform origin = null;

        public UnityEvent OnPress = new UnityEvent();
        public UnityEvent OnShake = new UnityEvent();
        public UnityEvent OnLoose = new UnityEvent();

        public void OnBeginDrag(PointerEventData data)
        {
            point(data);
            OnPress.Invoke();
        }

        public void OnDrag(PointerEventData data)
        {
            point(data);
            OnShake.Invoke();
        }

        public void OnEndDrag(PointerEventData data)
        {
            cursor.anchoredPosition = Vector2.zero;
            cabin.Arrow = Vector2.zero;
            OnLoose.Invoke();
        }

        private void point(PointerEventData data)
        {
            Vector2 position;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                origin, data.position, data.pressEventCamera, out position))
            {
                cursor.anchoredPosition = position.magnitude < radii ?
                    position : position.normalized * radii;
                cabin.Arrow = cursor.anchoredPosition / radii;
            }
        }
    }
}