using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Bush : MonoBehaviour
{
    private AudioSource audioSource; 
    [SerializeField] private float fadeSpeed = 10f;

    private bool playerInside = false;
    private Transform player;
    private CharacterController controller;
    private float targetVolume = 0f;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        audioSource.volume = 0f;
        audioSource.loop = true;
        audioSource.Play();
    }

    void Update()
    {
        if (playerInside && controller != null)
        {
            float speed = controller.velocity.magnitude;

            if (speed > 0.1f)
                targetVolume = 1f;
            else
                targetVolume = 0f;
        }
        else
        {
            targetVolume = 0f;
        }
        
        audioSource.volume = Mathf.Lerp(audioSource.volume, targetVolume, Time.deltaTime * fadeSpeed);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CharacterController>(out CharacterController player))
        {
            playerInside = true;
            controller = other.GetComponent<CharacterController>();
            print("Player is inside");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<CharacterController>(out CharacterController player))
        {
            playerInside = false;
            controller = null;
            print("Player is not inside");
        }
    }
}
