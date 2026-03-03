using UnityEngine;

/// <summary>
/// Handles all combat resolution logic including attack rolls, damage calculation, and damage application.
/// This is separate from UI and can be used by any system that needs to resolve attacks.
/// </summary>
public class CombatResolver : MonoBehaviour
{
    /// <summary>
    /// Resolves an attack from an attacker to a target using a specific weapon
    /// </summary>
    public AttackResult ResolveAttack(CreatureStats attacker, CreatureStats target, WeaponSO weapon)
    {
        // Validation checks
        if (attacker == null || target == null || weapon == null)
        {
            return new AttackResult(
                wasAttempted: false,
                wasHit: false,
                attackRoll: 0,
                targetAC: (int)target.armorClass,
                damageDealt: 0,
                failureReason: "Missing attacker, target, or weapon"
            );
        }

        // Is attacker dead?
        if (attacker.isDead)
        {
            return new AttackResult(
                wasAttempted: false,
                wasHit: false,
                attackRoll: 0,
                targetAC: (int)target.armorClass,
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
                targetAC: (int)target.armorClass,
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
                targetAC: (int)target.armorClass,
                damageDealt: 0,
                failureReason: "It's not the attacker's turn"
            );
        }

        // Perform attack roll
        int attackRoll = attacker.AttackRoll();
        int targetAC = (int)target.armorClass;
        bool isHit = attackRoll >= targetAC;

        int damageDealt = 0;
        if (isHit)
        {
            damageDealt = CalculateDamage(attacker, weapon);
            target.SubtractHealth(damageDealt);
        }

        return new AttackResult(
            wasAttempted: true,
            wasHit: isHit,
            attackRoll: attackRoll,
            targetAC: targetAC,
            damageDealt: damageDealt
        );
    }

    /// <summary>
    /// Calculates damage for an attack based on the weapon and attacker stats
    /// Can be extended to include modifiers, critical hits, resistances, etc.
    /// </summary>
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
