using System;

namespace Interfaces
{
    public interface IStamina
    {
        float Current { get; }
        float Max { get; }

        // True from running out until stamina has recovered enough to be used again.
        bool IsExhausted { get; }

        void Consume(float amount);

        event Action Depleted;

        // (current, max)
        event Action<float, float> Changed;
    }
}
