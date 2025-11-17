using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio.AudioZones
{
    public class AudioZoneManager : MonoBehaviour
    {
        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer audioMixer;
    
        [Header("Outside Settings")]
        [SerializeField] private float outsideNormalVolume = 0f;
        [SerializeField] private float outsideMuffledVolume = -20f;
        [SerializeField] private float outsideNormalLowpass = 22000f;
        [SerializeField] private float outsideMuffledLowpass = 800f;
    
        [Header("Inside Settings")]
        [SerializeField] private float insideNormalVolume = 0f;
        [SerializeField] private float insideMuffledVolume = -20f;
        [SerializeField] private float insideNormalLowpass = 22000f;
        [SerializeField] private float insideMuffledLowpass = 800f;
    
        [Header("Transition time")]
        [SerializeField] private float transitionDuration = 0.5f;

        private Coroutine _currentTransition;

        private void OnEnable()
        {
            AudioZoneEvents.OnZoneChanged += HandleZoneChange;
        }

        private void OnDisable()
        {
            AudioZoneEvents.OnZoneChanged -= HandleZoneChange;
        }

        private void Start()
        {
            SetZoneImmediate(AudioZoneEvents.Zone.Outside);
        }
        
        private void HandleZoneChange(AudioZoneEvents.Zone newZone)
        {
            if (_currentTransition != null)
            {
                StopCoroutine(_currentTransition);
            }
        
            _currentTransition = StartCoroutine(TransitionToZone(newZone));
        }
        
        private void SetZoneImmediate(AudioZoneEvents.Zone zone)
        {
            if (zone == AudioZoneEvents.Zone.Inside)
            {
                audioMixer.SetFloat("SFXOutsideVolume", outsideMuffledVolume);
                audioMixer.SetFloat("SFXOutsideLowpass", outsideMuffledLowpass);
                audioMixer.SetFloat("SFXInsideVolume", insideNormalVolume);
                audioMixer.SetFloat("SFXInsideLowpass", insideNormalLowpass);
            }
            else
            {
                audioMixer.SetFloat("SFXOutsideVolume", outsideNormalVolume);
                audioMixer.SetFloat("SFXOutsideLowpass", outsideNormalLowpass);
                audioMixer.SetFloat("SFXInsideVolume", insideMuffledVolume);
                audioMixer.SetFloat("SFXInsideLowpass", insideMuffledLowpass);
            }
        }

        private System.Collections.IEnumerator TransitionToZone(AudioZoneEvents.Zone zone)
        {
            float timer = 0f;
            
            audioMixer.GetFloat("SFXOutsideVolume", out float startOutsideVol);
            audioMixer.GetFloat("SFXOutsideLowpass", out float startOutsideLp);
            audioMixer.GetFloat("SFXInsideVolume", out float startInsideVol);
            audioMixer.GetFloat("SFXInsideLowpass", out float startInsideLp);
            
            float targetOutsideVol = zone == AudioZoneEvents.Zone.Inside ? outsideMuffledVolume : outsideNormalVolume;
            float targetOutsideLp = zone == AudioZoneEvents.Zone.Inside ? outsideMuffledLowpass : outsideNormalLowpass;
            float targetInsideVol = zone == AudioZoneEvents.Zone.Inside ? insideNormalVolume : insideMuffledVolume;
            float targetInsideLp = zone == AudioZoneEvents.Zone.Inside ? insideNormalLowpass : insideMuffledLowpass;
        
            while (timer < transitionDuration)
            {
                timer += Time.deltaTime;
                float t = timer / transitionDuration;
            
                audioMixer.SetFloat("SFXOutsideVolume", Mathf.Lerp(startOutsideVol, targetOutsideVol, t));
                audioMixer.SetFloat("SFXOutsideLowpass", Mathf.Lerp(startOutsideLp, targetOutsideLp, t));
                audioMixer.SetFloat("SFXInsideVolume", Mathf.Lerp(startInsideVol, targetInsideVol, t));
                audioMixer.SetFloat("SFXInsideLowpass", Mathf.Lerp(startInsideLp, targetInsideLp, t));
            
                yield return null;
            }
        
            audioMixer.SetFloat("SFXOutsideVolume", targetOutsideVol);
            audioMixer.SetFloat("SFXOutsideLowpass", targetOutsideLp);
            audioMixer.SetFloat("SFXInsideVolume", targetInsideVol);
            audioMixer.SetFloat("SFXInsideLowpass", targetInsideLp);
        
            _currentTransition = null;
        }
    }
}
