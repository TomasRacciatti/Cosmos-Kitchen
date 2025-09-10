using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utility
{
    public class SceneTeleport : MonoBehaviour
    {
        [SerializeField] private string sceneName;
        [SerializeField] private TextMeshProUGUI textMesh;

        private void Awake()
        {
            textMesh.SetText("Go to: " + sceneName);
        }

        public void Teleport()
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogWarning("No se asignó un nombre de escena en el inspector.");
            }
        }
    }
}
