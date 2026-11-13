using System;

namespace Interfaces
{
    public interface ISceneLoader
    {
        bool IsLoading { get; }

        // Raised right before the current scene starts unloading, while its objects are still intact.
        event Action LoadStarting;

        bool CanLoad(string sceneName);

        bool TryLoad(string sceneName);
    }
}
