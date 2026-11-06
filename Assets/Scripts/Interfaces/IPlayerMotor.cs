using UnityEngine;

namespace Interfaces
{
    // The player's regular walking/falling movement, as seen by other movement modes (e.g. climbing).
    public interface IPlayerMotor
    {
        bool Grounded { get; }
        LayerMask GroundLayers { get; }

        void Launch(Vector3 velocity);
    }
}
