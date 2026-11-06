using System;

namespace Interfaces
{
    public interface IStamina
    {
        float Current { get; }
        float Max { get; }
        
        bool IsExhausted { get; }
        
        bool IsInUse { get; }

        void Consume(float amount);

        event Action Depleted;

        // (current, max)
        event Action<float, float> Changed;
    }
}
