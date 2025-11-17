using System;

namespace Audio.AudioZones
{
    public static class AudioZoneEvents
    {
        public enum Zone {Outside, Inside}
    
        public static event Action<Zone> OnZoneChanged;

        public static void ChangeZone(Zone newZone)
        {
            OnZoneChanged?.Invoke(newZone);
        }
    }
}
