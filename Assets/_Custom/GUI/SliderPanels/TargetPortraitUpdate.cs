using UnityEngine;
using UnityEngine.UI;

public class TargetPortraitUpdate : MonoBehaviour
{
    private CreatureStats creatureStats;
    private CreatureStats targetStats;
    private PlayerTargeting playerTargeting;
    public Image icon;

    void OnEnable()
    {
        creatureStats = transform.root.GetComponent<CreatureStats>();
        playerTargeting = creatureStats.GetComponent<PlayerTargeting>();

        if (playerTargeting != null)
        {
            if (playerTargeting.currentTarget != null)
            {
                targetStats = playerTargeting.currentTarget.GetComponent<CreatureStats>();
                UpdatePortrait(targetStats);
            }
        }
        else
            Debug.LogWarning($"[TargetPanel] PlayerTargeting component not found on {creatureStats.gameObject.name}");

        // Subscribe to target change event
        if (playerTargeting != null)
            playerTargeting.OnTargetChanged += UpdatePortrait;
        else
            Debug.LogWarning($"[TargetPanel] Cannot subscribe to target change event - PlayerTargeting component not found on {creatureStats.gameObject.name}");
    }

    void OnDestroy()
    {
        // Unsubscribe from target change event to prevent memory leaks
        if (playerTargeting != null)
            playerTargeting.OnTargetChanged -= UpdatePortrait;
    }

    void UpdatePortrait(CreatureStats creatureStats)
    {
        if (creatureStats == null)
        {
            icon.sprite = null; // Clear icon if no target
            return;
        }
        
        if (icon != null && creatureStats != null && creatureStats.icon != null)
        {
            icon.sprite = creatureStats.icon;
        }
    }
}
