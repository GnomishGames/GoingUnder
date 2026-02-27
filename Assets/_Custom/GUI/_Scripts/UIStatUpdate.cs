using UnityEngine;
using TMPro;

public class UIStatUpdate : MonoBehaviour
{

    /*
    This script is attached to text objects in the character panel and updates their display when the corresponding stat changes.
    It uses the name of the GameObject it's attached to determine which stat to display (e.g. "Health", "Mana", "StrengthScore", etc.)
    */

    Transform player;
    CreatureStats creatureStats;
    TextMeshProUGUI textDisplay;
    [SerializeField] private string prefix = "";
    [SerializeField] private string suffix = "";
    [SerializeField] private string formatString = "F0"; // "F0" for whole numbers, "F1" for one decimal, etc.

    private string statName;

    void Start()
    {
        //find player by tag. This assumes this script is only used for the player character panel. If used for NPCs, this will need to be changed.
        player = GameObject.FindGameObjectWithTag("Player").transform; 

        // Use the GameObject's name as the stat name
        statName = this.gameObject.name;

        //get characterstats if not assigned
        creatureStats = player.GetComponent<CreatureStats>();

        //get textdisplay if not assigned
        textDisplay = GetComponent<TextMeshProUGUI>();

        if (creatureStats != null)
        {
            SubscribeToEvent();

            // Initialize display with current value after stats are calculated
            UpdateDisplayFromStatName();
        }
        else
        {
            Debug.LogWarning("CreatureStats not found on " + gameObject.name);
        }
    }

    void UpdateDisplayFromStatName()
    {
        switch (statName)
        {
            // String types
            case "Name":
                UpdateDisplayString(creatureStats.interactableName);
                break;
            case "Class":
                UpdateDisplayClass(creatureStats.characterClass);
                break;
            case "Race":
                UpdateDisplayRace(creatureStats.characterRace);
                break;

            // All other stats (numbers)
            default:
                float value = GetCurrentStatValue();
                UpdateDisplay(value);
                break;
        }
    }

    float GetCurrentStatValue()
    {
        switch (statName)
        {
            // Health
            case "Hitpoints":
                return creatureStats.currentHitPoints;
            case "MaxHitpoints":
                return creatureStats.maxHitpoints;

            // Experience
            case "Level":
                return creatureStats.characterLevel;
            case "Experience":
                return creatureStats.experience;
            case "MaxExperience":
                return creatureStats.maxExperience;

            // Stamina
            case "Stamina":
                return creatureStats.currentStamina;
            case "MaxStamina":
                return creatureStats.maxStamina;

            // Mana
            case "Mana":
                return creatureStats.currentMana;
            case "MaxMana":
                return creatureStats.maxMana;

            // Strength
            case "StrengthBase":
                return creatureStats.strengthBase;
            case "StrengthRace":
                return creatureStats.characterRace.strengthBonus;
            case "StrengthClass":
                return creatureStats.characterClass.strengthBonus;
            case "StrengthScore":
                return creatureStats.strengthScore;
            case "StrengthModifier":
                return creatureStats.strengthModifier;

            // Dexterity
            case "DexterityBase":
                return creatureStats.dexterityBase;
            case "DexterityRace":
                return creatureStats.characterRace.dexterityBonus;
            case "DexterityClass":
                return creatureStats.characterClass.dexterityBonus;
            case "DexterityScore":
                return creatureStats.dexterityScore;
            case "DexterityModifier":
                return creatureStats.dexterityModifier;

            // Constitution
            case "ConstitutionBase":
                return creatureStats.constitutionBase;
            case "ConstitutionRace":
                return creatureStats.characterRace.constitutionBonus;
            case "ConstitutionClass":
                return creatureStats.characterClass.constitutionBonus;
            case "ConstitutionScore":
                return creatureStats.constitutionScore;
            case "ConstitutionModifier":
                return creatureStats.constitutionModifier;

            // Intelligence
            case "IntelligenceBase":
                return creatureStats.intelligenceBase;
            case "IntelligenceRace":
                return creatureStats.characterRace.intelligenceBonus;
            case "IntelligenceClass":
                return creatureStats.characterClass.intelligenceBonus;
            case "IntelligenceScore":
                return creatureStats.intelligenceScore;
            case "IntelligenceModifier":
                return creatureStats.intelligenceModifier;

            // Wisdom
            case "WisdomBase":
                return creatureStats.wisdomBase;
            case "WisdomRace":
                return creatureStats.characterRace.wisdomBonus;
            case "WisdomClass":
                return creatureStats.characterClass.wisdomBonus;
            case "WisdomScore":
                return creatureStats.wisdomScore;
            case "WisdomModifier":
                return creatureStats.wisdomModifier;

            // Charisma
            case "CharismaBase":
                return creatureStats.charismaBase;
            case "CharismaRace":
                return creatureStats.characterRace.charismaBonus;
            case "CharismaClass":
                return creatureStats.characterClass.charismaBonus;
            case "CharismaScore":
                return creatureStats.charismaScore;
            case "CharismaModifier":
                return creatureStats.charismaModifier;

            // Armor
            case "ArmorClass":
                return creatureStats.armorClass;

            default:
                Debug.LogWarning($"Unknown stat name: {statName}");
                return 0;
        }
    }

    void OnDisable()
    {
        if (creatureStats == null)
            return;

        UnsubscribeFromEvent();
    }

    void SubscribeToEvent()
    {
        switch (statName)
        {
            // General
            case "Name":
                creatureStats.OnNameChanged += UpdateDisplayString;
                break;
            case "Class":
                creatureStats.OnClassChanged += UpdateDisplayClass;
                break;
            case "Race":
                creatureStats.OnRaceChanged += UpdateDisplayRace;
                break;

            // Health
            case "Health":
                creatureStats.OnHealthChanged += UpdateDisplay;
                break;
            case "MaxHealth":
                creatureStats.OnMaxHealthChanged += UpdateDisplay;
                break;

            // Experience
            case "Level":
                creatureStats.OnLevelChanged += UpdateDisplay;
                break;
            case "Experience":
                creatureStats.OnEXPChanged += UpdateDisplay;
                break;
            case "MaxExperience":
                creatureStats.OnEXPChanged += UpdateDisplay;
                break;

            // Stamina
            case "Stamina":
                creatureStats.OnStaminaChanged += UpdateDisplay;
                break;
            case "MaxStamina":
                creatureStats.OnMaxStaminaChanged += UpdateDisplay;
                break;

            // Mana
            case "Mana":
                creatureStats.OnManaChanged += UpdateDisplay;
                break;
            case "MaxMana":
                creatureStats.OnMaxManaChanged += UpdateDisplay;
                break;

            // Strength
            case "StrengthBase":
                creatureStats.OnStrengthBaseChanged += UpdateDisplay;
                break;
            case "StrengthRace":
                creatureStats.OnStrengthRaceChanged += UpdateDisplay;
                break;
            case "StrengthClass":
                creatureStats.OnStrengthClassChanged += UpdateDisplay;
                break;
            case "StrengthScore":
                creatureStats.OnStrengthScoreChanged += UpdateDisplay;
                break;
            case "StrengthModifier":
                creatureStats.OnStrengthModifierChanged += UpdateDisplay;
                break;

            // Dexterity
            case "DexterityBase":
                creatureStats.OnDexterityBaseChanged += UpdateDisplay;
                break;
            case "DexterityRace":
                creatureStats.OnDexterityRaceChanged += UpdateDisplay;
                break;
            case "DexterityClass":
                creatureStats.OnDexterityClassChanged += UpdateDisplay;
                break;
            case "DexterityScore":
                creatureStats.OnDexterityScoreChanged += UpdateDisplay;
                break;
            case "DexterityModifier":
                creatureStats.OnDexterityModifierChanged += UpdateDisplay;
                break;

            // Constitution
            case "ConstitutionBase":
                creatureStats.OnConstitutionBaseChanged += UpdateDisplay;
                break;
            case "ConstitutionRace":
                creatureStats.OnConstitutionRaceChanged += UpdateDisplay;
                break;
            case "ConstitutionClass":
                creatureStats.OnConstitutionClassChanged += UpdateDisplay;
                break;
            case "ConstitutionScore":
                creatureStats.OnConstitutionScoreChanged += UpdateDisplay;
                break;
            case "ConstitutionModifier":
                creatureStats.OnConstitutionModifierChanged += UpdateDisplay;
                break;

            // Intelligence
            case "IntelligenceBase":
                creatureStats.OnIntelligenceBaseChanged += UpdateDisplay;
                break;
            case "IntelligenceRace":
                creatureStats.OnIntelligenceRaceChanged += UpdateDisplay;
                break;
            case "IntelligenceClass":
                creatureStats.OnIntelligenceClassChanged += UpdateDisplay;
                break;
            case "IntelligenceScore":
                creatureStats.OnIntelligenceScoreChanged += UpdateDisplay;
                break;
            case "IntelligenceModifier":
                creatureStats.OnIntelligenceModifierChanged += UpdateDisplay;
                break;

            // Wisdom
            case "WisdomBase":
                creatureStats.OnWisdomBaseChanged += UpdateDisplay;
                break;
            case "WisdomRace":
                creatureStats.OnWisdomRaceChanged += UpdateDisplay;
                break;
            case "WisdomClass":
                creatureStats.OnWisdomClassChanged += UpdateDisplay;
                break;
            case "WisdomScore":
                creatureStats.OnWisdomScoreChanged += UpdateDisplay;
                break;
            case "WisdomModifier":
                creatureStats.OnWisdomModifierChanged += UpdateDisplay;
                break;

            // Charisma
            case "CharismaBase":
                creatureStats.OnCharismaBaseChanged += UpdateDisplay;
                break;
            case "CharismaRace":
                creatureStats.OnCharismaRaceChanged += UpdateDisplay;
                break;
            case "CharismaClass":
                creatureStats.OnCharismaClassChanged += UpdateDisplay;
                break;
            case "CharismaScore":
                creatureStats.OnCharismaScoreChanged += UpdateDisplay;
                break;
            case "CharismaModifier":
                creatureStats.OnCharismaModifierChanged += UpdateDisplay;
                break;

            // Armor
            case "ArmorClass":
                creatureStats.OnArmorClassChanged += UpdateDisplay;
                break;
        }
    }

    void UnsubscribeFromEvent()
    {
        switch (statName)
        {
            // General
            case "Name":
                creatureStats.OnNameChanged -= UpdateDisplayString;
                break;
            case "Class":
                creatureStats.OnClassChanged -= UpdateDisplayClass;
                break;
            case "Race":
                creatureStats.OnRaceChanged -= UpdateDisplayRace;
                break;

            // Health
            case "Health":
                creatureStats.OnHealthChanged -= UpdateDisplay;
                break;
            case "MaxHealth":
                creatureStats.OnMaxHealthChanged -= UpdateDisplay;
                break;

            // Experience
            case "Level":
                creatureStats.OnLevelChanged -= UpdateDisplay;
                break;
            case "Experience":
                creatureStats.OnEXPChanged -= UpdateDisplay;
                break;
            case "MaxExperience":
                creatureStats.OnEXPChanged -= UpdateDisplay;
                break;

            // Stamina
            case "Stamina":
                creatureStats.OnStaminaChanged -= UpdateDisplay;
                break;
            case "MaxStamina":
                creatureStats.OnMaxStaminaChanged -= UpdateDisplay;
                break;

            // Mana
            case "Mana":
                creatureStats.OnManaChanged -= UpdateDisplay;
                break;
            case "MaxMana":
                creatureStats.OnMaxManaChanged -= UpdateDisplay;
                break;

            // Strength
            case "StrengthBase":
                creatureStats.OnStrengthBaseChanged -= UpdateDisplay;
                break;
            case "StrengthRace":
                creatureStats.OnStrengthRaceChanged -= UpdateDisplay;
                break;
            case "StrengthClass":
                creatureStats.OnStrengthClassChanged -= UpdateDisplay;
                break;
            case "StrengthScore":
                creatureStats.OnStrengthScoreChanged -= UpdateDisplay;
                break;
            case "StrengthModifier":
                creatureStats.OnStrengthModifierChanged -= UpdateDisplay;
                break;

            // Dexterity
            case "DexterityBase":
                creatureStats.OnDexterityBaseChanged -= UpdateDisplay;
                break;
            case "DexterityRace":
                creatureStats.OnDexterityRaceChanged -= UpdateDisplay;
                break;
            case "DexterityClass":
                creatureStats.OnDexterityClassChanged -= UpdateDisplay;
                break;
            case "DexterityScore":
                creatureStats.OnDexterityScoreChanged -= UpdateDisplay;
                break;
            case "DexterityModifier":
                creatureStats.OnDexterityModifierChanged -= UpdateDisplay;
                break;

            // Constitution
            case "ConstitutionBase":
                creatureStats.OnConstitutionBaseChanged -= UpdateDisplay;
                break;
            case "ConstitutionRace":
                creatureStats.OnConstitutionRaceChanged -= UpdateDisplay;
                break;
            case "ConstitutionClass":
                creatureStats.OnConstitutionClassChanged -= UpdateDisplay;
                break;
            case "ConstitutionScore":
                creatureStats.OnConstitutionScoreChanged -= UpdateDisplay;
                break;
            case "ConstitutionModifier":
                creatureStats.OnConstitutionModifierChanged -= UpdateDisplay;
                break;

            // Intelligence
            case "IntelligenceBase":
                creatureStats.OnIntelligenceBaseChanged -= UpdateDisplay;
                break;
            case "IntelligenceRace":
                creatureStats.OnIntelligenceRaceChanged -= UpdateDisplay;
                break;
            case "IntelligenceClass":
                creatureStats.OnIntelligenceClassChanged -= UpdateDisplay;
                break;
            case "IntelligenceScore":
                creatureStats.OnIntelligenceScoreChanged -= UpdateDisplay;
                break;
            case "IntelligenceModifier":
                creatureStats.OnIntelligenceModifierChanged -= UpdateDisplay;
                break;

            // Wisdom
            case "WisdomBase":
                creatureStats.OnWisdomBaseChanged -= UpdateDisplay;
                break;
            case "WisdomRace":
                creatureStats.OnWisdomRaceChanged -= UpdateDisplay;
                break;
            case "WisdomClass":
                creatureStats.OnWisdomClassChanged -= UpdateDisplay;
                break;
            case "WisdomScore":
                creatureStats.OnWisdomScoreChanged -= UpdateDisplay;
                break;
            case "WisdomModifier":
                creatureStats.OnWisdomModifierChanged -= UpdateDisplay;
                break;

            // Charisma
            case "CharismaBase":
                creatureStats.OnCharismaBaseChanged -= UpdateDisplay;
                break;
            case "CharismaRace":
                creatureStats.OnCharismaRaceChanged -= UpdateDisplay;
                break;
            case "CharismaClass":
                creatureStats.OnCharismaClassChanged -= UpdateDisplay;
                break;
            case "CharismaScore":
                creatureStats.OnCharismaScoreChanged -= UpdateDisplay;
                break;
            case "CharismaModifier":
                creatureStats.OnCharismaModifierChanged -= UpdateDisplay;
                break;

            // Armor
            case "ArmorClass":
                creatureStats.OnArmorClassChanged -= UpdateDisplay;
                break;
        }
    }

    void UpdateDisplay(float value)
    {
        if (textDisplay != null)
        {
            // Get the correct value based on statName
            float displayValue = GetCurrentStatValue();
            textDisplay.text = prefix + displayValue.ToString(formatString) + suffix;
        }
        else
        {
            Debug.LogError($"TextDisplay is NULL on {gameObject.name}!");
        }
    }

    void UpdateDisplayString(string value)
    {
        if (textDisplay != null)
        {
            textDisplay.text = prefix + value + suffix;
        }
    }

    void UpdateDisplayClass(ClassSO classSO)
    {
        if (textDisplay != null && classSO != null)
        {
            textDisplay.text = prefix + classSO.name + suffix;
        }
    }

    void UpdateDisplayRace(RaceSO raceSO)
    {
        if (textDisplay != null && raceSO != null)
        {
            textDisplay.text = prefix + raceSO.name + suffix;
        }
    }
}
