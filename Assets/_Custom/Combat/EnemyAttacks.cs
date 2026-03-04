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
    AnimationController animController;
    
    private bool hasAttackedThisTurn = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        creatureStats = GetComponent<CreatureStats>();
        equipment = GetComponent<Equipment>();
        combatResolver = FindAnyObjectByType<CombatResolver>();
        initiative = FindAnyObjectByType<Initiative>();
        myStats = GetComponent<CreatureStats>();
        targetStats = player.GetComponent<CreatureStats>();
        animController = GetComponent<AnimationController>();
    }

    void Update()
    {
        if (creatureStats.inCombat && !hasAttackedThisTurn)
        {
            AttackPlayer(player);
            hasAttackedThisTurn = true; // Only attack once per turn
        }
        else if (!creatureStats.inCombat)
        {
            hasAttackedThisTurn = false; // Reset flag when it's not our turn
        }
    }

    public void AttackPlayer(Transform enemy)
    {
        // Silently stop if player is already dead
        if (targetStats.isDead)
        {
            return;
        }

        // Resolve the attack using the combat system
        int slotNumber = 0; // For simplicity, enemies will always use the first weapon slot. This can be expanded to allow for multiple attacks or different weapons.
        WeaponSO weapon = equipment.weaponSOs[slotNumber];
        AttackResult result = combatResolver.ResolveAttack(myStats, targetStats, weapon);

        //play attack animation here 
        animController.PlayAttack();

        
        // Handle the result
        if (!result.wasAttempted)
        {
            Debug.Log($"Enemy: Attack failed - {result.failureReason}");
            return;
        }

        if (result.wasHit)
        {
            Debug.Log($"Enemy: Attack hit! Attack Roll: {result.attackRoll} vs Target AC: {result.targetAC}, Damage: {result.damageDealt}");
        }
        else
        {
            Debug.Log($"Enemy: Attack missed! Attack Roll: {result.attackRoll} vs Target AC: {result.targetAC}");
        }

        // Advance to next turn
        initiative.NextTurn();
    }
}
