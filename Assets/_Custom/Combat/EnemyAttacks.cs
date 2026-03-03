using UnityEngine;

public class EnemyAttacks : MonoBehaviour
{
    Transform player;
    CreatureStats creatureStats;
    Equipment equipment;
    CombatResolver combatResolver;
    CreatureStats myStats;
    CreatureStats targetStats;
    Initiative initiative;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        creatureStats = GetComponent<CreatureStats>();
        equipment = GetComponent<Equipment>();
        combatResolver = FindAnyObjectByType<CombatResolver>();
        initiative = FindAnyObjectByType<Initiative>();
        myStats = GetComponent<CreatureStats>();
        targetStats = player.GetComponent<CreatureStats>();
    }

    void Update()
    {
        if (creatureStats.inCombat)
        {
            AttackPlayer(player);
        }
    }

    public void AttackPlayer(Transform enemy)
    {
        // Resolve the attack using the combat system
        int slotNumber = 0; // For simplicity, enemies will always use the first weapon slot. This can be expanded to allow for multiple attacks or different weapons.
        WeaponSO weapon = equipment.weaponSOs[slotNumber];
        AttackResult result = combatResolver.ResolveAttack(myStats, targetStats, weapon);

        // Handle the result
        if (!result.wasAttempted)
        {
            Debug.Log($"Weapon: Attack failed - {result.failureReason}");
            return;
        }

        if (result.wasHit)
        {
            Debug.Log($"Weapon: Attack hit! Attack Roll: {result.attackRoll} vs Target AC: {result.targetAC}, Damage: {result.damageDealt}");
        }
        else
        {
            Debug.Log($"Weapon: Attack missed! Attack Roll: {result.attackRoll} vs Target AC: {result.targetAC}");
        }

        // Advance to next turn
        initiative.NextTurn();
    }
}
