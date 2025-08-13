using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngredientScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] ScriptableIngredient _baseData;
    private Image _ingredientSprite;

    [Header("Process Data")]
    [SerializeField]    QualityLevel _ingredientQuality;
    [SerializeField]    CookingState _process;
    [SerializeField]    QualityLevel _processQuality;

    private void Start()
    {
        _ingredientSprite = gameObject.GetComponent<Image>();
        ChangeIcon();
    }

    public void ProcessFood(CookingState _cookingState, QualityLevel _cookingQuality)
    {
        _process = _cookingState;
        _processQuality = _cookingQuality;
        ChangeIcon();
    }

    private void ChangeIcon()
    {
        switch (_process)
        {
            case CookingState.Raw:
                _ingredientSprite.sprite = _baseData._inventoryRaw;
                break;
            case CookingState.Cut:
                _ingredientSprite.sprite = _baseData._inventoryCut;
                break;
            case CookingState.Blended:
                _ingredientSprite.sprite = _baseData._inventoryBlended;
                break;
            case CookingState.Cooked:
                _ingredientSprite.sprite = _baseData._inventoryCooked;
                break;
            case CookingState.Dried:
                break;
            case CookingState.Fried:
                break;
            case CookingState.Frozen:
                break;
            case CookingState.Flattened:
                break;
            case CookingState.Bubbly:
                break;
        }
    }

    public CookingState GetFoodState()
    {
        return _process;
    }

    public void SetIngredientQuality(int _random)
    {
        QualityLevel _randomQuality;
        switch (_random)
        {
            case 0:
                _randomQuality = QualityLevel.Horrible;
                break;
            case 1:
                _randomQuality = QualityLevel.Bad;
                break;
            case 2:
                _randomQuality = QualityLevel.Decent;
                break;
            case 3:
                _randomQuality = QualityLevel.Good;
                break;
            case 4:
                _randomQuality = QualityLevel.Excelent;
                break;
            default:
                _randomQuality = QualityLevel.Perfect;
                break;
        }
        _ingredientQuality = _randomQuality;
    }

    public int CutRequirement()
    {
        return _baseData._cuttingTool;
    }
    public int BlendRequirement()
    {
        return _baseData._blendingSetting;
    }
    public int HeatingRequirement()
    {
        return _baseData._heatingTemperature;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryTooltip._instance.CloseTooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        string _cookingState = ReturnProcessName();
        float _processQualityFill = ReturnQualityFill(_processQuality);
        float _ingredientQualityFill = ReturnQualityFill(_ingredientQuality);

        InventoryTooltip._instance.OpenIngredientTooltip(_baseData._ingredientName, _cookingState, _ingredientQualityFill, _processQualityFill ) ;
    }

    public string ReturnProcessName()
    {
        string _cookingState = "";

        switch (_process)
        {
            case CookingState.Raw:
                _cookingState = "Raw";
                break;
            case CookingState.Cut:
                _cookingState = "Cut";
                break;
            case CookingState.Blended:
                _cookingState = "Blended";
                break;
            case CookingState.Cooked:
                _cookingState = "Cooked";
                break;
            case CookingState.Dried:
                _cookingState = "Dried";
                break;
            case CookingState.Fried:
                _cookingState = "Fried";
                break;
            case CookingState.Frozen:
                _cookingState = "Frozen";
                break;
            case CookingState.Flattened:
                _cookingState = "Flattened";
                break;
            case CookingState.Bubbly:
                _cookingState = "Bubbly";
                break;
        }

        return _cookingState;
    }

    public string ReturnIngredientName()
    {
        return _baseData._ingredientName;
    }

    public float ReturnQualityFill(QualityLevel _quality)
    {
        float _qualityFill = 0;

        switch (_quality)
        {
            case QualityLevel.Horrible:
                _qualityFill = 0;
                break;
            case QualityLevel.Bad:
                _qualityFill = 0.2f;
                break;
            case QualityLevel.Decent:
                _qualityFill = 0.4f;
                break;
            case QualityLevel.Good:
                _qualityFill = 0.6f;
                break;
            case QualityLevel.Excelent:
                _qualityFill = 0.8f;
                break;
            case QualityLevel.Perfect:
                _qualityFill = 1;
                break;
        }

        return (_qualityFill);
    }

    public float ReturnIngredientQuality()
    {
        return (int)_ingredientQuality;
    }
    public float ReturnProcessQuality()
    {
        return (int)_processQuality;
    }
}
