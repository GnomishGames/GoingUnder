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

    void Awake()
    {
        skillBook = GetComponent<SkillBook>();
        equipment = GetComponent<Equipment>();
        combatLog = transform.root.GetComponentInChildren<CombatLogPanel>(true);
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

        //check for target null
        if (this.GetComponent<PlayerTargeting>().currentTarget == null)
        {
            Debug.Log($"SkillBar: No target selected for skill {skillSOs[slotNumber].name} in slot {slotNumber}.");
            return;
        }

        // target dead?
        if (this.GetComponent<PlayerTargeting>().currentTarget.GetComponent<CreatureStats>().isDead)
        {
            Debug.Log($"SkillBar: Target is too dead for skill {skillSOs[slotNumber].name} in slot {slotNumber}.");
            return;
        }

        // Am I dead?
        if (this.GetComponent<CreatureStats>().isDead)
        {
            Debug.Log($"SkillBar: Player is too dead to use skill {skillSOs[slotNumber].name} in slot {slotNumber}.");
            return;
        }

        // Call the skill's effect method here, passing necessary parameters
        Debug.Log($"SkillBar: Activating skill {skillSOs[slotNumber].name} from slot {slotNumber}.");

        // Apply self-heal if applicable
        if (skillSOs[slotNumber].selfHeal > 0 && this.GetComponent<CreatureStats>() != null)
        {
            this.GetComponent<CreatureStats>().Hitpoints.ModifyCurrent((int)skillSOs[slotNumber].selfHeal);
            Debug.Log($"SkillBar: Skill {skillSOs[slotNumber].name} healed {skillSOs[slotNumber].selfHeal} HP!");

            //combatlog message
            combatLog.SendMessageToCombatLog($"{this.GetComponent<CreatureStats>().interactableName} used {skillSOs[slotNumber].name} and healed for {skillSOs[slotNumber].selfHeal} HP!", CombatMessage.CombatMessageType.playerAttack);
        }

        // do damage to target if applicable
        var targetStats = this.GetComponent<PlayerTargeting>().currentTarget.GetComponent<CreatureStats>();
        if (targetStats != null && skillSOs[slotNumber].targetDamage > 0)
        {
            targetStats.Hitpoints.ModifyCurrent(-(int)skillSOs[slotNumber].targetDamage);

            //combatlog message
            combatLog.SendMessageToCombatLog($"{this.GetComponent<CreatureStats>().interactableName} used {skillSOs[slotNumber].name} and dealt {skillSOs[slotNumber].targetDamage} damage to {targetStats.interactableName}!", CombatMessage.CombatMessageType.playerAttack);
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
}