using System;
using System.Linq;
using UnityEngine;

/*
This script goes on the main heierarcy and is used to manage initiative order and turn tracking. 
It will likely need to reference the player and all enemies in the scene, as well as any UI elements related to initiative display.
*/

public class Initiative : MonoBehaviour
{
    Transform player;
    public Transform[] enemies;
    private System.Collections.Generic.List<Transform> turnOrder;
    private int currentTurnIndex = 0;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        enemies = GameObject.FindGameObjectsWithTag("Enemy").Select(e => e.transform).ToArray();
    }

    public void StartCombat() //call this from the enemy when they spawn
    {
        // Roll initiative for player and enemies, sort them, and start the turn order
        int playerInitiative = player.GetComponent<CreatureStats>().RollInitiative();
        var enemyInitiatives = enemies.Select(e => new { Enemy = e, Initiative = e.GetComponent<CreatureStats>().RollInitiative() }).ToList();

        // Sort by initiative (descending)
        turnOrder = enemyInitiatives.OrderByDescending(e => e.Initiative).Select(e => e.Enemy).ToList();
        turnOrder.Insert(0, player); // Player goes first for simplicity
        currentTurnIndex = 0;

        // Now you have a sorted turn order list to manage turns
        foreach (var combatant in turnOrder)
        {
            Debug.Log($"{combatant.name} has initiative {combatant.GetComponent<CreatureStats>().RollInitiative()}");
        }

        // find the first combatant in the turn order and start their turn
        StartTurn(turnOrder[0]);
    }

    public void StartTurn(Transform transform)
    {
        // Logic to start the turn for the given combatant (player or enemy)
        Debug.Log($"Starting turn for {transform.name}");

        //disable inCombat for all combatants
        player.GetComponent<CreatureStats>().inCombat = false;
        foreach (var enemy in enemies)
        {
            enemy.GetComponent<CreatureStats>().inCombat = false;
        }

        // Enable combat for this combatant
        transform.GetComponent<CreatureStats>().inCombat = true;
    }

    public void NextTurn()
    {
        // Move to the next combatant in turn order
        currentTurnIndex++;
        
        // Loop back to the start if we've reached the end
        if (currentTurnIndex >= turnOrder.Count)
        {
            currentTurnIndex = 0;
        }

        StartTurn(turnOrder[currentTurnIndex]);
    }
}
