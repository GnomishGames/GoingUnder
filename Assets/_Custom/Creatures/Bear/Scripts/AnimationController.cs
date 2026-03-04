using System.Collections;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    private CreatureStats creatureStats;

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
        creatureStats = GetComponent<CreatureStats>();
        PlayIdle(Idle1);
    }

    public void PlayIdle(AnimationClip animationClip = null)
    {
        creatureStats.CheckIfDead(); // Check if we're dead before trying to play an idle animation
        if (creatureStats.isDead)
            return;

        if (animationClip == null)
        {
            // Randomly select one of the idle animations
            animationClip = Random.value < 0.5f ? Idle1 : Idle2;
        }
        animator.Play(animationClip.name);
    }

    public void PlayAttack(AnimationClip animationClip = null)
    {
        creatureStats.CheckIfDead(); // Check if we're dead before trying to play an attack animation
        if (creatureStats.isDead)
            return;
            
        if (animationClip == null)
        {
            // Randomly select one of the attack animations
            animationClip = Random.value < 0.5f ? Attack1 : Attack2;
        }
        animator.Play(animationClip.name);
        StartCoroutine(ReturnToIdleAfterAttack(animationClip.length)); // Return to idle after attack
    }

    public void PlayDeath(AnimationClip animationClip = null)
    {
        if (animationClip == null)
        {
            // Randomly select one of the death animations
            animationClip = Random.value < 0.5f ? Death1 : Death2;
        }
        animator.Play(animationClip.name);
    }

    private IEnumerator ReturnToIdleAfterAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayIdle();
    }
}
