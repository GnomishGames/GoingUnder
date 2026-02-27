using UnityEngine;

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

[CreateAssetMenu(fileName = "New Race", menuName = "Scriptable Object/Character/Race")]
public class RaceSO : ScriptableObject
{
    public string raceName;

    //racial bonuses
    public int strengthBonus;
    public int constitutionBonus;
    public int dexterityBonus;
    public int intelligenceBonus;
    public int wisdomBonus;
    public int charismaBonus;

    //attack vars
    public float attackSpeed;

    //defense vars
    public Size size = Size.Medium;
    public int naturalAcBonus;

    //view vars
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
}