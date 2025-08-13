using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverAnimV2 : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Collider[] targetColliders;
    [SerializeField] private AudioClip sound;
    [SerializeField] private string[] GOChildren; // e.g. "Button0", "Button1"

    private Animator animator;
    private Camera FPCamera;
    private int hoveredIndex = -1;

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
        if (FPCamera == null || targetColliders == null)
            return;

        Ray ray = FPCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactableLayer))
        {
            for (int i = 0; i < targetColliders.Length; i++)
            {
                if (hit.collider == targetColliders[i])
                {
                    if (hoveredIndex != i)
                    {
                        ResetPreviousHover();

                        string hoverBool = "Hover" + GOChildren[i]; 
                        animator.SetBool(hoverBool, true);
                        hoveredIndex = i;

                        AudioManager.instance.PlaySFX(sound);
                    }
                    return;
                }
            }
        }

        if (hoveredIndex != -1)
        {
            ResetPreviousHover();
        }
    }

    private void ResetPreviousHover()
    {
        if (hoveredIndex != -1)
        {
            string hoverBool = "Hover" + GOChildren[hoveredIndex];
            animator.SetBool(hoverBool, false);
            hoveredIndex = -1;
        }
    }
}