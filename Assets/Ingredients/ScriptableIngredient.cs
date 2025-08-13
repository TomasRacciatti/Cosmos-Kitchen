using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum IngredientType
{
    Fruit,
    Plant,
    Fish,
    Exotic,
}
public enum CookingState
{
    Raw,
    Cut,
    Blended,
    Cooked,
    Dried,
    Fried,
    Frozen,
    Flattened,
    Bubbly,
}

public enum QualityLevel
{
    Horrible = 0,
    Bad = 20,
    Decent = 40,
    Good = 60,
    Excelent = 80,
    Perfect = 100,
}

[CreateAssetMenu(menuName = "Scriptables/Ingredient", fileName = "Ingredient_")]
public class ScriptableIngredient : ScriptableObject
{
    [Header("Data")]
    public IngredientType _type;
    public string _ingredientName;
    public Sprite _inventoryRaw;
    public Sprite _inventoryCut;
    public Sprite _inventoryBlended;
    public Sprite _inventoryCooked;

    [Header("Requierements")]
    public int _cuttingTool;
    public int _blendingSetting;
    public int _heatingTemperature;
}

