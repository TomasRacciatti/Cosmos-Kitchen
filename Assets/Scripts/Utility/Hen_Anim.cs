using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hen_Anim : MonoBehaviour
{
    private Animator animator;
    private float timer;

    private float minInterval = 5f;
    public float maxInterval = 8f;
    private string[] idleAnimations = { "Armature|Hen_Peck", "Armature|Hen_LookAround" };

    [Header("State")]
    public bool isSitting = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        ResetTimer();
        animator.Play("Armature|Hen_Idle");
    }

    private void Update()
    {
        if (isSitting)
        {
            animator.Play("Armature|Hen_Sitting");
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            PlayRandomIdleAnimation();
            ResetTimer();
        }
    }

    private void PlayRandomIdleAnimation()
    {
        int index = Random.Range(0, idleAnimations.Length);
        animator.Play(idleAnimations[index]);
    }

    private void ResetTimer()
    {
        timer = Random.Range(minInterval, maxInterval);
    }
}
