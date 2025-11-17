using UnityEngine;
using Audio.AudioZones;

namespace Audio.Music
{
    public class KitchenMusicTrigger : MonoBehaviour
    {
        public void OnEnterKitchen()
        {
            MusicEvents.RequestMusicChange(MusicEvents.MusicType.Kitchen);
            AudioZoneEvents.ChangeZone(AudioZoneEvents.Zone.Inside);
        }

        public void OnExitKitchen()
        {
            MusicEvents.RequestMusicResume();
            AudioZoneEvents.ChangeZone(AudioZoneEvents.Zone.Outside);
        }
    }
}
