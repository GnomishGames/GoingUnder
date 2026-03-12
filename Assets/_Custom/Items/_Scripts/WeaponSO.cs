using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Scriptable Object/Item/New Weapon")]
public class WeaponSO : EquipmentSO
{
    [Header("Weapon Damage Type")]
    public DamageType damageType;
    
    [Header("Die Information")]
    [Tooltip("Number of dice rolled for damage")]
    public int DieMultiplier; //number of dice

    [Tooltip("Maximum value of each die")]
    public int Die; //max die value
    
    [Tooltip("Bonus added to the damage roll")]
    public int DieBonus; //bonus added to the damage roll

    public override List<TooltipLine> GetTooltip()
    {
        var lines = base.GetTooltip();

        lines.Add(new TooltipLine(" "));
        lines.Add(new TooltipLine("Damage", $"{DieMultiplier}d{Die}+{DieBonus}"));
        lines.Add(new TooltipLine("Damage Type", damageType.ToString()));

        return lines;
    }
}

public enum DamageType { Blunt, Piercing, Slashing };