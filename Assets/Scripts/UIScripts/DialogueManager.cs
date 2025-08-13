using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager _instance;
    [SerializeField] GameObject _dialoguePanel;
    [SerializeField] GameObject _plateReceiver;
    [SerializeField] GameObject _interactionButton;
    [SerializeField] GameObject _notification;
    [SerializeField] GameObject _deliverButton;
    [SerializeField] GameObject _retryButton;
    [SerializeField] Image _clientPicture;
    [SerializeField] TextMeshProUGUI _notificationText;
    [SerializeField] TextMeshProUGUI _dialogueMesh;
    [SerializeField] TextMeshProUGUI _clientName;
    private ClientScript _clientScript;
    private PlateScript _plate;
    private bool _isShowing;

    void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else { Destroy(this); }

        _isShowing = false;

    }

    public void GetPlate()
    {
        if (_plateReceiver.GetComponentInChildren<PlateScript>())
        {
            _plate = _plateReceiver.GetComponentInChildren<PlateScript>();
            _clientScript.GetPlate(_plate);
        }
    }

    public void CorrectPlate()
    {
        _clientScript._deliveryComplete = true;
        Destroy(_plateReceiver.GetComponentInChildren<PlateScript>().gameObject);
    }

    public void ChangeDialogue(string _dialogue)
    {
        _dialogueMesh.text = _dialogue;
    }

    public void ShowInteraction()
    {
        _interactionButton.SetActive(true);
    }

    public void HideInteraction()
    {
        _interactionButton.SetActive(false);
    }
    public void OpenDialogue()
    {
        InputManager._instance.LockMovement();
        InputManager._instance.StopPlayerMovement();
        InputManager._instance.StopCamera();
        InputManager._instance.UnlockMouse();
        _dialoguePanel.SetActive(true);
        _isShowing = true;
    }

    public void CloseDialogue()
    {
        ClosePlateReceiver();
        _dialoguePanel.SetActive(false);
        _isShowing = false;
        InputManager._instance.UnlockMovement();
        InputManager._instance.ResumePlayerMovement();
        InputManager._instance.ResumeCamera();
        InputManager._instance.LockMouse();
        if (InventoryManager._instance._inventoryOpen)
        {
            InventoryManager._instance.ToggleInventory();
        }
    }

    public void ShowDeliveryButton()
    {
        _deliverButton.SetActive(true);
        OpenPlateReceiver();
    }

    public void ShowRetryButton()
    {
        _retryButton.SetActive(true);
    }

    public void HideDeliveryButton()
    {
        _deliverButton.SetActive(false);
    }

    public void SwitchDialogue()
    {
        if (_isShowing)
        {
            InputManager._instance.TurnOnMouselock();
            CloseDialogue();
            ShowInteraction();
        }
        else
        {
            InputManager._instance.TurnOffMouselock();
            OpenDialogue();
            HideInteraction();
        }
    }

    public void Retry()
    {
        _retryButton.SetActive(false);
        ShowDeliveryButton();
        _clientScript.Retry();
    }

    public void OpenPlateReceiver()
    {
        _plateReceiver.SetActive(true);
    }

    public void ClosePlateReceiver()
    {
        _plateReceiver.SetActive(false);
        HideDeliveryButton();
    }

    public void SetClient(ClientScript _client)
    {
        _clientScript = _client;
        _clientName.text = _client.ReturnClientName();
        _clientPicture.sprite = _client.ReturnClientIcon();
    }

    public void Notify(string text)
    {
        _notificationText.text = text;
        StartCoroutine(Notification());
    }

    IEnumerator Notification()
    {
        _notification.SetActive(true);
        yield return new WaitForSeconds(4f);
        _notification.SetActive(false);
    }

}
