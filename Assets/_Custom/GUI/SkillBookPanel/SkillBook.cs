using System;
using UnityEngine;

public class SkillBook : MonoBehaviour
{
    public SkillSO[] skillSOs = new SkillSO[10];

    //panel references
    public SkillBarPanel skillBarPanel;
    public SkillBar skillBar;

    //event triggered when a skill slot changes (passes slot number)
    public event Action<int> OnSkillChanged;

    void Awake()
    {
        skillBar = GetComponent<SkillBar>();
    }

    public void TriggerSkillChanged(int slotNumber)
    {
        OnSkillChanged?.Invoke(slotNumber);
    }

    public void MoveItem(int fromSlot, int toSlot)
    {
        SkillSO temp = skillSOs[fromSlot];
        skillSOs[fromSlot] = skillSOs[toSlot];
        skillSOs[toSlot] = temp;
        
        OnSkillChanged?.Invoke(fromSlot);
        OnSkillChanged?.Invoke(toSlot);
    }

    public void UnEquipSkill(int skillBookSlot, int skillPanelSlot)
    {
        if (skillSOs[skillBookSlot] == null)
        {
            var buffer = skillSOs[skillBookSlot];
            skillSOs[skillBookSlot] = skillBar.skillSOs[skillPanelSlot];
            skillBar.skillSOs[skillPanelSlot] = (SkillSO)buffer;
        }
        else if (skillSOs[skillBookSlot].slotType == skillBar.skillSOs[skillPanelSlot].slotType)
        {
            var buffer = skillSOs[skillBookSlot];
            skillSOs[skillBookSlot] = skillBar.skillSOs[skillPanelSlot];
            skillBar.skillSOs[skillPanelSlot] = (SkillSO)buffer;
        }
        
        OnSkillChanged?.Invoke(skillBookSlot);
        skillBar.TriggerSkillChanged(skillPanelSlot);
    }


}
