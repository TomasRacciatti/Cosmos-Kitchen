using UnityEngine;

public class HoverAnim : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Collider targetCollider;
    [SerializeField] private AudioClip sound;

    private Animator animator;
    private Camera FPCamera;
    private bool isHovered = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        CameraReference.OnCameraAvailable += SetCamera;

        if (CameraReference.Instance != null)
            SetCamera(CameraReference.Instance);
    }

    private void OnDisable()
    {
        CameraReference.OnCameraAvailable -= SetCamera;
    }

    private void SetCamera(Camera cam)
    {
        FPCamera = cam;
    }

    private void Update()
    {
        if (FPCamera == null || targetCollider == null)
            return;

        Ray ray = FPCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactableLayer))
        {
            if (hit.collider == targetCollider)
            {
                if (!isHovered)
                {
                    animator.SetBool("Hover", true);
                    isHovered = true;
                    AudioManager.instance.PlaySFX(sound);
                }
                return;
            }
        }

        if (isHovered)
        {
            animator.SetBool("Hover", false);
            isHovered = false;
        }
    }
}

