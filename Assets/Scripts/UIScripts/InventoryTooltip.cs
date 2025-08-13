using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTooltip : MonoBehaviour
{
    public static InventoryTooltip _instance;
    [Header("Ingredient Tooltip")]
    [SerializeField] GameObject _tooltipMain;
    [SerializeField] TextMeshProUGUI _tooltipIngredient;
    [SerializeField] TextMeshProUGUI _tooltipProcess;
    [SerializeField] Image _tooltipIngredientQuality;
    [SerializeField] Image _tooltipProcessQuality;
    [Header("Recipe Tooltip")]
    [SerializeField] GameObject _tooltipRecipeMain;
    [SerializeField] TextMeshProUGUI _tooltipRecipeName;
    [SerializeField] TextMeshProUGUI _tooltipRecipeDescription;
    [SerializeField] Image _tooltiplRecipeQuality;

    private void Start()
    {
        if (_instance == null)
        { 
            _instance = this;
        }    
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void OpenIngredientTooltip(string _ingredientName, string _processName, float _qualityFill, float _processFill)
    {
        _tooltipIngredient.text = _ingredientName;
        _tooltipProcess.text = _processName;
        _tooltipIngredientQuality.fillAmount = _qualityFill;
        _tooltipProcessQuality.fillAmount = _processFill;
        _tooltipMain.SetActive(true);
    }

    public void OpenRecipeTooltip(string _recipeName, string _recipeDescription, float _qualityFill)
    {
        _tooltipRecipeName.text = _recipeName;
        _tooltipRecipeDescription.text = _recipeDescription;
        _tooltiplRecipeQuality.fillAmount = _qualityFill;
        _tooltipRecipeMain.SetActive(true);
    }

    public void CloseTooltip()
    {
        _tooltipMain.SetActive(false);
        _tooltipRecipeMain.SetActive(false);
    }
}
