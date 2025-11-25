using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTriggerSFX : MonoBehaviour
{
    [SerializeField] AudioClip[] audio;

    public void Play()
    {
        if (audio.Length == 0) return;
        AudioClip clip = audio[Random.Range(0, audio.Length)];
        AudioManager.instance.PlaySFX(clip);
    }
}
