using System;
using Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public sealed class SceneLoader : ISceneLoader
    {
        public bool IsLoading { get; private set; }

        public event Action LoadStarting;

        public SceneLoader()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public bool CanLoad(string sceneName) =>
            !string.IsNullOrEmpty(sceneName) && Application.CanStreamedLevelBeLoaded(sceneName);

        public bool TryLoad(string sceneName)
        {
            if (IsLoading) return false;
            if (!CanLoad(sceneName))
            {
                Debug.LogWarning($"[SceneLoader] Scene '{sceneName}' is missing or not enabled in the Build Settings.");
                return false;
            }

            IsLoading = true;
            LoadStarting?.Invoke();
            SceneManager.LoadSceneAsync(sceneName);
            return true;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => IsLoading = false;
    }
}
