using System;
using UnityEngine;

public class WeaponPanel : MonoBehaviour
{
    public int fromSlot; //Tells us where the item came from when we drag/drop
    public string fromPanel; //tells us what panel it came from

    PlayerTargeting playerTargeting;
    Equipment equipment;
    CombatResolver combatResolver;
    CreatureStats creatureStats;
    CombatLogPanel combatLog;

    void Start()
    {
        creatureStats = GetComponentInParent<CreatureStats>();
        equipment = GetComponentInParent<Equipment>();
        playerTargeting = GetComponentInParent<PlayerTargeting>();
        combatResolver = FindFirstObjectByType<CombatResolver>();
        combatLog = transform.root.GetComponentInChildren<CombatLogPanel>(true);
    }

    internal void DoSkill(int slotNumber)
    {
        // check for skill null
        Debug.Log($"WeaponPanel: Attempting to use weapon in slot {slotNumber}..."); // Debug log to check slot number
        if (equipment.weaponSOs[slotNumber] == null)
        {
            Debug.Log($"WeaponPanel: No weapon equipped in slot {slotNumber}.");
            return;
        }

        if (playerTargeting.currentTarget == null)
        {
            combatLog.SendMessageToCombatLog($"{creatureStats.interactableName} tries to attack but has no target selected!", CombatMessage.CombatMessageType.playerAttack);
            return;
        }

        CreatureStats targetStats = playerTargeting.currentTarget.GetComponent<CreatureStats>();

        if (targetStats == null)
        {
            combatLog.SendMessageToCombatLog($"{creatureStats.interactableName} tries to attack but has no target!", CombatMessage.CombatMessageType.playerAttack);
            return;
        }

        if (equipment.weaponSOs[slotNumber] == null)
        {
            combatLog.SendMessageToCombatLog($"{creatureStats.interactableName} tries to attack with no weapon equipped!", CombatMessage.CombatMessageType.playerAttack);
            return;
        }

        // Check basic requirements
        if (playerTargeting.currentTarget == null)
        {
            combatLog.SendMessageToCombatLog($"{creatureStats.interactableName} tries to attack with no target selected!", CombatMessage.CombatMessageType.playerAttack);
            return;
        }

        // check if player is already dead
        if (creatureStats.isDead)
        {
            combatLog.SendMessageToCombatLog($"{creatureStats.interactableName} tries to attack but is already dead!", CombatMessage.CombatMessageType.playerAttack);
            return;
        }

        // check if target is already dead        
        if (targetStats.isDead)
        {
            combatLog.SendMessageToCombatLog($"{creatureStats.interactableName} tries to attack {targetStats.interactableName} but they are already dead!", CombatMessage.CombatMessageType.playerAttack);
            return;
        }

        // Resolve the attack using the combat system
        WeaponSO weapon = equipment.weaponSOs[slotNumber];
        AttackResult result = combatResolver.ResolveAttack(creatureStats, targetStats, weapon);

        // Handle the result
        if (!result.wasAttempted)
        {
            combatLog.SendMessageToCombatLog($"{creatureStats.interactableName} attacks {targetStats.interactableName} with {weapon.name} but the attack failed! (Reason: {result.failureReason})", CombatMessage.CombatMessageType.playerAttack);
            return;
        }

        if (result.wasHit)
        {
            combatLog.SendMessageToCombatLog($"{creatureStats.interactableName} attacks {targetStats.interactableName} with {weapon.name} and hits for {result.damageDealt} damage!", CombatMessage.CombatMessageType.playerAttack);
            //attackDie.SetDieValue(result.attackRoll);
            //damageDie.SetDieValue(result.damageDealt);

            //play attack animation here
            InstantiateEffect.PlayEffect(weapon.impactEffect, targetStats.transform.position, Quaternion.identity);
        }
        else
        {
            combatLog.SendMessageToCombatLog($"{creatureStats.interactableName} attacks {targetStats.interactableName} with {weapon.name} and misses! (Attack Roll: {result.attackRoll} vs Target AC: {result.targetAC})", CombatMessage.CombatMessageType.playerAttack);
            //attackDie.SetDieValue(result.attackRoll);
            //damageDie.SetDieValue(0);
        }

        // Advance to next turn
        Initiative initiative = FindAnyObjectByType<Initiative>();
        if (initiative != null)
        {
            //combatLog.SendMessageToCombatLog($"Player's turn ends.", CombatMessage.CombatMessageType.info);
            initiative.NextTurn();
        }
    }
}