using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPrefsAudioStore", menuName = "Scriptables/Audio/PlayerPrefs Store")]
public class PlayerPrefAudioStore_SO : ScriptableObject, IAudioSettingsStore
{
    private const string MASTER_KEY = "MasterVolume";
    private const string MUSIC_KEY  = "MusicVolume";
    private const string SFX_KEY    = "SFXVolume";
    
    // Si el rango es desde 0, se nos va a romper el mixer
    [Range(0.0001f, 1f)] [SerializeField] private float defaultMaster = 1f;
    [Range(0.0001f, 1f)] [SerializeField] private float defaultMusic = 1f;
    [Range(0.0001f, 1f)] [SerializeField] private float defaultSfx = 1f;
    
    public float Master
    {
        get => PlayerPrefs.GetFloat(MASTER_KEY, defaultMaster);
        set => PlayerPrefs.SetFloat(MASTER_KEY, Mathf.Clamp(value, 0.0001f, 1f));
    }

    public float Music
    {
        get => PlayerPrefs.GetFloat(MUSIC_KEY, defaultMusic);
        set => PlayerPrefs.SetFloat(MUSIC_KEY, Mathf.Clamp(value, 0.0001f, 1f));
    }

    public float Sfx
    {
        get => PlayerPrefs.GetFloat(SFX_KEY, defaultSfx);
        set => PlayerPrefs.SetFloat(SFX_KEY, Mathf.Clamp(value, 0.0001f, 1f));
    }

    public void Save() => PlayerPrefs.Save();

}
