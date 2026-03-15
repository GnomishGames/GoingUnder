using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Scriptable Object/Skill/New Skill")]
public class SkillSO : ItemSO
{
    public int staminaCost;
    public int manaCost;
    public int damage;
    public int heal;
    public GameObject impactEffect;
}
