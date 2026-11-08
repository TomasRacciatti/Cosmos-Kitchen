using UnityEngine;

namespace Interfaces
{
    public interface IClimbView
    {
        void ClimbStarted(bool fromGround);
        void ClimbEnded();

        // Become false as soon as the state's exit transition begins.
        bool IsPlayingClimbStart { get; }
        bool IsPlayingLedgeClimb { get; }

        void PlayLedgeClimb();
        
        void SetClimbMove(Vector2 move);

        void SetSlideEffect(bool active, Vector3 surfacePoint);
    }
}
