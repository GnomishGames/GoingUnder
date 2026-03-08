using UnityEngine;
using UnityEngine.UI;

public class PortraitUpdate : MonoBehaviour
{
    private CreatureStats creatureStats;
    public Image icon;

    void Start()
    {
        creatureStats = transform.root.GetComponent<CreatureStats>();

        if (creatureStats != null)
        {
            UpdatePortrait(creatureStats);
        }
        else
        {
            Debug.LogWarning($"[PortraitPanel] CreatureStats component not found on {transform.root.gameObject.name}");
        }
    }

    void OnEnable()
    {
        UpdatePortrait(creatureStats);
    }

    void UpdatePortrait(CreatureStats creatureStats)
    {
        if (icon != null && creatureStats != null && creatureStats.icon != null)
        {
            icon.sprite = creatureStats.icon;
        }
        else
        {
            Debug.LogWarning($"[PortraitPanel] Cannot update portrait - icon: {icon != null}, creatureStats: {creatureStats != null}, creatureStats.icon: {(creatureStats != null ? (creatureStats.icon != null).ToString() : "N/A")}");
        }
    }
}
