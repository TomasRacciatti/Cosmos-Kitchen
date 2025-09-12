using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClientOrder : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _clientNumber;
    [SerializeField] TextMeshProUGUI _orderDescription;

    public void SetOrder(string text)
    {
        _orderDescription.text = text;
    }

    public void SetClient(string text)
    {
        _clientNumber.text = text;
    }

    public void OrderComplete()
    {
        Destroy(gameObject);
    }

}
