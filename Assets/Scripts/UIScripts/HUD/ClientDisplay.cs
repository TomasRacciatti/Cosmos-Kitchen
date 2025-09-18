using System.Collections;
using System.Collections.Generic;
using Characters.Clients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientDisplay : MonoBehaviour
{
    [Header("Client Info")]
    [SerializeField] TextMeshProUGUI _clientName;
    [SerializeField] Image _clientIcon;
    [SerializeField] Image _clientGrade;

    public void UpdateClientDisplay(ClientController client)
    {
        _clientName.text = client.Name;
        _clientIcon.sprite = client.Icon;
    }
}
