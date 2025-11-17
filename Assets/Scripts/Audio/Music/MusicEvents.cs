using System;
using UnityEngine;

public static class MusicEvents
{
    public enum MusicType
    {
        Minigame,
        Kitchen,
        Pause,
        Eorth
    }
    
    public static event Action<MusicType> OnMusicChangeRequest;
    public static event Action OnMusicResumeRequested;

    public static void RequestMusicChange(MusicType musicType)
    {
        OnMusicChangeRequest?.Invoke(musicType);
    }

    public static void RequestMusicResume()
    {
        OnMusicResumeRequested?.Invoke();
    }
}
