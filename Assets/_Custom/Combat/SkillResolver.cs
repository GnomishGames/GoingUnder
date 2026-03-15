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
        }

        // Apply healing to target if applicable
        if (skill.heal > 0)
        {
            target.Hitpoints.ModifyCurrent(skill.heal);
        }

        //subtract stamina and mana costs from skill user
        if (skill.staminaCost > 0)
        {
            skillUser.Stamina.ModifyCurrent(-skill.staminaCost);
        }

        //subtract mana cost from skill user
        if (skill.manaCost > 0)
        {
            skillUser.Mana.ModifyCurrent(-skill.manaCost);
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