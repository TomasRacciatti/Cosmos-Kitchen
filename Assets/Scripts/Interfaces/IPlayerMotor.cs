using UnityEngine;

namespace Interfaces
{
    /// <summary>
    /// The player's regular (walking/falling) movement, as seen by other movement modes such as climbing.
    /// </summary>
    public interface IPlayerMotor
    {
        bool Grounded { get; }
        
        void Launch(Vector3 velocity);
    }
}
