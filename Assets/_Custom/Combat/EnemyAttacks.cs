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
    CombatLogPanel combatLog;

    float attackDelay = 1f; // Delay before enemy attacks after their turn starts

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
        combatLog = FindAnyObjectByType<CombatLogPanel>();
    }

    void Update()
    {
        if (creatureStats.inCombat && !hasAttackedThisTurn)
        {
            // wait 1 second
            attackDelay -= Time.deltaTime;
            if (attackDelay <= 0f)
            {
                AttackPlayer(player);
                hasAttackedThisTurn = true; // Only attack once per turn
            }
        }
        else if (!creatureStats.inCombat)
        {
            hasAttackedThisTurn = false; // Reset flag when it's not our turn
            attackDelay = 1f; // Reset attack delay for next turn
        }
    }

    public void AttackPlayer(Transform enemy)
    {
        // Silently stop if player is already dead
        if (targetStats.isDead)
            return;

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
            combatLog.SendMessageToCombatLog($"Enemy attacks {targetStats.name} with {weapon.name} but the attack failed! (Reason: {result.failureReason})", CombatMessage.CombatMessageType.enemyAttack);
            return;
        }

        if (result.wasHit)
        {
            Debug.Log($"Enemy: Attack hit! Attack Roll: {result.attackRoll} vs Target AC: {result.targetAC}, Damage: {result.damageDealt}");
            combatLog.SendMessageToCombatLog($"Enemy attacks {targetStats.name} with {weapon.name} and hits for {result.damageDealt} damage!", CombatMessage.CombatMessageType.enemyAttack);
        }
        else
        {
            Debug.Log($"Enemy: Attack missed! Attack Roll: {result.attackRoll} vs Target AC: {result.targetAC}");
            combatLog.SendMessageToCombatLog($"Enemy attacks {targetStats.name} with {weapon.name} but misses! (Attack Roll: {result.attackRoll} vs Target AC: {result.targetAC})", CombatMessage.CombatMessageType.enemyAttack);
        }

        // Advance to next turn
        initiative.NextTurn();
    }


}
