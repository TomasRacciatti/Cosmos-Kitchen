using UnityEngine;

public class ShipAnimTester : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SetAnimationState("Idle");

        if (Input.GetKeyDown(KeyCode.T))
            SetAnimationState("TakeOff");

        if (Input.GetKeyDown(KeyCode.Y))
            SetAnimationState("Land");
    }

    void SetAnimationState(string activeState)
    {
        animator.SetBool("Idle", activeState == "Idle");
        animator.SetBool("TakeOff", activeState == "TakeOff");
        animator.SetBool("Land", activeState == "Land");
    }
}