using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(DragableItem))]
public class PlateScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Ingredients")]
    [SerializeField] IngredientScript[] _ingredients;

    [Header("Plate Data")]
    public string _plateName;
    public string _plateDescription;
    public Sprite _plateIcon;
    public QualityLevel _plateGrade;
    public string[] _plateComments;
    private int _ingredientIndex;
    public Dictionary<string, string> _plateIngredients;

    public void CreatePlate(IngredientScript _ingredient0, IngredientScript _ingredient1, IngredientScript _ingredient2)
    {
        _ingredients = new IngredientScript[3];
        _ingredients[0] = _ingredient0;
        _ingredients[1] = _ingredient1;
        _ingredients[2] = _ingredient2;
        _plateComments = new string[6];
        _ingredientIndex = 0;

        CalculateGrade();
        for (int i = 0; i < _ingredients.Length; i++)
        {
            Destroy(_ingredients[i].gameObject);
        }
        InventoryManager._instance.Add(this.gameObject);
    }
    public void GetIngredients()
    {
        _plateIngredients = new Dictionary<string, string>();
        IngredientScript _ingredientScript;

        for (int i = 0; i < _ingredients.Length; i++)
        {
            _ingredientScript = _ingredients[i];

            _plateIngredients.Add(_ingredientScript.ReturnIngredientName(), _ingredientScript.ReturnProcessName());
        }
    }

    private void CalculateGrade()
    {
        float _plateGradetemp = 0;
        float debug = 0;

        for (int i = 0; i < _ingredients.Length; i++)
        {
            debug = _ingredients[i].ReturnIngredientQuality() + _ingredients[i].ReturnProcessQuality();
            _plateGradetemp += debug;
            UnityEngine.Debug.Log("Puntuacion de Ingrediente: " + debug);

            if (_ingredients[i].ReturnIngredientQuality() != 100)
            {
                MakeComments(_ingredients[i].ReturnIngredientName());
            }
            if (_ingredients[i].ReturnProcessQuality() != 100)
            {
                MakeComments(_ingredients[i].ReturnProcessName(), _ingredients[i].ReturnIngredientName());
            }
        }
        UnityEngine.Debug.Log("Puntuacion de Plato: " + _plateGradetemp);

        for (int i = 0; i < _plateComments.Length; i++)
        {
            UnityEngine.Debug.Log(_plateComments[i]);
        }

        switch (_plateGradetemp)
        {
            case <120:
                _plateGrade = QualityLevel.Horrible;
                break;
            case <240:
                _plateGrade = QualityLevel.Bad;
                break;
            case <360:
                _plateGrade = QualityLevel.Decent;
                break;
            case <480:
                _plateGrade = QualityLevel.Good;
                break;
            case <600:
                _plateGrade = QualityLevel.Excelent;
                break;
            default:
                _plateGrade = QualityLevel.Perfect;
                break;
        }
    }

    private void MakeComments(string _mistake)
    {
        _plateComments[_ingredientIndex] = "Low Quality " + _mistake;
        _ingredientIndex++;
    }
    private void MakeComments(string _mistake, string _ingredient)
    {
        _plateComments[_ingredientIndex] = "The " + _ingredient + " was badly " + _mistake;
        _ingredientIndex++;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        float _plateGradeFill = (float)_plateGrade/100;
        InventoryTooltip._instance.OpenRecipeTooltip(_plateName, _plateDescription, _plateGradeFill);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryTooltip._instance.CloseTooltip();
    }

    public QualityLevel ReturnQualityLevel()
    {
        return _plateGrade;
    }
}
