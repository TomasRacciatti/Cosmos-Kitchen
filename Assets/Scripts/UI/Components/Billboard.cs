using UnityEngine;

namespace UI.Components
{
    public class Billboard : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool onlyHorizontal = false;

        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (!mainCamera) return;

            if (onlyHorizontal)
            {
                Vector3 targetPos = mainCamera.transform.position;
                targetPos.y = transform.position.y;

                transform.LookAt(targetPos);
            }
            else
            {
                transform.LookAt(
                    transform.position + mainCamera.transform.rotation * Vector3.forward,
                    mainCamera.transform.rotation * Vector3.up
                );
            }
        }
    }
}