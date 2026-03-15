using System;
using UnityEngine;

public class SkillBar : MonoBehaviour
{
    public SkillBook skillBook;

    //skills that are equipped to the skill panel
    public SkillSO[] skillSOs = new SkillSO[10];

    //event triggered when a skill slot changes (passes slot number)
    public event Action<int> OnSkillChanged;

    Equipment equipment;
    public CombatLogPanel combatLog;

    PlayerTargeting playerTargeting;
    CreatureStats targetStats;
    CreatureStats creatureStats;
    SkillResolver skillResolver;

    void Awake()
    {
        skillBook = GetComponent<SkillBook>();
        equipment = GetComponent<Equipment>();
        combatLog = transform.root.GetComponentInChildren<CombatLogPanel>(true);
        playerTargeting = GetComponent<PlayerTargeting>();
        creatureStats = GetComponent<CreatureStats>();
        skillResolver = FindFirstObjectByType<SkillResolver>();

        if (skillResolver == null)
        {
            Debug.LogError($"SkillBar: No SkillResolver found in scene! Skill usage will not work without a SkillResolver.");
        }
    }

    public void TriggerSkillChanged(int slotNumber)
    {
        OnSkillChanged?.Invoke(slotNumber);
    }

    public void MoveSkill(int from, int to)
    {
        var buffer = skillSOs[to];
        skillSOs[to] = skillSOs[from];
        skillSOs[from] = buffer;

        OnSkillChanged?.Invoke(from);
        OnSkillChanged?.Invoke(to);
    }

    public void UnEquipSkill(int skillBarSlot, int skillBookSlot)
    {
        if (skillBook == null)
        {
            Debug.LogError($"SkillBar: skillBook is null! Cannot unequip skill.");
            return;
        }

        if (skillSOs[skillBarSlot] == null)
        {
            var buffer = skillSOs[skillBarSlot];
            skillSOs[skillBarSlot] = skillBook.skillSOs[skillBookSlot];
            skillBook.skillSOs[skillBookSlot] = (SkillSO)buffer;
        }
        else if (skillBook.skillSOs[skillBookSlot] != null && skillSOs[skillBarSlot].slotType == skillBook.skillSOs[skillBookSlot].slotType)
        {
            var buffer = skillSOs[skillBarSlot];
            skillSOs[skillBarSlot] = skillBook.skillSOs[skillBookSlot];
            skillBook.skillSOs[skillBookSlot] = (SkillSO)buffer;
        }

        OnSkillChanged?.Invoke(skillBarSlot);
        skillBook.TriggerSkillChanged(skillBookSlot);
    }

    public void DoSkill(int slotNumber)
    {
        // check for skill null
        if (skillSOs[slotNumber] == null)
        {
            Debug.Log($"SkillBar: No skill equipped in slot {slotNumber}.");
            return;
        }

        if (!CheckForTarget()) //if no target is selected, log it and do not attempt skill
            return;

        targetStats = playerTargeting.currentTarget.GetComponent<CreatureStats>();

        if (!CheckRequiredReferences()) //if any required references are missing, log errors and do not attempt skill
            return;

        if (CheckForDead()) //if player or target is dead, log it and do not attempt skill
            return;

        //resolve the skill using the skill resolver
        SkillResult skillResult = skillResolver.ResolveSkill(creatureStats, targetStats, skillSOs[slotNumber]);

        //handle the result
        if (!skillResult.wasAttempted)
        {
            combatLog.SendMessageToCombatLog($"{creatureStats.interactableName} tries to use {skillSOs[slotNumber].name} on {targetStats.interactableName} but the skill usage failed! (Reason: {skillResult.failureReason})", CombatMessage.CombatMessageType.playerAttack);
            return;
        }

        // If we got here, the skill was attempted. Log success or failure.
        if (skillResult.wasSuccessful)
        {
            string resultMessage = $"{creatureStats.interactableName} uses {skillSOs[slotNumber].name} on {targetStats.interactableName} and successfully hits for {skillResult.damageDealt} damage!";
            if (skillResult.healingDone > 0)
            {
                resultMessage += $" and heals for {skillResult.healingDone} hitpoints!";
            }
            combatLog.SendMessageToCombatLog(resultMessage, CombatMessage.CombatMessageType.playerAttack);

            //play skill effect here
            InstantiateEffect.PlayEffect(skillSOs[slotNumber].impactEffect, targetStats.transform.position, Quaternion.identity);
        }
        else
        {
            combatLog.SendMessageToCombatLog($"{creatureStats.interactableName} uses {skillSOs[slotNumber].name} on {targetStats.interactableName} but misses! (Damage Dealt: {skillResult.damageDealt})", CombatMessage.CombatMessageType.playerAttack);
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

    private bool CheckForDead()
    {
        if (this.GetComponent<CreatureStats>().isDead)
        {
            Debug.Log($"SkillBar: Player is dead and cannot use skills.");
            combatLog.SendMessageToCombatLog($"{this.GetComponent<CreatureStats>().interactableName} tries to use {skillSOs[0].name} but is dead and cannot act!", CombatMessage.CombatMessageType.info);
            return true;
        }

        if (playerTargeting.currentTarget.GetComponent<CreatureStats>().isDead)
        {
            Debug.Log($"SkillBar: Target is dead. Cannot use skill on dead target.");
            combatLog.SendMessageToCombatLog($"{this.GetComponent<CreatureStats>().interactableName} tries to use {skillSOs[0].name} on {playerTargeting.currentTarget.GetComponent<CreatureStats>().interactableName} but the target is already dead!", CombatMessage.CombatMessageType.info);
            return true;
        }

        return false;
    }

    private bool CheckForTarget()
    {
        if (playerTargeting.currentTarget == null)
        {
            Debug.Log($"SkillBar: No target selected for skill.");
            combatLog.SendMessageToCombatLog($"{this.GetComponent<CreatureStats>().interactableName} tries to use {skillSOs[0].name} but has no target selected!", CombatMessage.CombatMessageType.info);
            return false;
        }

        return true;
    }

    private bool CheckRequiredReferences()
    {
        bool hasErrors = false;

        if (equipment == null)
        {
            Debug.LogError($"SkillBar: Equipment component not found on player!");
            hasErrors = true;
        }

        if (combatLog == null)
        {
            Debug.LogError($"SkillBar: CombatLogPanel not found in parent hierarchy!");
            hasErrors = true;
        }

        if (playerTargeting == null)
        {
            Debug.LogError($"SkillBar: PlayerTargeting component not found on player!");
            hasErrors = true;
        }

        if (this.GetComponent<CreatureStats>() == null)
        {
            Debug.LogError($"SkillBar: CreatureStats component not found on player!");
            hasErrors = true;
        }

        return !hasErrors;
    }
}