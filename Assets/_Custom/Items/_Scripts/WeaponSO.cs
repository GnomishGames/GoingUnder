using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Scriptable Object/Item/New Weapon")]
public class WeaponSO : EquipmentSO
{
    [Header("Weapon Stats")]
    public DamageType damageType;
    public int Damage; //max
    public int Range;
}

public enum DamageType { Blunt, Piercing, Slashing };