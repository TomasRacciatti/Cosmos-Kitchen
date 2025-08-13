using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientDisplay : MonoBehaviour
{
    [Header("Ingredients")]
    [SerializeField] TextMeshProUGUI _ingredientName;
    [SerializeField] Image _ingredientPicture;

    public void UpdateIngredientDisplay(string name, Sprite icon)
    {
        _ingredientName.text = name;
        _ingredientPicture.sprite = icon;
    }
}
