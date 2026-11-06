using UnityEngine;

namespace Interfaces
{
    public interface IClimbView
    {
        void ClimbStarted(bool fromGround);
        void ClimbEnded();
        
        void SetClimbMove(Vector2 move);
    }
}
