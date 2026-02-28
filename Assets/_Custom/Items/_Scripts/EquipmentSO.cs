using UnityEngine;

public class EquipmentSO : ItemSO
{
    [Header("Primary Bonuses")]
    public int ArmorBonus;
    public int StaminaBonus;
    public int ManaBonus;

    [Header("Stat Bonuses")]
    public int StrengthBonus;
    public int ConstitutionBonus;
    public int DexterityBonus;
    public int IntelligenceBonus;
    public int WisdomBonus;
    public int CharismaBonus;

    [Header("Critical Bonuses")]
    public int CriticalChanceBonus;
    public int CriticalDamageBonus;
}
