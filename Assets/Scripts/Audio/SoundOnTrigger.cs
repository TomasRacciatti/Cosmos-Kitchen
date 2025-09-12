using Audio;
using Managers;
using UnityEngine;

namespace Features
{
    public class SoundOnTrigger : MonoBehaviour
    {
        private AudioSource audioSource; 
        [SerializeField] private float fadeSpeed = 10f;
        [SerializeField] private AudioCue audioCue;
        [SerializeField] private float minPitch = 0.9f;
        [SerializeField] private float maxPitch = 1.7f;

        private bool playerInside = false;
        private Transform player;
        private CharacterController controller;
        private float targetVolume = 0f;

        void Start()
        {
            if (audioSource == null) audioSource = GetComponent<AudioSource>();

            audioSource.volume = 0f;
            audioSource.loop = true;
        }

        void Update()
        {
            if (playerInside && controller != null)
            {
                float speed = controller.velocity.magnitude;
                
                targetVolume = speed > 0.1f ? 1f : 0f;
                
                audioSource.pitch = Random.Range(minPitch, maxPitch); // Dejo esto para variar pitch
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
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<CharacterController>(out CharacterController player))
            {
                playerInside = false;
                controller = null;
            }
        }
    }
}