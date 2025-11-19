using UnityEngine;
using TMPro;

public class ControlsManager : MonoBehaviour
{
    [Header("Textos de controles")]
    [SerializeField] private TMP_Text movementControlsText;
    [SerializeField] private TMP_Text actionControlsText;
    [SerializeField] private TMP_Text menuControlsText;

    private void Start()
    {
        // Actualizar los textos
        movementControlsText.text = "WASD - Movement\n" +
                                    "Mouse - Camera";
                                /*+ "ScrollWheel - Nivel de Cámara";*/
                                    
        actionControlsText.text = "E - Interact / Exit\n" + 
                                  "Shift + L-Click - Split One Item\n" +
                                  "Ctrl + L-Click - Split Half\n" +
                                  "Right Click - Send Item to Slot";
                                 
        menuControlsText.text = "Tab - Open Inventory\n" +
                                "B - Cookbook\n" +
                                "O - Order List\n" +
                                "ESC - Pause Menu" ;
    }

    public void BackToMainMenu()
    {
        FindObjectOfType<MenuManager>().ShowMainMenu();
    }
}