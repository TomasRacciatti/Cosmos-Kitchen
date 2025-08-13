using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Image _image;
    [HideInInspector] public Transform _parentAfterDrag;

    private void Start()
    {
        _image = GetComponent<Image>();    
    }

    public void OnBeginDrag(PointerEventData _eventData)
    {
        _parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        _image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData _eventData)
    {
        transform.position = Input.mousePosition;

    }

    public void OnEndDrag(PointerEventData _eventData)
    {
        transform.SetParent(_parentAfterDrag);
        _image.raycastTarget = true;
    }

}
