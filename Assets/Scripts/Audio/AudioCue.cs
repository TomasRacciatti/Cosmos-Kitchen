using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioCue", menuName = "Scriptables/Audio/AudioCue")]
public class AudioCue : ScriptableObject
{
    [SerializeField] private AudioClip[] clips;

    public AudioClip GetRandomClip()
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }
}
