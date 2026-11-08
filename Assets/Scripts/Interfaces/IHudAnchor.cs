using UnityEngine;

namespace Interfaces
{
    // A point on the character that HUD elements follow on screen. Moves with the animated body,
    // unlike the player root (which e.g. teleports onto the ledge at the start of the climb-up).
    public interface IHudAnchor
    {
        Transform HudAnchor { get; }
    }
}
