namespace Interfaces
{
    // Lets cutscenes park the player in place: no gravity or movement while suspended.
    public interface ISuspendableMotor
    {
        void SetSuspended(bool suspended);
    }
}
