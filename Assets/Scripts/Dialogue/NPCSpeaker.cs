using System;
using UnityEngine;

public class NPCSpeaker : MonoBehaviour
{
    [Header("Ratio de blips")]
    [Range(0f, 1f)] public float ratio = 0.5f;
    public float gapSeconds = 0.08f;

    [Header("Pitch")]
    public float basePitch = 1f;
    [Range(0f, 1f)] public float pitchJitter = 0.0f;

    // Eventos
    public event Action<NPCSpeaker> OnSpeakStart;
    public event Action<NPCSpeaker> OnBlip;
    public event Action<NPCSpeaker> OnSpeakEnd;

    // SpeakerManager calls
    public void RaiseSpeakStart() => OnSpeakStart?.Invoke(this);
    public void RaiseBlip()       => OnBlip?.Invoke(this);
    public void RaiseSpeakEnd()   => OnSpeakEnd?.Invoke(this);
}

