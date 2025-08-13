using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Ball : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Canvas canvas;
    public Column fromColumn;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private int originalSiblingIndex;
    private Color ballColor;
    private Vector2 dragOffset;
    private Transform originalParent;
    private Transform dragLayer;

    [Header("SFX")]
    [SerializeField] AudioClip DragSound;
    [SerializeField] AudioClip DropSound;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetColor(Color color)
    {
        ballColor = color;
        GetComponent<Image>().color = color;
    }

    public Color GetColor()
    {
        return ballColor;
    }

    public void OnBeginDrag(PointerEventData eventData)
{
    AudioManager.instance.StopAllSFX();
    AudioManager.instance.PlaySFX(DragSound);
    
    originalPosition = rectTransform.anchoredPosition;
    originalSiblingIndex = rectTransform.GetSiblingIndex();
    originalParent = transform.parent;

    canvasGroup.blocksRaycasts = false;

    if (dragLayer == null)
        dragLayer = canvas.transform.Find("DragLayer");

    if (dragLayer != null)
        transform.SetParent(dragLayer, worldPositionStays: false);

    RectTransform columnRect = fromColumn.transform as RectTransform;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        columnRect,
        eventData.position,
        canvas.worldCamera,
        out Vector2 localPoint);

    dragOffset = rectTransform.anchoredPosition - localPoint;
}


    public void OnDrag(PointerEventData eventData)
    {
        
        if (canvas == null || fromColumn == null) return;

        RectTransform columnRect = fromColumn.transform as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            columnRect,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPoint);

        rectTransform.anchoredPosition = localPoint + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        AudioManager.instance.StopAllSFX();
        AudioManager.instance.PlaySFX(DropSound);
        
        canvasGroup.blocksRaycasts = true;

        Column targetColumn = null;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (var result in results)
        {
            Column col = result.gameObject.GetComponent<Column>();
            if (col != null)
            {
                targetColumn = col;
                break;
            }
        }

        if (targetColumn != null && targetColumn.CanAddBall(this))
        {
            if (targetColumn != fromColumn)
            {
                fromColumn.RemoveBall(this);
                targetColumn.AddBall(gameObject);
                fromColumn = targetColumn;
            }
            UpdatePositionToColumnTop();
            fromColumn.UpdateBallInteractivity();
        }
        else
        {
            ReturnToOriginalPosition();
            fromColumn.UpdateBallInteractivity();
        }
    }

    private void UpdatePositionToColumnTop()
    {
        rectTransform.SetParent(fromColumn.transform);
        rectTransform.SetSiblingIndex(fromColumn.balls.Count - 1);
        rectTransform.anchoredPosition = new Vector2(0, -30 * (fromColumn.balls.Count - 1));
    }

    private void ReturnToOriginalPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
        rectTransform.SetSiblingIndex(originalSiblingIndex);
    }
}