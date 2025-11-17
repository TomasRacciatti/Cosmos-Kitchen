using UnityEngine;

namespace Audio.Music
{
    [CreateAssetMenu(fileName = "MusicBank", menuName = "ScriptableObject/Audio/MusicBank")]
    public class MusicBank : ScriptableObject
    {
        [Header("Level Music")]
        [SerializeField] private AudioClip eorthMusic;

        [Header("Minigame Music")]
        [SerializeField] private AudioClip[] minigameMusic;

        [Header("Location Music")]
        [SerializeField] private AudioClip kitchenMusic;
        
        public AudioClip EorthMusic => eorthMusic;
        public AudioClip KitchenMusic => kitchenMusic;

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
