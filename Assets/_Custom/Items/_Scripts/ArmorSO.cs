using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Armor", menuName = "Scriptable Object/Item/New Armor")]
public class ArmorSO : EquipmentSO
{

    public override List<TooltipLine> GetTooltip()
    {
        var lines = base.GetTooltip();

        lines.Add(new TooltipLine(" "));
        lines.Add(new TooltipLine("Armor", ArmorBonus.ToString()));

        return lines;
    }
}
