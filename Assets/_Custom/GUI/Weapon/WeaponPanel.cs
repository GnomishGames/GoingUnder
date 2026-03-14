using System;
using UnityEngine;

public class WeaponPanel : MonoBehaviour
{
    public int fromSlot; //Tells us where the item came from when we drag/drop
    public string fromPanel; //tells us what panel it came from

    PlayerTargeting playerTargeting;
    Equipment equipment;
    AttackResolver attackResolver;
    CreatureStats creatureStats;
    CombatLogPanel combatLog;
    CreatureStats targetStats;

    void Awake()
    {
        creatureStats = GetComponentInParent<CreatureStats>();
        equipment = GetComponentInParent<Equipment>();
        playerTargeting = GetComponentInParent<PlayerTargeting>();
        attackResolver = FindFirstObjectByType<AttackResolver>();
        combatLog = transform.root.GetComponentInChildren<CombatLogPanel>(true);
    }

    internal void DoSkill(int slotNumber)
    {
        if (!CheckForTarget()) //if no target is selected, log it and do not attempt attack
            return;
            
        targetStats = playerTargeting.currentTarget.GetComponent<CreatureStats>();

        if (!CheckRequiredReferences()) //if any required references are missing, log errors and do not attempt attack
            return;
        if (CheckForDead()) //if player or target is dead, log it and do not attempt attack
            return;

        // Resolve the attack using the combat system
        WeaponSO weapon = equipment.weaponSOs[slotNumber];
        AttackResult attackResult = attackResolver.ResolveAttack(creatureStats, targetStats, weapon);

        // Handle the result
        if (!attackResult.wasAttempted)
        {
            combatLog.SendMessageToCombatLog($"{creatureStats.interactableName} attacks {targetStats.interactableName} with {weapon.name} but the attack failed! (Reason: {attackResult.failureReason})", CombatMessage.CombatMessageType.playerAttack);
            return;
        }

        if (attackResult.wasHit)
        {
            combatLog.SendMessageToCombatLog($"{creatureStats.interactableName} attacks {targetStats.interactableName} with {weapon.name} and hits for {attackResult.damageDealt} damage!", CombatMessage.CombatMessageType.playerAttack);
            //attackDie.SetDieValue(result.attackRoll);
            //damageDie.SetDieValue(result.damageDealt);

            //play attack animation here
            InstantiateEffect.PlayEffect(weapon.impactEffect, targetStats.transform.position, Quaternion.identity);
        }
        else
        {
            combatLog.SendMessageToCombatLog($"{creatureStats.interactableName} attacks {targetStats.interactableName} with {weapon.name} and misses! (Attack Roll: {attackResult.attackRoll} vs Target AC: {attackResult.targetAC})", CombatMessage.CombatMessageType.playerAttack);
            //attackDie.SetDieValue(result.attackRoll);
            //damageDie.SetDieValue(0);
        }

        targetStats.CheckIfDead();

        // Advance to next turn
        Initiative initiative = FindAnyObjectByType<Initiative>();
        if (initiative != null)
        {
            //combatLog.SendMessageToCombatLog($"Player's turn ends.", CombatMessage.CombatMessageType.info);
            initiative.NextTurn();
        }
    }

    bool CheckForTarget()
    {
        if (playerTargeting.currentTarget == null)
        {
            Debug.Log($"WeaponPanel: No target selected for attack.");
            combatLog.SendMessageToCombatLog($"{creatureStats.interactableName} tries to attack but has no target selected!", CombatMessage.CombatMessageType.info);
            return false;
        }

        return true;
    }

    bool CheckForDead()
    {
        if (creatureStats.isDead)
        {
            combatLog.SendMessageToCombatLog($"{creatureStats.interactableName} has died!", CombatMessage.CombatMessageType.info);
        }

        if (targetStats.isDead)
        {
            combatLog.SendMessageToCombatLog($"{targetStats.interactableName} has died!", CombatMessage.CombatMessageType.info);
        }

        return creatureStats.isDead || targetStats.isDead;
    }

    bool CheckRequiredReferences()
    {
        if (playerTargeting == null)
        {
            Debug.LogError("WeaponPanel: PlayerTargeting component not found in parent hierarchy!");
        }

        if (equipment == null)
        {
            Debug.LogError("WeaponPanel: Equipment component not found in parent hierarchy!");
        }

        if (attackResolver == null)
        {
            Debug.LogError("WeaponPanel: CombatResolver not found in scene!");
        }

        if (creatureStats == null)
        {
            Debug.LogError("WeaponPanel: CreatureStats component not found in parent hierarchy!");
        }

        if (combatLog == null)
        {
            Debug.LogError("WeaponPanel: CombatLogPanel not found in parent hierarchy!");
        }

        return playerTargeting != null && equipment != null && attackResolver != null && creatureStats != null && combatLog != null;
    }
}