using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Ball : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Canvas canvas;
    public Column fromColumn;
    
    [Header("SFX")] 
    [SerializeField] AudioClip DragSound;
    [SerializeField] AudioClip DropSound;
    
    private RectTransform _rt;
    private CanvasGroup _cg;
    private Transform _originalParent;
    private int _originalSiblingIndex;
    private Vector3 _originalPosition;
    private bool _draggingAllowed;
    private RectTransform _dragLayer;
    private Vector2 _dragOffset;
    private RectTransform _parentRect;
    private GraphicRaycaster _raycaster;

    private Color _ballColor = Color.white;
    
    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _cg = GetComponent<CanvasGroup>();
    }
    
    private void Start()
    {
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        _raycaster = canvas != null
            ? canvas.GetComponent<GraphicRaycaster>() ?? canvas.gameObject.AddComponent<GraphicRaycaster>()
            : null;

        var t = canvas != null ? canvas.transform.Find("DragLayer") : null;
        if (t != null) _dragLayer = t as RectTransform;
    }
    
    #region Color API
    public void SetColor(Color c)
    {
        _ballColor = c;
        var img = GetComponent<Image>();
        if (img != null) img.color = c;
    }

    public Color GetColor() => _ballColor;
    #endregion
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        _draggingAllowed = (fromColumn != null && fromColumn.TopBall == this);
        if (!_draggingAllowed) return;


        _originalParent = transform.parent;
        _originalSiblingIndex = transform.GetSiblingIndex();
        _originalPosition = _rt.anchoredPosition;

        var newParent = (Transform)_dragLayer ?? canvas.transform;
        transform.SetParent(newParent, worldPositionStays: true);
        _rt.localScale = Vector3.one;
        
        _parentRect = newParent as RectTransform;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentRect, eventData.position, eventData.pressEventCamera, out var mouseLocal);
        
        _dragOffset = _rt.anchoredPosition - mouseLocal;
        
        if (_cg != null) _cg.blocksRaycasts = false;

        if (AudioManager.instance != null && DragSound != null)
            AudioManager.instance.PlaySFX(DragSound);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!_draggingAllowed) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentRect, eventData.position, eventData.pressEventCamera, out var mouseLocal);

        _rt.anchoredPosition = mouseLocal + _dragOffset;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_draggingAllowed) return;
        if (_cg) _cg.blocksRaycasts = true;

        // Find a drop area under the pointer
        ColumnDropArea dropArea = RaycastForDropArea(eventData);

        if (dropArea != null && dropArea.column != null && dropArea.column.CanAddBall(this))
        {
            // Update stacks (source pop first, then push into target)
            if (fromColumn != null) fromColumn.TryPopBall(this);
            dropArea.column.PushBall(this);

            if (AudioManager.instance != null && DropSound != null)
                AudioManager.instance.PlaySFX(DropSound);
        }
        else
        {
            // Snap back to original position/parent and keep stack unchanged
            transform.SetParent(_originalParent, worldPositionStays: true);
            transform.SetSiblingIndex(_originalSiblingIndex);
            _rt.anchoredPosition = _originalPosition;

            if (AudioManager.instance != null && DropSound != null)
                AudioManager.instance.PlaySFX(DropSound);
        }

        _draggingAllowed = false;
    }
    
    private ColumnDropArea RaycastForDropArea(PointerEventData eventData)
    {
        if (_raycaster == null) return null;

        var results = new List<RaycastResult>();
        _raycaster.Raycast(eventData, results);

        for (int i = 0; i < results.Count; i++)
        {
            var go = results[i].gameObject;
            var area = go.GetComponent<ColumnDropArea>();
            if (area != null) return area;
        }
        return null;
    }

}