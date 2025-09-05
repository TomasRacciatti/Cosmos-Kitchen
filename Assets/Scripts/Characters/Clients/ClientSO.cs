using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Client", fileName = "Client_")]
public class ClientSO : ScriptableObject
{
    [Header("Client")]
    public string _clientName;
    public Sprite _clientIcon;
    public int _clientNumber;
    public bool _isCritic;

    [Header("Dialogue")]
    public string _askingDialogue;
    public string _deliveredDialogue;
    public string _perfectDialogue;
    public string _repeatingDialogue;
    public string _wrongDialogue;

    [Header("Plate Required")]
    public IngredientScript[] _plateIngredientsScripts;

    [Header("Order Description")]
    public string _orderDescription;
}
