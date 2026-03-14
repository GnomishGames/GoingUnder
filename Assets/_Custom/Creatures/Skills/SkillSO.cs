using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Scriptable Object/Skill/New Skill")]
public class SkillSO : ItemSO
{
    public int staminaCost;
    public int targetDamage;
    public int selfDamage;
    public int selfHeal;
    public float attackRange;
    public GameObject impactEffect;
}
