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
    //CreatureStats playerStats;
    PlayerTargeting playerTargeting;
    Equipment playerEquipment;
    TextMeshProUGUI textDisplay;
    [SerializeField] private bool useTargetStats = false;
    [SerializeField] private string prefix = "";
    [SerializeField] private string suffix = "";
    [SerializeField] private string noTargetValue = "-";
    [SerializeField] private string formatString = "F0"; // "F0" for whole numbers, "F1" for one decimal, etc.

    private string statName;
    private bool hasStarted = false;

    void Awake()
    {
        // Use the GameObject's name as the stat name
        statName = this.gameObject.name;

        // Get text display reference once
        textDisplay = GetComponent<TextMeshProUGUI>();

        player = transform.root;
        playerTargeting = player.GetComponent<PlayerTargeting>();
        playerEquipment = player.GetComponent<Equipment>();
    }

    void OnEnable()
    {
        if (!hasStarted) //for some reason OnEnable can occur before Start, so we have to check for it
            return;
        GetCreatureStats();
    }

    void Start()
    {
        hasStarted = true;
        GetCreatureStats();
    }

    private void GetCreatureStats()
    {
        if (useTargetStats)
        {
            CreatureStats targetStats = null;

            if (playerTargeting != null)
            {
                playerTargeting.OnTargetChanged += HandleTargetChanged;

                if (playerTargeting.currentTarget != null)
                {
                    targetStats = playerTargeting.currentTarget.GetComponent<CreatureStats>();
                }
                else
                {
                    Debug.LogWarning($"UIStatUpdate: No current target or CreatureStats found on target for {gameObject.name}");
                }
            }

            SetCreatureStats(targetStats);
        }
        else
        {
            if (player != null)
            {

                creatureStats = player.GetComponent<CreatureStats>();
                SetCreatureStats(creatureStats);
            }
        }
    }

    void HandleTargetChanged(CreatureStats newTargetStats)
    {
        SetCreatureStats(newTargetStats);
    }

    void SetCreatureStats(CreatureStats newStats)
    {
        if (creatureStats == newStats)
        {
            UpdateDisplayFromStatName();
            return;
        }

        if (creatureStats != null)
        {
            UnsubscribeFromEvent();
        }

        creatureStats = newStats;

        if (creatureStats != null)
        {
            SubscribeToEvent();

            // Initialize display with current value after stats are calculated
            UpdateDisplayFromStatName();
        }
        else
        {
            UpdateEmptyDisplay();
        }
    }

    void UpdateDisplayFromStatName()
    {
        if (creatureStats == null)
        {
            UpdateEmptyDisplay();
            return;
        }

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
                int value = GetCurrentStatValue();
                UpdateDisplay(value);
                break;
        }
    }

    int GetCurrentStatValue()
    {
        if (creatureStats == null)
            return 0;

        switch (statName)
        {
            // Health
            case "Hitpoints":
                return creatureStats.Hitpoints.Current;
            case "MaxHitpoints":
                return creatureStats.Hitpoints.Max;

            // Experience
            case "Level":
                return creatureStats.characterLevel;
            case "Experience":
                return creatureStats.experience;
            case "MaxExperience":
                return creatureStats.maxExperience;

            // Stamina
            case "Stamina":
                return creatureStats.Stamina.Current;
            case "MaxStamina":
                return creatureStats.Stamina.Max;

            // Mana
            case "Mana":
                return creatureStats.Mana.Current;
            case "MaxMana":
                return creatureStats.Mana.Max;

            // Strength
            case "StrengthBase":
                return creatureStats.Strength.BaseValue;
            case "StrengthRace":
                return creatureStats.Strength.RaceBonus;
            case "StrengthClass":
                return creatureStats.Strength.ClassBonus;
            case "StrengthEquipment":
                return playerEquipment.StrengthBonus;
            case "StrengthScore":
                return creatureStats.Strength.Score;
            case "StrengthModifier":
                return creatureStats.Strength.Modifier;

            // Dexterity
            case "DexterityBase":
                return creatureStats.Dexterity.BaseValue;
            case "DexterityRace":
                return creatureStats.Dexterity.RaceBonus;
            case "DexterityClass":
                return creatureStats.Dexterity.ClassBonus;
            case "DexterityEquipment":
                return playerEquipment.DexterityBonus;
            case "DexterityScore":
                return creatureStats.Dexterity.Score;
            case "DexterityModifier":
                return creatureStats.Dexterity.Modifier;

            // Constitution
            case "ConstitutionBase":
                return creatureStats.Constitution.BaseValue;
            case "ConstitutionRace":
                return creatureStats.Constitution.RaceBonus;
            case "ConstitutionClass":
                return creatureStats.Constitution.ClassBonus;
            case "ConstitutionEquipment":
                return playerEquipment.ConstitutionBonus;
            case "ConstitutionScore":
                return creatureStats.Constitution.Score;
            case "ConstitutionModifier":
                return creatureStats.Constitution.Modifier;

            // Intelligence
            case "IntelligenceBase":
                return creatureStats.Intelligence.BaseValue;
            case "IntelligenceRace":
                return creatureStats.Intelligence.RaceBonus;
            case "IntelligenceClass":
                return creatureStats.Intelligence.ClassBonus;
            case "IntelligenceEquipment":
                return playerEquipment.IntelligenceBonus;
            case "IntelligenceScore":
                return creatureStats.Intelligence.Score;
            case "IntelligenceModifier":
                return creatureStats.Intelligence.Modifier;

            // Wisdom
            case "WisdomBase":
                return creatureStats.Wisdom.BaseValue;
            case "WisdomRace":
                return creatureStats.Wisdom.RaceBonus;
            case "WisdomClass":
                return creatureStats.Wisdom.ClassBonus;
            case "WisdomEquipment":
                return playerEquipment.WisdomBonus;
            case "WisdomScore":
                return creatureStats.Wisdom.Score;
            case "WisdomModifier":
                return creatureStats.Wisdom.Modifier;

            // Charisma
            case "CharismaBase":
                return creatureStats.Charisma.BaseValue;
            case "CharismaRace":
                return creatureStats.Charisma.RaceBonus;
            case "CharismaClass":
                return creatureStats.Charisma.ClassBonus;
            case "CharismaEquipment":
                return playerEquipment.CharismaBonus;
            case "CharismaScore":
                return creatureStats.Charisma.Score;
            case "CharismaModifier":
                return creatureStats.Charisma.Modifier;

            // Armor
            case "ArmorClass":
                return creatureStats.armorClass;
            case "ArmorClassBase":
                return creatureStats.armorClassBase;
            case "EquipmentAC":
                return playerEquipment.ArmorAC;
            case "SizeAcBonus":
                return creatureStats.characterRace.sizeAcBonus;

            default:
                Debug.LogWarning($"Unknown stat name: {statName}");
                return 0;
        }
    }

    void OnDisable()
    {
        if (playerTargeting != null)
        {
            playerTargeting.OnTargetChanged -= HandleTargetChanged;
        }

        if (creatureStats != null)
        {
            UnsubscribeFromEvent();
            creatureStats = null;
        }
    }

    void SubscribeToEvent()
    {
        if (creatureStats == null)
            return;

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
            case "Hitpoints":
                creatureStats.Hitpoints.OnCurrentChanged += UpdateDisplay;
                break;
            case "MaxHitpoints":
                creatureStats.Hitpoints.OnMaxChanged += UpdateDisplay;
                break;

            // Experience
            case "Level":
                creatureStats.OnLevelChanged += UpdateDisplay;
                break;
            case "Experience":
                creatureStats.OnEXPChanged += UpdateDisplay;
                break;
            case "MaxExperience":
                creatureStats.OnMaxExperienceChanged += UpdateDisplay;
                break;

            // Stamina
            case "Stamina":
                creatureStats.Stamina.OnCurrentChanged += UpdateDisplay;
                break;
            case "MaxStamina":
                creatureStats.Stamina.OnMaxChanged += UpdateDisplay;
                break;

            // Mana
            case "Mana":
                creatureStats.Mana.OnCurrentChanged += UpdateDisplay;
                break;
            case "MaxMana":
                creatureStats.Mana.OnMaxChanged += UpdateDisplay;
                break;

            // Strength
            case "StrengthBase":
                creatureStats.Strength.OnChanged += UpdateDisplay;
                break;
            case "StrengthRace":
                creatureStats.Strength.OnChanged += UpdateDisplay;
                break;
            case "StrengthClass":
                creatureStats.Strength.OnChanged += UpdateDisplay;
                break;
            case "StrengthScore":
                creatureStats.Strength.OnChanged += UpdateDisplay;
                break;
            case "StrengthModifier":
                creatureStats.Strength.OnChanged += UpdateDisplay;
                break;

            // Dexterity
            case "DexterityBase":
                creatureStats.Dexterity.OnChanged += UpdateDisplay;
                break;
            case "DexterityRace":
                creatureStats.Dexterity.OnChanged += UpdateDisplay;
                break;
            case "DexterityClass":
                creatureStats.Dexterity.OnChanged += UpdateDisplay;
                break;
            case "DexterityScore":
                creatureStats.Dexterity.OnChanged += UpdateDisplay;
                break;
            case "DexterityModifier":
                creatureStats.Dexterity.OnChanged += UpdateDisplay;
                break;

            // Constitution
            case "ConstitutionBase":
                creatureStats.Constitution.OnChanged += UpdateDisplay;
                break;
            case "ConstitutionRace":
                creatureStats.Constitution.OnChanged += UpdateDisplay;
                break;
            case "ConstitutionClass":
                creatureStats.Constitution.OnChanged += UpdateDisplay;
                break;
            case "ConstitutionScore":
                creatureStats.Constitution.OnChanged += UpdateDisplay;
                break;
            case "ConstitutionModifier":
                creatureStats.Constitution.OnChanged += UpdateDisplay;
                break;

            // Intelligence
            case "IntelligenceBase":
                creatureStats.Intelligence.OnChanged += UpdateDisplay;
                break;
            case "IntelligenceRace":
                creatureStats.Intelligence.OnChanged += UpdateDisplay;
                break;
            case "IntelligenceClass":
                creatureStats.Intelligence.OnChanged += UpdateDisplay;
                break;
            case "IntelligenceScore":
                creatureStats.Intelligence.OnChanged += UpdateDisplay;
                break;
            case "IntelligenceModifier":
                creatureStats.Intelligence.OnChanged += UpdateDisplay;
                break;

            // Wisdom
            case "WisdomBase":
                creatureStats.Wisdom.OnChanged += UpdateDisplay;
                break;
            case "WisdomRace":
                creatureStats.Wisdom.OnChanged += UpdateDisplay;
                break;
            case "WisdomClass":
                creatureStats.Wisdom.OnChanged += UpdateDisplay;
                break;
            case "WisdomScore":
                creatureStats.Wisdom.OnChanged += UpdateDisplay;
                break;
            case "WisdomModifier":
                creatureStats.Wisdom.OnChanged += UpdateDisplay;
                break;

            // Charisma
            case "CharismaBase":
                creatureStats.Charisma.OnChanged += UpdateDisplay;
                break;
            case "CharismaRace":
                creatureStats.Charisma.OnChanged += UpdateDisplay;
                break;
            case "CharismaClass":
                creatureStats.Charisma.OnChanged += UpdateDisplay;
                break;
            case "CharismaScore":
                creatureStats.Charisma.OnChanged += UpdateDisplay;
                break;
            case "CharismaModifier":
                creatureStats.Charisma.OnChanged += UpdateDisplay;
                break;

            // Armor
            case "ArmorClass":
                creatureStats.OnArmorClassChanged += UpdateDisplay;
                break;
            case "ArmorClassBase":
                creatureStats.OnArmorClassChanged += UpdateDisplay;
                break;
            case "EquipmentAC":
                creatureStats.OnArmorClassChanged += UpdateDisplay;
                break;
            case "SizeAcBonus":
                creatureStats.OnArmorClassChanged += UpdateDisplay;
                break;
        }
    }

    void UnsubscribeFromEvent()
    {
        if (creatureStats == null)
            return;

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

            // Hitpoints
            case "Hitpoints":
                creatureStats.Hitpoints.OnCurrentChanged -= UpdateDisplay;
                break;
            case "MaxHitpoints":
                creatureStats.Hitpoints.OnMaxChanged -= UpdateDisplay;
                break;

            // Experience
            case "Level":
                creatureStats.OnLevelChanged -= UpdateDisplay;
                break;
            case "Experience":
                creatureStats.OnEXPChanged -= UpdateDisplay;
                break;
            case "MaxExperience":
                creatureStats.OnMaxExperienceChanged -= UpdateDisplay;
                break;

            // Stamina
            case "Stamina":
                creatureStats.Stamina.OnCurrentChanged -= UpdateDisplay;
                break;
            case "MaxStamina":
                creatureStats.Stamina.OnMaxChanged -= UpdateDisplay;
                break;

            // Mana
            case "Mana":
                creatureStats.Mana.OnCurrentChanged -= UpdateDisplay;
                break;
            case "MaxMana":
                creatureStats.Mana.OnMaxChanged -= UpdateDisplay;
                break;

            // Strength
            case "StrengthBase":
                creatureStats.Strength.OnChanged -= UpdateDisplay;
                break;
            case "StrengthRace":
                creatureStats.Strength.OnChanged -= UpdateDisplay;
                break;
            case "StrengthClass":
                creatureStats.Strength.OnChanged -= UpdateDisplay;
                break;
            case "StrengthScore":
                creatureStats.Strength.OnChanged -= UpdateDisplay;
                break;
            case "StrengthModifier":
                creatureStats.Strength.OnChanged -= UpdateDisplay;
                break;

            // Dexterity
            case "DexterityBase":
                creatureStats.Dexterity.OnChanged -= UpdateDisplay;
                break;
            case "DexterityRace":
                creatureStats.Dexterity.OnChanged -= UpdateDisplay;
                break;
            case "DexterityClass":
                creatureStats.Dexterity.OnChanged -= UpdateDisplay;
                break;
            case "DexterityScore":
                creatureStats.Dexterity.OnChanged -= UpdateDisplay;
                break;
            case "DexterityModifier":
                creatureStats.Dexterity.OnChanged -= UpdateDisplay;
                break;

            // Constitution
            case "ConstitutionBase":
                creatureStats.Constitution.OnChanged -= UpdateDisplay;
                break;
            case "ConstitutionRace":
                creatureStats.Constitution.OnChanged -= UpdateDisplay;
                break;
            case "ConstitutionClass":
                creatureStats.Constitution.OnChanged -= UpdateDisplay;
                break;
            case "ConstitutionScore":
                creatureStats.Constitution.OnChanged -= UpdateDisplay;
                break;
            case "ConstitutionModifier":
                creatureStats.Constitution.OnChanged -= UpdateDisplay;
                break;

            // Intelligence
            case "IntelligenceBase":
                creatureStats.Intelligence.OnChanged -= UpdateDisplay;
                break;
            case "IntelligenceRace":
                creatureStats.Intelligence.OnChanged -= UpdateDisplay;
                break;
            case "IntelligenceClass":
                creatureStats.Intelligence.OnChanged -= UpdateDisplay;
                break;
            case "IntelligenceScore":
                creatureStats.Intelligence.OnChanged -= UpdateDisplay;
                break;
            case "IntelligenceModifier":
                creatureStats.Intelligence.OnChanged -= UpdateDisplay;
                break;

            // Wisdom
            case "WisdomBase":
                creatureStats.Wisdom.OnChanged -= UpdateDisplay;
                break;
            case "WisdomRace":
                creatureStats.Wisdom.OnChanged -= UpdateDisplay;
                break;
            case "WisdomClass":
                creatureStats.Wisdom.OnChanged -= UpdateDisplay;
                break;
            case "WisdomScore":
                creatureStats.Wisdom.OnChanged -= UpdateDisplay;
                break;
            case "WisdomModifier":
                creatureStats.Wisdom.OnChanged -= UpdateDisplay;
                break;

            // Charisma
            case "CharismaBase":
                creatureStats.Charisma.OnChanged -= UpdateDisplay;
                break;
            case "CharismaRace":
                creatureStats.Charisma.OnChanged -= UpdateDisplay;
                break;
            case "CharismaClass":
                creatureStats.Charisma.OnChanged -= UpdateDisplay;
                break;
            case "CharismaScore":
                creatureStats.Charisma.OnChanged -= UpdateDisplay;
                break;
            case "CharismaModifier":
                creatureStats.Charisma.OnChanged -= UpdateDisplay;
                break;

            // Armor
            case "ArmorClass":
                creatureStats.OnArmorClassChanged -= UpdateDisplay;
                break;
            case "ArmorClassBase":
                creatureStats.OnArmorClassChanged -= UpdateDisplay;
                break;
            case "EquipmentAC":
                creatureStats.OnArmorClassChanged -= UpdateDisplay;
                break;
            case "SizeAcBonus":
                creatureStats.OnArmorClassChanged -= UpdateDisplay;
                break;
        }
    }

    void UpdateDisplay(int value)
    {
        if (textDisplay != null)
        {
            // Get the correct value based on statName
            textDisplay.text = prefix + value.ToString(formatString) + suffix;
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

    void UpdateEmptyDisplay()
    {
        if (textDisplay != null)
        {
            textDisplay.text = prefix + noTargetValue + suffix;
        }
    }
}
