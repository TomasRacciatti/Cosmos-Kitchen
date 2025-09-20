using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MiniGames.EggSort
{
    public class Egg : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image image;
        
        private bool _dragable = false;
        private Canvas _canvas;
        private Column _column;

        public void SetColumn(Column column)
        {
            _column = column;
        }
        public Column GetColumn() => _column;
        
        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            if (image == null) image = GetComponentInChildren<Image>();
        }

        private void OnEnable()
        {
            _canvas.sortingOrder = 70;
            image.raycastTarget = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_column.IsTopEgg(this)) return;

            _dragable = true;
            image.raycastTarget = false;
            _canvas.sortingOrder = 75;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_dragable) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform,
                eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
            transform.localPosition = localPoint;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_dragable) return;
            _canvas.sortingOrder = 70;
            image.raycastTarget = true;
            transform.SetParent(_column.transform, false); //hardcodeado hasta las tetas
            transform.SetParent(_column.childTransform, false); //cositas que unity me obliga a hacer
            _dragable = false;
        }
    }
}