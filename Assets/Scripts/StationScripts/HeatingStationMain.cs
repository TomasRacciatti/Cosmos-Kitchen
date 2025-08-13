using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeatingStationMain : MonoBehaviour, IDropHandler
{
    public GameObject _itemInSlot;
    public IngredientScript _ingredientInSlot;
    IngredientScript _ingredientCheck;

    [SerializeField] TextMeshProUGUI _buttonText;

    int _toolRequired;
    [SerializeField] Slider _toolCheck;
    public bool _thermoRunning = false;
    private Coroutine _thermoRutine;
    private float _internalThermo;
    
    [Header("SFX")]
    [SerializeField] AudioClip StationSound;


    public void OnDrop(PointerEventData _eventData)
    {
        _ingredientCheck = _eventData.pointerDrag.GetComponent<IngredientScript>();

        if (transform.childCount == 0)
        {
            if (_ingredientCheck != null)
            {
                if (_ingredientCheck.GetFoodState() == CookingState.Raw)
                {
                    GameObject _dropped = _eventData.pointerDrag;
                    DragableItem _dragableItem = _dropped.GetComponent<DragableItem>();
                    _dragableItem._parentAfterDrag = transform;
                    _itemInSlot = _dropped;
                    _ingredientInSlot = _ingredientCheck;
                    _toolRequired = _ingredientInSlot.HeatingRequirement();
                }
            }
        }
    }

    public void InteractThermometer(Slider _thermometer)
    {
        
        AudioManager.instance.PlaySFX(StationSound); //sfx

        if (transform.childCount == 1)
        {
            _toolCheck = _thermometer;
            if (_thermoRunning)
            {
                _buttonText.text = "Start";
                StopThermometer();
                AudioManager.instance.StopAllSFX();  //stop vfx
            }
            else
            {
                _buttonText.text = "Stop";
                _thermoRunning = true;
                _thermometer.value = 0;
                _thermoRutine = StartCoroutine(ThermometerCoroutine());
            }
        }
    }

    private void StopThermometer()
    {
        StopAllCoroutines();
        _thermoRutine = null;
        _thermoRunning = false;
        _toolCheck.value = 0;
        HeatCheck();
    }

    IEnumerator ThermometerCoroutine()
    {
        
        while (_toolCheck.value < 100)
        {
            _toolCheck.value++;
            _internalThermo = _toolCheck.value;
            yield return new WaitForFixedUpdate();
        }

        if (_toolCheck.value == 100)
        StopThermometer();
        AudioManager.instance.StopAllSFX();  //stop vfx
        _buttonText.text = "Start";
    }

    private void HeatCheck()
    {
        QualityLevel _qualityResult;
        _internalThermo = (int)Mathf.Round(_internalThermo / 20);
        int _qualityValue = (int)Mathf.Abs(_internalThermo - _toolRequired);

        switch (_qualityValue)
        {
            case 0:
                _qualityResult = QualityLevel.Perfect;
                Debug.Log("Cooked: Perfect");
                break;
            case 1:
                _qualityResult = QualityLevel.Excelent;
                Debug.Log("Cooked: Excelent");
                break;
            case 2:
                _qualityResult = QualityLevel.Good;
                Debug.Log("Cooked: Good");
                break;
            case 3:
                _qualityResult = QualityLevel.Decent;
                Debug.Log("Cooked: Decent");
                break;
            case 4:
                _qualityResult = QualityLevel.Bad;
                Debug.Log("Cooked: Bad");
                break;
            default:
                _qualityResult = QualityLevel.Horrible;
                Debug.Log("Cooked: Horrible");
                break;
        }

        _internalThermo = 0;
        _ingredientInSlot.ProcessFood(CookingState.Cooked, _qualityResult);
        InventoryManager._instance.Add(_itemInSlot);
        _itemInSlot = null;
        _ingredientInSlot = null;
    }
       
}
