using UnityEngine;

public class SkillResolver : MonoBehaviour
{
    // This class is responsible for resolving skill usage including checking for valid targets, applying damage, and triggering any additional effects.
    // It is separate from UI and can be used by any system that needs to resolve skill usage.

    public SkillResult ResolveSkill(CreatureStats skillUser, CreatureStats target, SkillSO skill)
    {
        // Validation checks
        if (skillUser == null || skill == null)
        {
            return new SkillResult(
                wasAttempted: false,
                wasSuccessful: false,
                damageDealt: 0,
                healingDone: 0,

                failureReason: "Missing skill user or skill"
            );
        }

        if (target == null)
        {
            Debug.LogError($"SkillResolver: No target specified for skill usage!");
            return new SkillResult(
                wasAttempted: false,
                wasSuccessful: false,
                damageDealt: 0,
                healingDone: 0,
                failureReason: "No target specified"
            );
        }

        // Is skill user dead?
        if (skillUser.isDead)
        {
            Debug.LogError($"SkillResolver: Skill user {skillUser.interactableName} is dead and cannot use skills!");
            return new SkillResult(
                wasAttempted: false,
                wasSuccessful: false,
                damageDealt: 0,
                healingDone: 0,
                failureReason: "Skill user is dead"
            );
        }

        // Is target dead?
        if (target.isDead)
        {
            Debug.LogError($"SkillResolver: Target {target.interactableName} is already dead! Cannot use skills on a dead target.");
            return new SkillResult(
                wasAttempted: false,
                wasSuccessful: false,
                damageDealt: 0,
                healingDone: 0,
                failureReason: "Target is dead"
            );
        }

        // Apply damage to target if applicable
        if (skill.targetDamage > 0)
        {
            target.Hitpoints.ModifyCurrent(-skill.targetDamage);
            Debug.Log($"{skillUser.interactableName} uses {skill.name} on {target.interactableName} for {skill.targetDamage} damage!");
        }

        // Apply healing to target if applicable
        if (skill.selfHeal > 0)
        {
            target.Hitpoints.ModifyCurrent(skill.selfHeal);
            Debug.Log($"{skillUser.interactableName} uses {skill.name} on {target.interactableName} and heals for {skill.selfHeal} hitpoints!");
        }

        return new SkillResult(
            wasAttempted: true,
            wasSuccessful: true,
            damageDealt: skill.targetDamage,
            healingDone: skill.selfHeal
        );
    }
}