using Interfaces;
using Managers;
using UI;
using UnityEngine;

namespace Characters.Player
{
    public class InteractComponent : MonoBehaviour
    {
        [SerializeField] private LayerMask interactableLayers;
        [SerializeField] private float cameraRadius = 10f;
        [SerializeField] private float ownerRadius = 5f;

        private IInteractable activeInteractable;
        private Camera mainCamera;

        public void Interact()
        {
            activeInteractable?.Interact(gameObject);
        }

        private void Update()
        {
            DetectInteractable();
        }

        private void DetectInteractable()
        {
            if (mainCamera == null) mainCamera = GameManager.Player.MainCamera; // ta raro esto
            if (mainCamera == null) return;
            
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
            IInteractable newInteractable = null; //new

            if (Physics.Raycast(ray, out var hit, cameraRadius, interactableLayers))
            {
                if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
                {
                    float distance = Vector3.Distance(transform.position, hit.collider.transform.position);
                    if (distance <= ownerRadius) //new
                    {                            //new
                        newInteractable = interactable; //new
                    }                            //new

                    //old code removed:
                    //if (distance > ownerRadius)
                    //{
                    //    ClearActiveInteractable();
                    //    return;
                    //}
                    //if (activeInteractable == interactable) return;
                    //ClearActiveInteractable();
                    //activeInteractable = interactable;
                    //if (activeInteractable == null) return;
                    //activeInteractable?.EnableInteract();
                    //InteractButton.Show();
                    //CrosshairManager.StartAnimate();
                    //return;
                }
            }

            //new
            if (newInteractable != activeInteractable)
            {
                ClearActiveInteractable();

                if (newInteractable != null)
                {
                    activeInteractable = newInteractable;
                    activeInteractable.EnableInteract();
                    InteractButton.Show();
                    CrosshairManager.StartAnimate();
                }
                else
                {
                    CrosshairManager.StopAnimate();
                }
            }
            //new
            
            //old code removed:
            //ClearActiveInteractable();
            //CrosshairManager.StopAnimate();
        }

        private void ClearActiveInteractable()
        {
            if (activeInteractable == null) return;
            activeInteractable.DisableInteract();
            InteractButton.Hide();
            activeInteractable = null;
        }
    }
}