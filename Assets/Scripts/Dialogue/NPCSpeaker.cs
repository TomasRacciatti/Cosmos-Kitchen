using System;
using UnityEngine;

public class NPCSpeaker : MonoBehaviour
{
    [Header("Identity & Lines")]
    public string npcName = "NPC";
    public DialogLine[] lines;

    [Header("Talk Blip Behavior (per-NPC)")]
    [Range(0f, 1f)] public float ratio = 0.5f;   // blips per letter (scaled)
    public float gapSeconds = 0.08f;

    [Header("Pitch (per-NPC)")]
    public float basePitch = 1f;
    [Range(0f, 1f)] public float pitchJitter = 0.0f;

    // Events (per-NPC)
    public event Action<NPCSpeaker> OnSpeakStart;
    public event Action<NPCSpeaker> OnBlip;
    public event Action<NPCSpeaker> OnSpeakEnd;

    // SpeakerManager calls these:
    public void RaiseSpeakStart() => OnSpeakStart?.Invoke(this);
    public void RaiseBlip()       => OnBlip?.Invoke(this);
    public void RaiseSpeakEnd()   => OnSpeakEnd?.Invoke(this);
}

