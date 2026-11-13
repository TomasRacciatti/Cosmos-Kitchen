namespace Interfaces
{
    public interface ISceneLoader
    {
        bool IsLoading { get; }

        bool CanLoad(string sceneName);

        bool TryLoad(string sceneName);
    }
}
