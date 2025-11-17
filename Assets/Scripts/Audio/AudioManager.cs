using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Funcionalidad")] 
    public Sound[] sounds;
    public static AudioManager instance;

    [SerializeField] AudioSource _sfxSource;
    [SerializeField] AudioSource musicSource;

    [SerializeField] private PlayerPrefAudioStore_SO store;
    [SerializeField] private AudioMixerApplier_SO applier;

    private Dictionary<string, Sound> _byName;

    public AudioSource SFXSource => _sfxSource;
    
    
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        _byName = new Dictionary<string, Sound>(StringComparer.Ordinal);
        foreach (var s in sounds) _byName[s.name] = s;

        applier.ApplyAll(store.Master, store.Music, store.Sfx);
        
        StartCoroutine(DelayedApplySavedVolumes());
    }
    
    private IEnumerator DelayedApplySavedVolumes()
    {
        yield return null;
        
        applier.ApplyAll(store.Master, store.Music, store.Sfx);
        
    }

    public void SetAmbiance(AudioClip background)
    {
        musicSource.clip = background;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void Play(string name)
    {
        if (!_byName.TryGetValue(name, out var s))
        {
            Debug.LogWarning($"Audio: {name} not found.");
            return;
        }

        // apply per-sound pitch momentarily, then restore
        float prevPitch = _sfxSource.pitch;
        _sfxSource.pitch = s.pitch;
        _sfxSource.PlayOneShot(s.clip, s.volume);
        _sfxSource.pitch = prevPitch;
    }

    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        float prevPitch = _sfxSource.pitch;
        _sfxSource.pitch = pitch;
        _sfxSource.PlayOneShot(clip, volume);
        _sfxSource.pitch = prevPitch;
    }

    public void StopAllSFX()
    {
        _sfxSource.Stop();
    }
}