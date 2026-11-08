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

        // 0..1 through the climb-up clip; 0 until the Animator has entered it.
        float LedgeClimbProgress { get; }

        void PlayLedgeClimb();

        // How far the player must be moved (in its local space) when the climb-up starts, so the clip's
        // first frame lines up with the climbing pose. False if it couldn't be measured.
        bool TryGetLedgeClimbOffset(out Vector3 localOffset);
        
        void SetClimbMove(Vector2 move);

        void SetSlideEffect(bool active, Vector3 surfacePoint);
    }
}
