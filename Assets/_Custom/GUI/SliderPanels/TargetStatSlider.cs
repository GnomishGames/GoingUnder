using UnityEngine;
using UnityEngine.UI;

public class TargetStatSlider : MonoBehaviour
{
    private PlayerTargeting playerTargeting;
    private CreatureStats targetStats;

    private string statName;
    private Slider slider;

    private void OnEnable()
    {
        statName = this.gameObject.name;

        playerTargeting = transform.root.GetComponentInChildren<PlayerTargeting>();
        targetStats = playerTargeting.currentTarget.GetComponent<CreatureStats>();

        slider = GetComponent<Slider>();

        SubscribeToEvent();
        // Initialize display with current value
        UpdateSlider(0); // Parameter ignored, values fetched by statName
    }

    void SubscribeToEvent()
    {
        switch (statName)
        {
            //Hitpoints
            case "Hitpoints":
                targetStats.Hitpoints.OnCurrentChanged += UpdateSlider;
                break;
            case "MaxHitpoints":
                targetStats.Hitpoints.OnMaxChanged += UpdateSlider;
                break;

            //Experience
            case "Experience":
                targetStats.OnEXPChanged += UpdateSlider;
                break;
            case "MaxExperience":
                targetStats.OnEXPChanged += UpdateSlider;
                break;

            // Stamina
            case "Stamina":
                targetStats.Stamina.OnCurrentChanged += UpdateSlider;
                break;
            case "MaxStamina":
                targetStats.Stamina.OnMaxChanged += UpdateSlider;
                break;

            // Mana
            case "Mana":
                targetStats.Mana.OnCurrentChanged += UpdateSlider;
                break;
            case "MaxMana":
                targetStats.Mana.OnMaxChanged += UpdateSlider;
                break;
        }
    }

    void UpdateSlider(int eventValue)
    {
        // Ignore the event parameter and fetch the correct values based on statName
        int currentValue = GetCurrentStatValue();
        int maxValue = GetMaxStatValue();

        // IMPORTANT: Set maxValue FIRST, then value, to avoid clamping issues
        slider.maxValue = maxValue;
        slider.value = currentValue;
    }

    int GetCurrentStatValue()
    {
        switch (statName)
        {
            case "Hitpoints":
                return targetStats.Hitpoints.Current;
            case "Experience":
                return targetStats.experience;
            case "Stamina":
                return targetStats.Stamina.Current;
            case "Mana":
                return targetStats.Mana.Current;
            default:
                Debug.LogError($"Stat name {statName} not recognized!");
                return 0;
        }
    }

    int GetMaxStatValue()
    {
        switch (statName)
        {
            case "Hitpoints":
                return targetStats.Hitpoints.Max;
            case "Experience":
                return targetStats.maxExperience;
            case "Stamina":
                return targetStats.Stamina.Max;
            case "Mana":
                return targetStats.Mana.Max;
            default:
                Debug.LogError($"Stat name {statName} not recognized!");
                return 0;
        }
    }
}
