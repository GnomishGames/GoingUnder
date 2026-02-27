using UnityEngine;

[CreateAssetMenu(fileName = "New Race", menuName = "Scriptable Object/Character/Race")]
public class RaceSO : ScriptableObject
{
    public string raceName;

    [Header("Racial bonuses should be 1 unless the race has special properties.")]
    [Tooltip("Creatures like a zombie might have more strength than they would appear to have at first glance.")]
    public int strengthBonus = 1;
    [Tooltip("Creatures like a dwarf might have more constitution than they would appear to have at first glance.")]
    public int constitutionBonus = 1;
    [Tooltip("Creatures like an elf might have more dexterity than they would appear to have at first glance.")]
    public int dexterityBonus = 1;
    [Tooltip("Creatures like a gnome might have more intelligence than they would appear to have at first glance.")]
    public int intelligenceBonus = 1;
    [Tooltip("Creatures like a halfling might have more wisdom than they would appear to have at first glance.")]
    public int wisdomBonus = 1;
    [Tooltip("Creatures like a vampire might have more charisma than they would appear to have at first glance.")]
    public int charismaBonus = 1;

    [Header("The size of the creature affects its AC.")] 
    [Tooltip("Larger creatures are easier to hit, while smaller creatures are harder to hit.")]
    public Size size = Size.Medium;
    
    [Header("Creatures that have some kind of natural armor (like a turtle shell) can have a natural AC bonus.")]
    [Tooltip("This is added to the size AC bonus and any armor the creature is wearing to determine total AC.")]
    public int naturalAcBonus;

    [Header("The creature's vision range and angle. Unlikely to ever be used in this game.")]
    public float viewRadius = 100f;
    public float viewAngle = 180f;

    public int sizeAcBonus
    {
        get
        {
            switch (size)
            {
                case Size.Colossal:
                    return -8;
                case Size.Gargantuan:
                    return -4;
                case Size.Huge:
                    return -2;
                case Size.Large:
                    return -1;
                case Size.Medium:
                    return 0;
                case Size.Small:
                    return 1;
                case Size.Tiny:
                    return 2;
                case Size.Diminutive:
                    return 4;
                case Size.Fine:
                    return 8;
                default:
                    return 0;
            }
        }
    }

    public enum Size
    {
        Colossal,   // -8
        Gargantuan, // -4
        Huge,       // -2
        Large,      // -1
        Medium,     // +0
        Small,      // +1
        Tiny,       // +2
        Diminutive, // +4
        Fine        // +8
    }
}