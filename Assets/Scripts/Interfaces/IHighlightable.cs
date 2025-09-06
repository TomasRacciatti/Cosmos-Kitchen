using UnityEngine;

namespace Interfaces
{
    public interface IHighlightable
    {
        void EnableHighlight();
    
        void DisableHighlight();
    }

    public interface IHighlightableParameters
    {
        void SetHighlightStrength(float strength);
    
        void SetHighlight(Color color);
    }
}