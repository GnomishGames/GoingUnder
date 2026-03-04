using System;
using System.Linq.Expressions;
using UnityEngine;

public class SkillBar : MonoBehaviour
{
    public SkillBook skillBook;

    //skills that are equipped to the skill panel
    public SkillSO[] skillSOs = new SkillSO[10];

    //event triggered when a skill slot changes (passes slot number)
    public event Action<int> OnSkillChanged;

    Equipment equipment;

    void Awake()
    {
        skillBook = GetComponent<SkillBook>();
        equipment = GetComponent<Equipment>();
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

        // check if attack hits
        int attackRoll = this.GetComponent<CreatureStats>().toHit();

        // calculate damage
        if (attackRoll >= this.GetComponent<PlayerTargeting>().currentTarget.GetComponent<CreatureStats>().armorClass)
        {
            // roll the weapon damage. Take the die and get a random number from that. 
            // Repeat for the dieMultiplier and add them together. Then add the dieBonus.

            //first get the weapon
            var weapon = equipment.weaponSOs[0]; // Assuming the first weapon slot is used for the attack

            // Now calculate the damage based on the weapon's stats
            int damage = 0;
            for (int i = 0; i < weapon.DieMultiplier; i++)
            {
                damage += UnityEngine.Random.Range(1, weapon.Die + 1);
                Debug.Log($"SkillBar: Rolled {damage} damage for skill {skillSOs[slotNumber].name} in slot {slotNumber}.");
            }

            damage += weapon.DieBonus;

            // Apply damage to the target
            this.GetComponent<PlayerTargeting>().currentTarget.GetComponent<CreatureStats>().Hitpoints.ModifyCurrent(-damage);

            Debug.Log($"SkillBar: Skill {skillSOs[slotNumber].name} hit for {damage} damage!");
        }
        else
        {
            Debug.Log($"SkillBar: Skill {skillSOs[slotNumber].name} missed!");
        }
    }
}