using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "PlayerPrefsAudioStore", menuName = "Scriptables/Audio/Mixer Applier")]
public class AudioMixerApplier_SO : ScriptableObject, IAudioVolumeApplier
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string masterParam = "MasterVolume";
    [SerializeField] private string musicParam  = "MusicVolume";
    [SerializeField] private string sfxParam    = "SFXVolume";

    public void ApplyMaster(float linear01) => SetDecibels(masterParam, linear01);
    public void ApplyMusic(float linear01) => SetDecibels(musicParam, linear01);
    public void ApplySfx(float linear01) => SetDecibels(sfxParam, linear01);
    
    public void ApplyAll(float master, float music, float sfx)
    {
        ApplyMaster(master);
        ApplyMusic(music);
        ApplySfx(sfx);
    }
    
    private void SetDecibels(string exposedParam, float linear01)
    {
        float v = Mathf.Clamp(linear01, 0.0001f, 1f);
        float dB = Mathf.Log10(v) * 20f;
        mixer.SetFloat(exposedParam, dB);
    }
}
