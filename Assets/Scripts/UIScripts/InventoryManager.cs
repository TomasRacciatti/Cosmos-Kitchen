using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager _instance;
    public InventorySlot[] _inventorySlots;
    [SerializeField] GameObject _inventoryMain;
    [SerializeField] GameObject _bookMain;
    public bool _inventoryOpen;
    public bool _bookOpen;
    public bool _canToggleBook = true;
    [Header("SFX")]
    [SerializeField] AudioClip OpenBookSound;
    [SerializeField] AudioClip CloseBookSound;
    
    [SerializeField] AudioClip InventoryOpenSound;
    [SerializeField] AudioClip InventoryCloseSound;

    private void Start()
    {
        _instance = this;
    }

    public void ToggleInventory()
    {
        if (_canToggleBook)
        {
            if (_bookOpen)
            {
                ToggleBook();
            }
            
            _inventoryOpen = !_inventoryOpen;

            if (_inventoryOpen)
            {
                AudioManager.instance.PlaySFX(InventoryOpenSound); 
                _inventoryMain.SetActive(true);
                InputManager._instance.UnlockMouse();
                InputManager._instance.StopAll();
                InputManager._instance.TurnOffInteraction();
            }
            else
            {
                AudioManager.instance.PlaySFX(InventoryCloseSound); 
                InventoryTooltip._instance.CloseTooltip();
                _inventoryMain.SetActive(false);
                InputManager._instance.ResumeAll();
                InputManager._instance.TurnOnInteraction();
            }
            
        }
    }
    public void ToggleBook()
    {
        if (_inventoryOpen) { ToggleInventory(); }
        _bookOpen = !_bookOpen;

        if (_bookOpen)
        {
            AudioManager.instance.PlaySFX(OpenBookSound); 
            _bookMain.SetActive(true);
            InputManager._instance.UnlockMouse();
            InputManager._instance.StopAll();
            InputManager._instance.TurnOffInteraction();
        }
        else
        {
            AudioManager.instance.PlaySFX(CloseBookSound); 
            _bookMain.SetActive(false);
            InputManager._instance.ResumeAll();
            InputManager._instance.TurnOnInteraction();
        }
    }

    public void CloseAll()
    {
        _bookMain.SetActive(false);
        _inventoryMain.SetActive(false);
        InputManager._instance.TurnOnInteraction();
    }

    public void Add(GameObject _item)
    {
        IngredientScript _itemIngredient = _item.GetComponent<IngredientScript>();

        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            if (_inventorySlots[i].transform.childCount == 0)
            {
                _item.transform.SetParent(_inventorySlots[i].transform);
                _item.transform.localScale = Vector3.one;
                //_inventorySlots[i]._ingredientInSlot = _itemIngredient;
                break;
            }
        }
    }
}
