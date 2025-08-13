using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    //public IngredientScript _ingredientInSlot;

    private void Start()
    {
        //if (transform.childCount == 1)
           //_ingredientInSlot = GetComponentInChildren<IngredientScript>();
    }

    public void OnDrop(PointerEventData _eventData)
    {
        IngredientScript _ingredientCheck = _eventData.pointerDrag.GetComponent<IngredientScript>();
        PlateScript _plateCheck = _eventData.pointerDrag.GetComponent<PlateScript>();

        if (transform.childCount == 0)
        {
            if (_ingredientCheck)
            {
                GameObject _dropped = _eventData.pointerDrag;
                DragableItem _dragableItem = _dropped.GetComponent<DragableItem>();
                _dragableItem._parentAfterDrag = transform;
                _dropped.transform.localScale = Vector3.one;
                //_ingredientInSlot = _ingredientCheck;
            }

            if (_plateCheck)
            {
                GameObject _dropped = _eventData.pointerDrag;
                DragableItem _dragableItem = _dropped.GetComponent<DragableItem>();
                _dragableItem._parentAfterDrag = transform;
                _dropped.transform.localScale = Vector3.one;
            }
        }

        _ingredientCheck = null;
    }

}
