using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Audio.Music;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Funcionalidad")] public Sound[] sounds;
    public static AudioManager instance;

    [SerializeField] AudioSource _sfxSource;
    [SerializeField] AudioSource musicSource;

    [SerializeField] private PlayerPrefAudioStore_SO store;
    [SerializeField] private AudioMixerApplier_SO applier;

    [Header("Music Management")]
    [SerializeField] private SoMusicBank musicBank;
    [SerializeField] private float musicFadeDuration = 0.2f;

    private Dictionary<string, Sound> _byName;
    
    private Stack<AudioClip> _musicStack = new Stack<AudioClip>();
    private Coroutine _currentMusicTransition;

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

    private void OnEnable()
    {
        MusicEvents.OnMusicChangeRequest += HandleMusicChange;
        MusicEvents.OnMusicResumeRequested += HandleMusicResume;
    }

    private void OnDisable()
    {
        MusicEvents.OnMusicChangeRequest -= HandleMusicChange;
        MusicEvents.OnMusicResumeRequested -= HandleMusicResume;
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
    
    
    // ======== MUSIC ========
    private void HandleMusicChange(MusicEvents.MusicType musicType)
    {
        AudioClip newClip = GetMusicClip(musicType);
        if (newClip == null) return;
        
        if (musicSource.clip != null && musicSource.clip != newClip)
            _musicStack.Push(musicSource.clip);

        bool instant = (musicType == MusicEvents.MusicType.Pause);
        PlayMusic(newClip, instant);
    }

    private void HandleMusicResume()
    {
        if (_musicStack.Count == 0)
        {
            Debug.LogWarning("No previous music to resume.");
            return;
        }

        AudioClip previousClip = _musicStack.Pop();
        PlayMusic(previousClip);
    }

    private AudioClip GetMusicClip(MusicEvents.MusicType musicType)
    {
        switch (musicType)
        {
            case MusicEvents.MusicType.Eorth:
                return musicBank.EorthMusic;
            case MusicEvents.MusicType.Pause:
                return musicBank.PauseMusic;
            case MusicEvents.MusicType.Kitchen:
                return musicBank.KitchenMusic;
            case MusicEvents.MusicType.Minigame:
                return musicBank.GetRandomMiniGameMusic();
            default:
                Debug.LogWarning($"Unknown music type: {musicType}");
                return null;
        }
    }

    private void PlayMusic(AudioClip clip, bool instant = false)
    {
        if (clip == musicSource.clip && musicSource.isPlaying)
            return; 
        
        if (_currentMusicTransition != null)
        {
            StopCoroutine(_currentMusicTransition);
        }

        if (instant)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
        else
        {
            _currentMusicTransition = StartCoroutine(CrossFadeMusic(clip));
        }
    }

    private IEnumerator CrossFadeMusic(AudioClip newClip)
    {
        float timer = 0f;
        float startVolume = musicSource.volume;

        // fade out actual
        while (timer < musicFadeDuration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0, timer / musicFadeDuration);
            yield return null;
        }
        
        // switch de musica
        musicSource.clip = newClip;
        musicSource.Play();
        
        // fade in al nuevo
        timer = 0f;
        while (timer < musicFadeDuration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0, startVolume, timer / musicFadeDuration);
            yield return null;
        }
        
        musicSource.volume = startVolume;
        _currentMusicTransition = null;
    }
}