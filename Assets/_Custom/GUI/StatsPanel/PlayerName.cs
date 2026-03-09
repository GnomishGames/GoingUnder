using UnityEngine;
using TMPro;
using System.ComponentModel.Design.Serialization;

public class PlayerName : MonoBehaviour
{
    private TextMeshProUGUI playerNameText;
    private CreatureStats playerStats;

    void OnEnable()
    {
        playerNameText = GetComponent<TextMeshProUGUI>();
        playerStats = transform.root.GetComponentInChildren<CreatureStats>();

        UpdateName(playerStats.interactableName); // Initialize with current name

        // Subscribe to any event that would change the player's name (if applicable)
        playerStats.OnNameChanged += UpdateName; // Assuming you have an event like this in CreatureStats
    }

    void UpdateName(string newName)
    {
        if (playerStats != null)
        {
            playerNameText.text = newName;
        }
        else
        {
            playerNameText.text = "NoName";
        }
    }

    void OnDisable()
    {
        // Unsubscribe from the event to prevent memory leaks
        if (playerStats != null)
        {
            playerStats.OnNameChanged -= UpdateName;
        }
    }
}
