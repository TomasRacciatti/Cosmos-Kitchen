using System;
using UnityEngine;

namespace Audio.Music
{
    public class LevelMusicInitializaer : MonoBehaviour
    {
        [SerializeField] private MusicEvents.MusicType levelMusic = MusicEvents.MusicType.Eorth;

        private void Start()
        {
            MusicEvents.RequestMusicChange(levelMusic);
        }
    }
}
