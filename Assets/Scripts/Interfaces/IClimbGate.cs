namespace Interfaces
{
    // Any component on the PlayerClimbing GameObject can block grabbing walls (e.g. no stamina).
    public interface IClimbGate
    {
        bool AllowsClimbing { get; }
    }
}
