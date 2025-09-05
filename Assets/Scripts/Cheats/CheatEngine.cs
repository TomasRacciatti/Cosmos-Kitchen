using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatEngine : MonoBehaviour
{
    [SerializeField] GameObject[] _ingredientPrefabs;
    [SerializeField] GameObject[] _platePrefabs;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            AddPlate(0);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            AddPlate(1);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            AddPlate(2);
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            AddPlate(3);
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            AddPlate(4);
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            AddPlate(5);
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            AddPlate(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddIngredient(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddIngredient(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AddIngredient(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AddIngredient(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            AddIngredient(4);
        }
    }

    private void AddIngredient(int i)
    {
        GameObject ingredient = Instantiate(_ingredientPrefabs[i]);
        ingredient.GetComponent<IngredientScript>().SetIngredientQuality(5);
        InventoryManager._instance.Add(ingredient);
    }

    private void AddPlate(int i)
    {
        GameObject plate = Instantiate(_platePrefabs[i]);
        plate.GetComponent<PlateScript>().GetIngredients();
        InventoryManager._instance.Add(plate);
    }
}
