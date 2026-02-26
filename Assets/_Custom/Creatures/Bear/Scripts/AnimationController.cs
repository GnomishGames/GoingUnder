using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    
    [Header("Idle Animations")]
    [SerializeField] private AnimationClip Idle1;
    [SerializeField] private AnimationClip Idle2;

    [Header("Roar Animations")]
    [SerializeField] private AnimationClip Roar1;
    [SerializeField] private AnimationClip Roar2;

    [Header("Attack Animations")]
    [SerializeField] private AnimationClip Attack1;
    [SerializeField] private AnimationClip Attack2;

    [Header("Death Animations")]
    [SerializeField] private AnimationClip Death1;
    [SerializeField] private AnimationClip Death2;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // For testing purposes, trigger animations with number keys
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            animator.Play(Idle1.name);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            animator.Play(Attack1.name);
            // go back to idle after attack animation finishes
            Invoke("PlayIdle", Attack1.length);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            animator.Play(Attack2.name);
            // go back to idle after attack animation finishes
            Invoke("PlayIdle", Attack2.length);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            animator.Play(Idle2.name);
        }
    }

    private void PlayIdle()
    {
        animator.Play(Idle1.name);
    }
}
