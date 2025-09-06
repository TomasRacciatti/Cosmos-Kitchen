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
        movementControlsText.text = "WASD - Movimiento\n" +
                                    "Mouse - Cámara\n" +
                                    "ScrollWheel - Nivel de Cámara";
                                    
        actionControlsText.text = "Click Izquierdo - Abrir Mesas de Trabajo\n" +
                                  "E - Interactuar/Salir";
                                 
        menuControlsText.text = "ESC - Pausa/Menú\n" +
                                "B - Libro de cocina\n" +
                                "Tab - Abrir Inventario";
    }

    public void BackToMainMenu()
    {
        FindObjectOfType<MenuManager>().ShowMainMenu();
    }
}