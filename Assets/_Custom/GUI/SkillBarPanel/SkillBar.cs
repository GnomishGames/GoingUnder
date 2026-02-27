using System;
using UnityEngine;

public class SkillBar : MonoBehaviour
{
    public SkillBook skillBook;

    //skills that are equipped to the skill panel
    public SkillSO[] skillSOs = new SkillSO[10];

    //event triggered when a skill slot changes (passes slot number)
    public event Action<int> OnSkillChanged;

    void Awake()
    {
        skillBook = GetComponent<SkillBook>();
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
        else if (skillSOs[skillBarSlot].slotType == skillBook.skillSOs[skillBookSlot].slotType)
        {
            var buffer = skillSOs[skillBarSlot];
            skillSOs[skillBarSlot] = skillBook.skillSOs[skillBookSlot];
            skillBook.skillSOs[skillBookSlot] = (SkillSO)buffer;
        }
        
        OnSkillChanged?.Invoke(skillBarSlot);
        skillBook.TriggerSkillChanged(skillBookSlot);
    }
}