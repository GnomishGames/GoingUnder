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
            //Heath
            case "Health":
                creatureStats.OnHealthChanged += UpdateSlider;
                break;
            case "MaxHealth":
                creatureStats.OnMaxHealthChanged += UpdateSlider;
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
                creatureStats.OnStaminaChanged += UpdateSlider;
                break;
            case "MaxStamina":
                creatureStats.OnMaxStaminaChanged += UpdateSlider;
                break;

            // Mana
            case "Mana":
                creatureStats.OnManaChanged += UpdateSlider;
                break;
            case "MaxMana":
                creatureStats.OnMaxManaChanged += UpdateSlider;
                break;
        }
    }

    void UpdateSlider(float eventValue)
    {
        // Ignore the event parameter and fetch the correct values based on statName
        float currentValue = GetCurrentStatValue();
        float maxValue = GetMaxStatValue();

        // IMPORTANT: Set maxValue FIRST, then value, to avoid clamping issues
        slider.maxValue = maxValue;
        slider.value = currentValue;
    }

    float GetCurrentStatValue()
    {
        switch (statName)
        {
            case "Hitpoints":
                return creatureStats.currentHitPoints;
            case "Experience":
                return creatureStats.experience;
            case "Stamina":
                return creatureStats.currentStamina;
            case "Mana":
                return creatureStats.currentMana;
            default:
                Debug.LogError($"Stat name {statName} not recognized!");
                return 0;
        }
    }

    float GetMaxStatValue()
    {
        switch (statName)
        {
            case "Hitpoints":
                return creatureStats.maxHitpoints;
            case "Experience":
                return creatureStats.maxExperience;
            case "Stamina":
                return creatureStats.maxStamina;
            case "Mana":
                return creatureStats.maxMana;
            default:
                Debug.LogError($"Stat name {statName} not recognized!");
                return 0;
        }
    }
}
