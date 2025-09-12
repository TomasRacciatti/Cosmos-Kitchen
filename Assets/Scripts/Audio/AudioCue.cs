using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(fileName = "AudioCue", menuName = "ScriptableObject/Audio/AudioCue")]
    public class AudioCue : ScriptableObject
    {
        [SerializeField] private AudioClip[] clips;
        
        public AudioClip[] Clips => clips;

        public static AudioClip GetRandomClip(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0) return null;
            return clips[Random.Range(0, clips.Length)];
        }
    }
}
