using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatSlider : MonoBehaviour
{
    private CreatureStats creatureStats;
    private CharacterPanel characterPanel;

    private string statName;
    private Slider slider;

    private void Start()
    {
        statName = this.gameObject.name;

        characterPanel = GetComponentInParent<CharacterPanel>();

        creatureStats = transform.root.GetComponent<CreatureStats>();

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
                creatureStats.Hitpoints.OnCurrentChanged += UpdateSlider;
                break;
            case "MaxHitpoints":
                creatureStats.Hitpoints.OnMaxChanged += UpdateSlider;
                break;

            //Experience
            case "Experience":
                creatureStats.OnEXPChanged += UpdateSlider;
                break;
            case "MaxExperience":
                creatureStats.OnEXPChanged += UpdateSlider;
                break;

            // Stamina
            case "Stamina":
                creatureStats.Stamina.OnCurrentChanged += UpdateSlider;
                break;
            case "MaxStamina":
                creatureStats.Stamina.OnMaxChanged += UpdateSlider;
                break;

            // Mana
            case "Mana":
                creatureStats.Mana.OnCurrentChanged += UpdateSlider;
                break;
            case "MaxMana":
                creatureStats.Mana.OnMaxChanged += UpdateSlider;
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
                return creatureStats.Hitpoints.Current;
            case "Experience":
                return creatureStats.experience;
            case "Stamina":
                return creatureStats.Stamina.Current;
            case "Mana":
                return creatureStats.Mana.Current;
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
                return creatureStats.Hitpoints.Max;
            case "Experience":
                return creatureStats.maxExperience;
            case "Stamina":
                return creatureStats.Stamina.Max;
            case "Mana":
                return creatureStats.Mana.Max;
            default:
                Debug.LogError($"Stat name {statName} not recognized!");
                return 0;
        }
    }
}
