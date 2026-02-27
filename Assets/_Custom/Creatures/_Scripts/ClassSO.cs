using UnityEngine;

[CreateAssetMenu(fileName = "New Class", menuName = "Scriptable Object/Character/Class")]
public class ClassSO : ScriptableObject
{
    public string className;

    public int hitDie;
    public int manaDie;

    //stats
    public float strengthBonus;
    public float dexterityBonus;
    public float constitutionBonus;
    public float intelligenceBonus;
    public float wisdomBonus;
    public float charismaBonus;
}