using UnityEngine;

namespace Audio.Music
{
    public class KitchenMusicTrigger : MonoBehaviour
    {
        public void OnEnterKitchen()
        {
            MusicEvents.RequestMusicChange(MusicEvents.MusicType.Kitchen);
        }

        public void OnExitKitchen()
        {
            MusicEvents.RequestMusicResume();
        }
    }
}
