using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CookingStationManager : MonoBehaviour
{
    [SerializeField] InventorySlot[] _inventorySlots;
    [SerializeField] GameObject[] _platePrefab;
    [SerializeField] IngredientScript[] _ingredientScriptsReference;

    private Dictionary<string, string> _plateIngredients;
    
    [Header("SFX")]
    [SerializeField] AudioClip StationSound;


    public void CookPlate()
    {
        if (_inventorySlots != null)
        {
            if (CheckRaw())
            {
                GetIngredients();
                GameObject _plate = FindPlate();
                PlateScript _plateScript = _plate.GetComponent<PlateScript>();
                
                AudioManager.instance.PlaySFX(StationSound); //sfx  

                _plateScript.CreatePlate(_inventorySlots[0].GetComponentInChildren<IngredientScript>(), _inventorySlots[1].GetComponentInChildren<IngredientScript>(), _inventorySlots[2].GetComponentInChildren<IngredientScript>());
                _plateScript._plateIngredients = _plateIngredients;
            }
        }
    }

    private bool CheckRaw()
    {
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            if (_inventorySlots[i].GetComponentInChildren<PlateScript>())
            {
                return false;
            }
            if (_inventorySlots[i].GetComponentInChildren<IngredientScript>().ReturnProcessName() == "Raw")
            {
                return false;
            }
        }
        return true;
    }

    private void GetIngredients()
    {
        _plateIngredients = new Dictionary<string, string>();
        IngredientScript _ingredientScript;

        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            _ingredientScript = _inventorySlots[i].GetComponentInChildren<IngredientScript>();

            _plateIngredients.Add(_ingredientScript.ReturnIngredientName(), _ingredientScript.ReturnProcessName());
        }
    }

    private GameObject FindPlate()
    {
        if (_plateIngredients.ContainsKey("Eggs") && _plateIngredients.ContainsKey("Milk") && _plateIngredients.ContainsKey("Salmon"))
        {
            if (_plateIngredients["Eggs"] == "Cut" && _plateIngredients["Milk"] == "Blended" && _plateIngredients["Salmon"] == "Cooked") //Salmon Omelette
            {
                return Instantiate(_platePrefab[1]);
            }
        }

        if (_plateIngredients.ContainsKey("Eggs") && _plateIngredients.ContainsKey("Milk") && _plateIngredients.ContainsKey("Apple"))
        {
            if (_plateIngredients["Eggs"] == "Blended" && _plateIngredients["Milk"] == "Blended" && _plateIngredients["Apple"] == "Cut") //Apple Pie
            {
                return Instantiate(_platePrefab[2]);
            }
        }

        if (_plateIngredients.ContainsKey("Eggs") && _plateIngredients.ContainsKey("Milk") && _plateIngredients.ContainsKey("Corn"))
        {
            if (_plateIngredients["Eggs"] == "Blended" && _plateIngredients["Milk"] == "Blended" && _plateIngredients["Corn"] == "Blended") //Plato de pasta
            {
                return Instantiate(_platePrefab[3]);
            }
        }

        if (_plateIngredients.ContainsKey("Eggs") && _plateIngredients.ContainsKey("Milk") && _plateIngredients.ContainsKey("Apple"))
        {
            if (_plateIngredients["Eggs"] == "Blended" && _plateIngredients["Milk"] == "Cooked" && _plateIngredients["Apple"] == "Blended") //Pudin de Manzana
            {
                return Instantiate(_platePrefab[4]);
            }
        }

        if (_plateIngredients.ContainsKey("Eggs") && _plateIngredients.ContainsKey("Fish") && _plateIngredients.ContainsKey("Corn"))
        {
            if (_plateIngredients["Eggs"] == "Cooked" && _plateIngredients["Fish"] == "Cooked" && _plateIngredients["Corn"] == "Cut") //Plato de Salmon
            {
                return Instantiate(_platePrefab[5]);
            }
        }

        if (_plateIngredients.ContainsKey("Apple") && _plateIngredients.ContainsKey("Fish") && _plateIngredients.ContainsKey("Corn"))
        {
            if (_plateIngredients["Apple"] == "Blended" && _plateIngredients["Fish"] == "Cut" && _plateIngredients["Corn"] == "Cooked") //Ceviche de pescado y manzana
            {
                return Instantiate(_platePrefab[6]);
            }
        }

        if (_plateIngredients.ContainsKey("Eggs") && _plateIngredients.ContainsKey("Fish") && _plateIngredients.ContainsKey("Corn"))
        {
            if (_plateIngredients["Eggs"] == "Cut" && _plateIngredients["Fish"] == "Blended" && _plateIngredients["Corn"] == "Blended") //Rollo Primavera
            {
                return Instantiate(_platePrefab[7]);
            }
        }
        return Instantiate(_platePrefab[0]); //Default Plate
    }
}
