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
}

public enum DamageType { Blunt, Piercing, Slashing };