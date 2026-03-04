using UnityEngine;

[CreateAssetMenu(fileName = "New Class", menuName = "Scriptable Object/Character/Class")]
public class ClassSO : ScriptableObject
{
    public string className;

    [Header("Hit and Mana die affect how much health and mana the creature has.")]
    public int hitDie;
    public int manaDie;

    [Header("Stat bonuses are special skills developed by the class. 1 is normal.")]
    [Tooltip("A warrior might have more strength than they would appear to have at first glance.")]
    public int strengthBonus;
    [Tooltip("A rogue might have more dexterity than they would appear to have at first glance.")]
    public int dexterityBonus;
    [Tooltip("A paladin might have more constitution than they would appear to have at first glance.")]
    public int constitutionBonus;
    [Tooltip("A wizard might have more intelligence than they would appear to have at first glance.")]
    public int intelligenceBonus;
    [Tooltip("A cleric might have more wisdom than they would appear to have at first glance.")]
    public int wisdomBonus;
    [Tooltip("A bard might have more charisma than they would appear to have at first glance.")]
    public int charismaBonus;
}