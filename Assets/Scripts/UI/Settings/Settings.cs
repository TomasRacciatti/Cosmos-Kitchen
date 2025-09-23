using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace UI.Settings
{
    public class Settings : MonoBehaviour
    {
        [Header("OnCloseEvents")]
        [SerializeField] private UnityEvent onClose;

        [Header("Audio")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private IntegerSetting masterVolume;
        [SerializeField] private IntegerSetting musicVolume;
        [SerializeField] private IntegerSetting sfxVolume;

        [Header("Gameplay")]
        [SerializeField] private FloatSetting sensitivity;
        [SerializeField] private BooleanSetting invertYAxis;

        private void Awake()
        {
            if (masterVolume != null)
            {
                //masterVolume.Value = 
                masterVolume.OnValueChanged.AddListener(ApplyMasterVolume);
            }

            if (musicVolume != null)
            {
                musicVolume.OnValueChanged.AddListener(ApplyMusicVolume);
            }

            if (sfxVolume != null)
            {
                sfxVolume.OnValueChanged.AddListener(ApplySfxVolume);
            }

            if (sensitivity != null)
            {
                sensitivity.OnValueChanged.AddListener(ApplySensitivity);
            }

            if (invertYAxis != null)
            {
                invertYAxis.OnValueChanged.AddListener(ApplyInvertYAxis);
            }
        }

        public void CloseSettings()
        {
            onClose?.Invoke();
        }
        
        private void SetVolume(string exposedParam, float value0To100)
        {
            float v = Mathf.Clamp(value0To100, 0f, 100f);
            float linear01 = v / 100f;
            linear01 = Mathf.Max(linear01, 0.0001f);
            float dB = Mathf.Log10(linear01) * 20f;
            audioMixer.SetFloat(exposedParam, dB);
        }
        private void ApplyMasterVolume(int value)
        {
            SetVolume("MasterVolume", value);
        }
        
        private void ApplyMusicVolume(int value)
        {
            SetVolume("MusicVolume", value);
        }

        private void ApplySfxVolume(int value)
        {
            SetVolume("SFXVolume", value);
        }

        private void ApplySensitivity(float value)
        {
            Debug.Log($"Sensitivity: {value}");
        }

        private void ApplyInvertYAxis(bool value)
        {
            Debug.Log($"Invert Y Axis: {value}");
        }
    }
}