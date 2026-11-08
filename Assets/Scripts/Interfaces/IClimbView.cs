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

        // Where the climb-up clip's first frame draws the body, in the player's local space (no lean).
        bool TryGetLedgeClimbStartBody(out Vector3 localPosition);

        // World position of the animated body (e.g. hips).
        Vector3 BodyPosition { get; }

        // 0..1 from the climb-up clip's curves: how planted the hands (on the top) and feet (on the wall) are.
        void GetLedgeClimbIKWeights(out float hands, out float feet);
        
        void SetClimbMove(Vector2 move);

        void SetSlideEffect(bool active, Vector3 surfacePoint);
    }
}
