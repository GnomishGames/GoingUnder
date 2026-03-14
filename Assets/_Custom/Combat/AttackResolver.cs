using UnityEngine;

// Handles all combat resolution logic including attack rolls, damage calculation, and damage application.
// This is separate from UI and can be used by any system that needs to resolve attacks.

public class AttackResolver : MonoBehaviour
{
    // Resolves an attack from an attacker to a target using a specific weapon
    public AttackResult ResolveAttack(CreatureStats attacker, CreatureStats target, WeaponSO weapon)
    {
        // Validation checks
        if (attacker == null || weapon == null)
        {
            return new AttackResult(
                wasAttempted: false,
                wasHit: false,
                attackRoll: 0,
                targetAC: target != null ? target.armorClass : 0,
                damageDealt: 0,
                failureReason: "Missing attacker or weapon"
            );
        }

        // is target null? This can happen if the player tries to attack without having a target selected, or if the target dies before the attack resolves.
        if (target == null)
        {
            Debug.LogError($"AttackResolver: No target specified for attack!");
            return new AttackResult(
                wasAttempted: false,
                wasHit: false,
                attackRoll: 0,
                targetAC: 0,
                damageDealt: 0,
                failureReason: "No target specified"
            );
        }

        // Is attacker dead?
        if (attacker.isDead)
        {
            return new AttackResult(
                wasAttempted: false,
                wasHit: false,
                attackRoll: 0,
                targetAC: target != null ? target.armorClass : 0,
                damageDealt: 0,
                failureReason: "Attacker is dead"
            );
        }

        // Is target dead?
        if (target.isDead)
        {
            return new AttackResult(
                wasAttempted: false,
                wasHit: false,
                attackRoll: 0,
                targetAC: target != null ? target.armorClass : 0,
                damageDealt: 0,
                failureReason: "Target is already dead"
            );
        }

        // Is it attacker's turn?
        if (!attacker.inCombat)
        {
            return new AttackResult(
                wasAttempted: false,
                wasHit: false,
                attackRoll: 0,
                targetAC: target != null ? target.armorClass : 0,
                damageDealt: 0,
                failureReason: "It's not the attacker's turn"
            );
        }

        // Perform attack roll
        int toHit = attacker.AttackRoll();
        int targetAC = target != null ? target.armorClass : 0;
        bool isHit = toHit >= targetAC;

        int damageDealt = 0;
        if (isHit)
        {
            damageDealt = CalculateDamage(attacker, weapon);
            if (target != null)
            {
                target.Hitpoints.ModifyCurrent(-damageDealt);
            }
        }

        // invoke death event
        target.CheckIfDead();

        return new AttackResult(
            wasAttempted: true,
            wasHit: isHit,
            attackRoll: toHit,
            targetAC: targetAC,
            damageDealt: damageDealt
        );
    }

    // Calculates damage for an attack based on the weapon and attacker stats
    // Can be extended to include modifiers, critical hits, resistances, etc.
    private int CalculateDamage(CreatureStats attacker, WeaponSO weapon)
    {
        int damage = 0;

        // Roll weapon damage dice
        for (int i = 0; i < weapon.DieMultiplier; i++)
        {
            damage += Random.Range(1, weapon.Die + 1);
        }

        // Add weapon bonus
        damage += weapon.DieBonus;

        // TODO: Can add attacker modifiers here (strength bonus, etc.)
        // Example: damage += (int)attacker.strengthModifier;

        return Mathf.Max(1, damage); // Minimum 1 damage
    }
}
