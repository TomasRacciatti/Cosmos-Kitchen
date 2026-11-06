using UnityEngine;

namespace Interfaces
{
    public interface IClimbView
    {
        void ClimbStarted(bool fromGround);
        void ClimbEnded();

        // Becomes false as soon as Climb Start's exit transition begins.
        bool IsPlayingClimbStart { get; }
        
        void SetClimbMove(Vector2 move);

        void SetSlideEffect(bool active, Vector3 surfacePoint);
    }
}
