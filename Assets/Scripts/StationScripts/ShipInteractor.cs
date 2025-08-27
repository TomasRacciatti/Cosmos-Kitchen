using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShipInteractor : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private bool _active;
    private bool _isPlayerMovement;
    private PlayerViewer _playerViewer;
    
    [Header("SFX")]
    [SerializeField] AudioClip EnterShipSound;
    [SerializeField] GameObject SignExit;

    private void Start()
    {
        _active = false;
        _isPlayerMovement = true;
        SignExit.SetActive(false);
    }

    private void Update()
    {
        if (_active)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (InputManager._instance._canInteract)
                {
                    AudioSource source = gameObject.AddComponent<AudioSource>();
                    source.clip = EnterShipSound;
                    source.Play();
                    StartCoroutine(FadeOutSFX(source, 1.35f));
                    
                    Interact();
                }
            }
        }
    }
    
    private IEnumerator FadeOutSFX(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        source.Stop();
        AudioManager.instance.StopAllSFX();
        source.volume = startVolume;
    }

    private void OnTriggerEnter(Collider _other)
    {
        _playerViewer = _other.GetComponent<PlayerViewer>();
        if (_other.GetComponent<PlayerViewer>())
        {
            DialogueManager._instance.ShowInteraction();
            _active = true;
        }
    }

    private void OnTriggerExit(Collider _other)
    {
        if (_other.GetComponent<PlayerViewer>())
        {
            DialogueManager._instance.HideInteraction();
            _active = false;
        }
    }

    private void Interact()
    {
        CameraMovement._instance.SwitchCamera();
        SwitchPlayerMovement();
        print(_isPlayerMovement);
    }

    private void SwitchPlayerMovement()
    {
        if (_isPlayerMovement)
        {
            DialogueManager._instance.HideInteraction();
            _playerViewer.HidePlayer();
            InputManager._instance.StopPlayerMovement();
            InputManager._instance.LockMovement();
            InputManager._instance.TurnOffMouselock();
            PanelManager._instance.OpenFirstPerson();
            _isPlayerMovement = false;
            _active = false;
        }
        else
        {
            DialogueManager._instance.ShowInteraction();
            InputManager._instance.UnlockMovement();
            InputManager._instance.ResumePlayerMovement();
            InputManager._instance.ResumeCamera();
            InputManager._instance.TurnOnMouselock();
            InventoryManager._instance.CloseAll();
            PanelManager._instance.CloseFirstPerson();
            _isPlayerMovement = true;
            _active = true;
            _playerViewer.ShowPlayer();
        }
        SignExit.SetActive(false);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = EnterShipSound;
        source.Play();
        StartCoroutine(FadeOutSFX(source, 1.35f));
        
        Interact();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        SignExit.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SignExit.SetActive(false);
    }
}
