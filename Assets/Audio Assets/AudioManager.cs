using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;   
    public static AudioManager instance;

    [SerializeField] AudioSource SFXSource; //NEW
    [SerializeField] AudioSource musicSource;

    void Awake()
    {
        
        if (instance == null)
            instance = this;
        else { Destroy(gameObject); return; }
        
        DontDestroyOnLoad(this);
        
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.spatialBlend = s.spatialBlend;
            s.source.loop = s.loop;
        }
    }

    void Start()
    {
        /*musicSource.clip = background;
        musicSource.Play();
        musicSource.loop = true;*/
        musicSource.volume = 0.5f;
        SFXSource.volume = 0.5f;

        SetSliders();
    }

    public void SetAmbiance(AudioClip background)
    {
        musicSource.clip = background;
        musicSource.Play();
        musicSource.loop = true;
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning( "Audio: " + name + " not found.");
            return;
        }
        s.source.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void StopAllSFX()
    {
        SFXSource.Stop();
    }


    public void SetSliders()
    {
        AudioSliders.instance.musicSlider.onValueChanged.AddListener((v) => {musicSource.volume = v;});
        AudioSliders.instance.SFXSlider.onValueChanged.AddListener((v) => {SFXSource.volume = v;});
        AudioSliders.instance.musicSlider.value = musicSource.volume;
        AudioSliders.instance.SFXSlider.value = SFXSource.volume;
    }
}
