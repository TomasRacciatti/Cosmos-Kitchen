using System;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [FormerlySerializedAs("masterVolumeSlider")]
    [Header("UI")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle fullscreenToggle;
    
    [Header("Backend")]
    [SerializeField] private PlayerPrefAudioStore_SO store;
    [SerializeField] private AudioMixerApplier_SO applier;
    
    [Header("Safe find")]
    [SerializeField] private MenuManager menuManager;

    private void Awake()
    {
        ConfigureSlider(masterSlider);
        ConfigureSlider(musicSlider);
        ConfigureSlider(sfxSlider);
    }
    
    // Helper para asegurarnos que los valores estend donde los queremos. No es verdaderamente necesario pero  
    // nos ayuda a evitar errores
    private static void ConfigureSlider(Slider s)
    {
        s.minValue = 0.0001f;
        s.maxValue = 1f;
        s.wholeNumbers = false;
    }

    private void Start()
    {
        masterSlider.SetValueWithoutNotify(store.Master);
        musicSlider.SetValueWithoutNotify(store.Music);
        sfxSlider.SetValueWithoutNotify(store.Sfx);

        applier.ApplyAll(store.Master, store.Music, store.Sfx);
        
        fullscreenToggle.isOn = Screen.fullScreen;
        
        masterSlider.onValueChanged.AddListener(OnMasterChanged);
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }
    
    private void OnMasterChanged(float volume) { store.Master = volume; applier.ApplyMaster(volume);}
    private void OnMusicChanged (float volume) { store.Music = volume; applier.ApplyMusic(volume);}
    private void OnSfxChanged   (float volume) { store.Sfx = volume; applier.ApplySfx(volume);}
    
    private void SetFullscreen(bool isFullscreen) => Screen.fullScreen = isFullscreen;

    public void SaveSettings()
    {
        store.Save();
        
        if (menuManager != null) 
            menuManager.ShowMainMenu();
        else // Fallback por las dudas pero no deberia entrar aca si asigno desde el inspector
            FindObjectOfType<MenuManager>()?.ShowMainMenu();
    }
}