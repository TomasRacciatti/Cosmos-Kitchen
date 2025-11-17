using UnityEngine;

namespace Audio.Music
{
    [CreateAssetMenu(fileName = "MusicBank", menuName = "ScriptableObject/Audio/MusicBank")]
    public class SoMusicBank : ScriptableObject
    {
        [Header("Planet/Level Music")]
        [SerializeField] private AudioClip eorthMusic;

        [Header("General Music")]
        [SerializeField] private AudioClip[] minigameMusic;
        [SerializeField] private AudioClip kitchenMusic;
        [SerializeField] private AudioClip pauseMusic;
        
        public AudioClip EorthMusic => eorthMusic;
        public AudioClip KitchenMusic => kitchenMusic;
        public AudioClip PauseMusic => kitchenMusic;

        public AudioClip GetRandomMiniGameMusic()
        {
            if (minigameMusic == null || minigameMusic.Length == 0)
            {
                Debug.LogWarning("No minigame music available in MusicBank!");
                return null;
            }
            
            int randomIndex = Random.Range(0, minigameMusic.Length);
            return minigameMusic[randomIndex];
        }
    }
}
