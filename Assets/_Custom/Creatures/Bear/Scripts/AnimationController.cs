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
        PlayIdle();
    }

    public void PlayIdle()
    {
        animator.Play(Idle1.name);
    }

    public void PlayAttack()
    {
        animator.Play(Attack1.name);
    }

    public void PlayDeath()
    {
        animator.Play(Death1.name);
    }
}
