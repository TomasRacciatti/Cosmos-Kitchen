using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NavigationPlanetUpdate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] GameObject _planetPopUp;

    [Header("Popup Info")]
    [SerializeField] string _planetName;
    [SerializeField] string _planetDescription;
    [SerializeField] Sprite _planetIcon;

    private void ShowPopUp()
    {
        _planetPopUp.SetActive(true);
    }

    private void HidePopUp()
    {
        _planetPopUp.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowPopUp();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HidePopUp();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        NavigationPanelManager._instance.UpdatePlanetInfo(_planetName, _planetDescription, _planetIcon);
    }
}
