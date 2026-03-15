using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TargetSelf : MonoBehaviour, IPointerClickHandler
{
    PlayerTargeting playerTargeting;
    TargetPanel targetPanel;
    CreatureStats creatureStats;

    //events
    public event Action<CreatureStats> OnTargetChanged; // Event to notify when the target changes, passing the new target's CreatureStats

    void Awake()
    {
        targetPanel = transform.root.GetComponentInChildren<TargetPanel>(true); //target panel is on a child object of the player, but is inactive at the start of the game
        playerTargeting = GetComponentInParent<PlayerTargeting>();
    }

    void Start()
    {
        targetPanel.gameObject.SetActive(false); //make sure the target panel is inactive at the start of the game
    }

    // clicking on the players own stat window (this object) targets the player
    public void OnPointerClick(PointerEventData eventData)
    {
        creatureStats = transform.root.GetComponent<CreatureStats>();
        playerTargeting.currentTarget = creatureStats; // Set the current target to the player itself
        OnTargetChanged?.Invoke(creatureStats);

        targetPanel.gameObject.SetActive(true);
    }
}
