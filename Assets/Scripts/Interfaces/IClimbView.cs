using UnityEngine;

namespace Interfaces
{
    public interface IClimbView
    {
        void ClimbStarted(bool fromGround);
        void ClimbEnded();

        /// <summary>True while the Climb Start animation is playing (until its exit transition begins).</summary>
        bool IsPlayingClimbStart { get; }
        
        void SetClimbMove(Vector2 move);
    }
}
