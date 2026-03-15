using UnityEngine;

// This class is responsible for resolving skill usage including checking for valid targets, applying damage, and triggering any additional effects.
// It is separate from UI and can be used by any system that needs to resolve skill usage.

public class SkillResolver : MonoBehaviour
{
    // Resolves the usage of a skill by a skill user on a target. Returns a SkillResult indicating the outcome.
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

        // is target null? This can happen if the player tries to use a skill that requires a target without having one selected, or if the target dies before the skill resolves.
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

        // Check if skill user has enough stamina and mana to use the skill
        if (skillUser.Stamina.Current < skill.staminaCost)
        {
            Debug.LogError($"SkillResolver: {skillUser.interactableName} does not have enough stamina to use {skill.name}!");
            return new SkillResult(
                wasAttempted: false,
                wasSuccessful: false,
                damageDealt: 0,
                healingDone: 0,
                failureReason: "Not enough stamina"
            );
        }

        if (skillUser.Mana.Current < skill.manaCost)
        {
            Debug.LogError($"SkillResolver: {skillUser.interactableName} does not have enough mana to use {skill.name}!");
            return new SkillResult(
                wasAttempted: false,
                wasSuccessful: false,
                damageDealt: 0,
                healingDone: 0,
                failureReason: "Not enough mana"
            );
        }

        // Apply damage to target if applicable
        if (skill.damage > 0)
        {
            target.Hitpoints.ModifyCurrent(-skill.damage);
            Debug.Log($"{skillUser.interactableName} uses {skill.name} on {target.interactableName} for {skill.damage} damage!");
        }

        // Apply healing to target if applicable
        if (skill.heal > 0)
        {
            target.Hitpoints.ModifyCurrent(skill.heal);
            Debug.Log($"{skillUser.interactableName} uses {skill.name} on {target.interactableName} and heals for {skill.heal} hitpoints!");
        }

        //subtract stamina and mana costs from skill user
        if (skill.staminaCost > 0)
        {
            skillUser.Stamina.ModifyCurrent(-skill.staminaCost);
            Debug.Log($"{skillUser.interactableName} pays {skill.staminaCost} stamina to use {skill.name}.");
        }

        if (skill.manaCost > 0)
        {
            skillUser.Mana.ModifyCurrent(-skill.manaCost);
            Debug.Log($"{skillUser.interactableName} pays {skill.manaCost} mana to use {skill.name}.");
        }

        return new SkillResult(
            wasAttempted: true,
            wasSuccessful: true,
            damageDealt: skill.damage,
            healingDone: skill.heal,
            staminaUsed: skill.staminaCost,
            manaUsed: skill.manaCost
        );
    }
}