using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Characters.Player;

namespace Utility
{
    public class SceneTeleport : MonoBehaviour
    {
        [SerializeField] private bool activateCursor;
        
        [SerializeField] private string sceneName;
        [SerializeField] private TextMeshProUGUI textMesh;

        private void Awake()
        {
            if (textMesh != null)
                textMesh.SetText("Go to: " + sceneName);
        }

        public void Teleport()
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
                
                if (activateCursor)
                    PlayerInputs.SetCursor(true);
                else
                    PlayerInputs.SetCursor(false);
            }
            else
            {
                Debug.LogWarning("No se asignó un nombre de escena en el inspector.");
            }
        }
    }
}
