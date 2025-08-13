using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutDownMinigame : MonoBehaviour
{
    [SerializeField] GameObject _ingredientPrefab;
    private bool _interactionEnabled = false;
    
    [Header("SFX")]
    [SerializeField] AudioClip EnterSound;
    [SerializeField] AudioClip FinishSound;

    private void Update()
    {
        if (_interactionEnabled)
        {
            if (Input.GetKeyDown(KeyCode.E) && InputManager._instance._canInteract)
            {
                
                AudioManager.instance.PlaySFX(FinishSound); 
                Interaction();
            }
        }
    }
    private void OnTriggerEnter(Collider _other)
    {
        AudioManager.instance.PlaySFX(EnterSound); 
        if (_other.gameObject.GetComponent<PlayerMovement>())
        {
            ShowInteractionButton();
        }
    }
    private void OnTriggerExit(Collider _other)
    {
        if (_other.gameObject.GetComponent<PlayerMovement>())
        {
            HideInteractionButton();
        }
    }

    void Interaction()
    {
        GameObject _ingredient = Instantiate(_ingredientPrefab);
        _ingredient.GetComponent<IngredientScript>().SetIngredientQuality(Random.Range(0,6));
        string ingredientName = _ingredient.GetComponent<IngredientScript>().ReturnIngredientName();
        InventoryManager._instance.Add(_ingredient);
        DialogueManager._instance.Notify("You got " + ingredientName + "!!!");
    }

    void ShowInteractionButton()
    {
        DialogueManager._instance.ShowInteraction();
        _interactionEnabled = true;
    }

    void HideInteractionButton()
    {
        DialogueManager._instance.HideInteraction();
        _interactionEnabled = false;
    }
}
