using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audio
{
    public class SoundInsideTrigger : MonoBehaviour
    {
        private readonly List<GameObject> insideObjects = new();
        private AudioSource audioSource;

        [SerializeField] private float fadeSpeed = 1f;
        [SerializeField] private AudioCue audioCue;
        [SerializeField] private float minPitch = 0.9f;
        [SerializeField] private float maxPitch = 1.6f;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.volume = 0f;
            audioSource.loop = false;
            audioSource.playOnAwake = false;
        }

        private void Update()
        {
            float activity = CalculateActivity();

            if (activity > 0.1f)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = AudioCue.GetRandomClip(audioCue.Clips);
                    audioSource.pitch = Random.Range(minPitch, maxPitch);

                    audioSource.Play();
                }
                
                float targetVolume = Mathf.Clamp01(0.3f + activity * 0.05f);
                audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);
            }
            else
            {
                audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0f, fadeSpeed * Time.deltaTime);

                if (audioSource.volume <= 0.0001f && audioSource.isPlaying)
                    audioSource.Stop();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!insideObjects.Contains(other.gameObject))
                insideObjects.Add(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            insideObjects.Remove(other.gameObject);
        }
        
        private float CalculateActivity()
        {
            insideObjects.RemoveAll(obj => !obj || !obj.activeInHierarchy);

            return insideObjects.Sum(GetObjectSpeed);
        }

        private float GetObjectSpeed(GameObject obj)
        {
            if (obj.TryGetComponent<Rigidbody>(out var rb))
                return rb.velocity.magnitude;

            if (obj.TryGetComponent<CharacterController>(out var cc))
                return cc.velocity.magnitude;

            return 0f;
        }
    }
}