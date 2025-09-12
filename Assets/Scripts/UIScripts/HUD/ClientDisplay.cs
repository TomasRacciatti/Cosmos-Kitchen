using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientDisplay : MonoBehaviour
{
    [Header("Client Info")]
    [SerializeField] TextMeshProUGUI _clientName;
    [SerializeField] Image _clientIcon;
    [SerializeField] Image _clientGrade;

    public void UpdateClientDisplay(ClientScript client)
    {
        _clientName.text = client.ReturnClientName();
        _clientIcon.sprite = client.ReturnClientIcon();
        _clientGrade.fillAmount = (float)client.ReturnQualityLevel() / 100;
    }
}
