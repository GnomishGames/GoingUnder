using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Scriptable Object/Skill/New Skill")]
public class SkillSO : ItemSO
{
    public float staminaCost;
    public int targetDamage;
    public float selfDamage;
    public float attackRange;
}
