using UnityEngine;

public class NPCAnimatorController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            SetAnimationState("Idle");

        if (Input.GetKeyDown(KeyCode.W))
            SetAnimationState("Walk");

        if (Input.GetKeyDown(KeyCode.E))
            SetAnimationState("Sitting");
    }

    void SetAnimationState(string activeState)
    {
        animator.SetBool("Idle", activeState == "Idle");
        animator.SetBool("Walk", activeState == "Walk");
        animator.SetBool("Sitting", activeState == "Sitting");
    }
}