using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] GameObject _ingredientPrefab;
    [SerializeField] MonoBehaviour _minigameBehaviour;

    private IMinigame _minigame;
    private bool _interactionEnabled = false;

    private void Awake()
    {
        _minigame = _minigameBehaviour as IMinigame;
        if (_minigame == null)
            Debug.LogError("El componente no implementa IMinigame.");
    }

    private void Update()
    {
        if (_interactionEnabled && Input.GetKeyDown(KeyCode.E) && InputManager._instance._canInteract)
        {
            Debug.Log("Activando minijuego...");
            _minigame.StartMinigame(OnMinigameSuccess, OnMinigameFailure);
            InventoryManager._instance.CloseAll();
            InputManager._instance.StopAll();
            InputManager._instance.StopInput();
            HideInteractionButton();
        }
    }

    private void OnTriggerEnter(Collider _other)
    {
        if (_other.GetComponent<PlayerMovement>()) ShowInteractionButton();
    }

    private void OnTriggerExit(Collider _other)
    {
        if (_other.GetComponent<PlayerMovement>()) HideInteractionButton();
    }

    private void OnMinigameSuccess()
    {
        InputManager._instance.ResumeAll();
        InputManager._instance.ResumeInput();
        GameObject ingredient = Instantiate(_ingredientPrefab);
        ingredient.GetComponent<IngredientScript>().SetIngredientQuality(Random.Range(0, 6));
        string ingredientName = ingredient.GetComponent<IngredientScript>().ReturnIngredientName();
        DialogueManager._instance.Notify("You got " + ingredientName + "!!!");
        InventoryManager._instance.Add(ingredient);
        ShowInteractionButton();
    }

    private void OnMinigameFailure()
    {
        InputManager._instance.ResumeAll();
        InputManager._instance.ResumeInput();
        Debug.Log("Minigame Failure");
        ShowInteractionButton();
    }

    private void ShowInteractionButton()
    {
        DialogueManager._instance.ShowInteraction();
        _interactionEnabled = true;
    }

    private void HideInteractionButton()
    {
        DialogueManager._instance.HideInteraction();
        _interactionEnabled = false;
    }
}