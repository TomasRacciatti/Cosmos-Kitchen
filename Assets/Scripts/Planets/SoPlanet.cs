using System.Collections.Generic;
using Characters.Clients;
using Interfaces;
using Items.Core;
using UnityEngine;

namespace Planets
{
    [CreateAssetMenu(menuName = "ScriptableObject/Planet", fileName = "Planet_")]
    public class SoPlanet : ScriptableObject
    {
        [Header("Info")]
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;
        [SerializeField, TextArea(2, 5)] private string description;

        [Header("Travel")]
#if UNITY_EDITOR
        [SerializeField, Tooltip("The planet's scene. It must be enabled in File > Build Settings.")]
        private UnityEditor.SceneAsset scene;
#endif
        [SerializeField, HideInInspector] private string sceneName;
        [SerializeField, Min(0), Tooltip("Golden Novas needed to travel to this planet.")]
        private int requiredNovas;

        [Header("Contents")]
        [SerializeField] private SoItem[] ingredients;
        [SerializeField] private ClientSO[] clients;

        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public string Description => description;
        public string SceneName => sceneName;
        public int RequiredNovas => requiredNovas;
        public IReadOnlyList<SoItem> Ingredients => ingredients;
        public IReadOnlyList<ClientSO> Clients => clients;

        public int MissingNovas(INovaProgress progress) => Mathf.Max(0, requiredNovas - progress.Novas);

        public bool IsUnlocked(INovaProgress progress) => MissingNovas(progress) == 0;

#if UNITY_EDITOR
        // The SceneAsset only exists in the editor, so its name is copied into a field the build keeps.
        private void OnValidate()
        {
            sceneName = scene != null ? scene.name : string.Empty;
            if (scene == null) return;

            var path = UnityEditor.AssetDatabase.GetAssetPath(scene);
            foreach (var buildScene in UnityEditor.EditorBuildSettings.scenes)
                if (buildScene.enabled && buildScene.path == path) return;

            Debug.LogWarning($"[SoPlanet] {name}: scene '{sceneName}' is not enabled in the Build Settings.", this);
        }
#endif
    }
}
