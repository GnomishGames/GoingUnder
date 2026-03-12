using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class TargetStatUpdate : MonoBehaviour
{
    /*
    This script is responsible for updating the target stat display in the UI. 
    It listens for changes in the target's stats and updates the display accordingly. 
    The subscriptions dictionary holds the actions to subscribe and unsubscribe from stat change events, 
    while the statGetters dictionary holds functions to retrieve the current value of each stat.
    */

    Dictionary<string, (Action subscribe, Action unsubscribe)> subscriptions;
    Dictionary<string, Func<int>> statGetters;
    CreatureStats creatureStats;
    PlayerTargeting playerTargeting;
    Equipment equipment;
    TextMeshProUGUI statText;
    string statName;
    bool hasStarted;

    void Awake()
    {
        GetReferences();
        InitializeStatGetters();
    }

    void OnEnable()
    {
        if (!hasStarted)
            return;

        EnsureInitialized();
        SubscribeToEvent(); // subscribe to the relevant stat change event based on the statName
    }

    void Start()
    {
        EnsureInitialized();
        hasStarted = true;
        SubscribeToEvent();
        UpdateDisplay(GetCurrentStatValue()); // ensure display is initialized with current value at start
    }

    void EnsureInitialized()
    {
        if (creatureStats == null || equipment == null || statText == null || string.IsNullOrEmpty(statName))
            GetReferences();

        if (statGetters == null)
            InitializeStatGetters();

        if (subscriptions == null)
            CreateSubscriptions();
    }

    void UpdateDisplay(int value)
    {
        if (statText != null)
        {
            // Recompute by statName because event payloads can differ from the field being displayed.
            int currentValue = GetCurrentStatValue();
            statText.text = currentValue.ToString();
        }
    }

    int GetCurrentStatValue()
    {
        if (creatureStats == null || statGetters == null || !IsCurrentStatReady())
            return 0;

        if (statGetters.TryGetValue(statName, out var getter))
            return getter();

        return 0;
    }

    bool IsCurrentStatReady()
    {
        if (creatureStats == null)
            return false;

        switch (statName)
        {
            case "Hitpoints":
            case "MaxHitpoints":
                return creatureStats.Hitpoints != null;
            case "Stamina":
            case "MaxStamina":
                return creatureStats.Stamina != null;
            case "Mana":
            case "MaxMana":
                return creatureStats.Mana != null;
            default:
                return true;
        }
    }

    void SubscribeToEvent()
    {
        if (subscriptions == null)
            return;

        if (!IsCurrentStatReady())
            return;

        if (subscriptions.TryGetValue(statName, out var subscription))
        {
            subscription.subscribe();
        }
        else
        {
            Debug.LogWarning($"PlayerStatUpdate: No subscription found for stat {statName} on {gameObject.name}");
        }
    }

    void GetReferences()
    {
        playerTargeting = GetComponentInParent<PlayerTargeting>(); // get the PlayerTargeting component from the root of this UI element (which should be the player)
        if (playerTargeting == null)
            Debug.LogError($"PlayerStatUpdate: Could not find PlayerTargeting component in root of {gameObject.name}");
        
        creatureStats = playerTargeting.currentTarget.GetComponentInParent<CreatureStats>(); // get the CreatureStats component from the root of this UI element (which should be the player)
        if (creatureStats == null)
            Debug.LogError($"PlayerStatUpdate: Could not find CreatureStats component in root of {gameObject.name}");

        equipment = playerTargeting.currentTarget.GetComponentInParent<Equipment>(); // get the Equipment component from the root of this UI element (which should be the player)
        if (equipment == null)
            Debug.LogError($"PlayerStatUpdate: Could not find Equipment component in root of {gameObject.name}");

        statName = gameObject.name;// use the gameobjects name as the stat name
        if (string.IsNullOrEmpty(statName))
            Debug.LogError($"PlayerStatUpdate: GameObject name is null or empty for {gameObject.name}");

        statText = GetComponent<TextMeshProUGUI>(); // get the TextMeshProUGUI component on this gameobject to update the text
        if (statText == null)
            Debug.LogError($"PlayerStatUpdate: Could not find TextMeshProUGUI component on {gameObject.name}");
    }

    void UpdateDisplayString(string value)
    {
        if (statText != null)
        {
            statText.text = value;
            Debug.Log($"PlayerStatUpdate: Updated display for {statName} to {value} on {gameObject.name}");
        }
    }

    void UpdateDisplayClass(ClassSO classSO)
    {
        if (statText != null && classSO != null)
        {
            statText.text = classSO.name;
            Debug.Log($"PlayerStatUpdate: Updated display for {statName} to {classSO.name} on {gameObject.name}");
        }
    }

    void UpdateDisplayRace(RaceSO raceSO)
    {
        if (statText != null && raceSO != null)
        {
            statText.text = raceSO.name;
            Debug.Log($"PlayerStatUpdate: Updated display for {statName} to {raceSO.name} on {gameObject.name}");
        }
    }

    private void CreateSubscriptions()
    {
        subscriptions = new Dictionary<string, (Action subscribe, Action unsubscribe)>()
        {
            { "Name", (
                () => creatureStats.OnNameChanged += UpdateDisplayString,
                () => creatureStats.OnNameChanged -= UpdateDisplayString) },
            { "Class", (
                () => creatureStats.OnClassChanged += UpdateDisplayClass,
                () => creatureStats.OnClassChanged -= UpdateDisplayClass) },
            { "Race", (
                () => creatureStats.OnRaceChanged += UpdateDisplayRace,
                () => creatureStats.OnRaceChanged -= UpdateDisplayRace) },
            //HP
            { "Hitpoints", (
                () => creatureStats.Hitpoints.OnCurrentChanged += UpdateDisplay,
                () => creatureStats.Hitpoints.OnCurrentChanged -= UpdateDisplay) },
            { "MaxHitpoints", (
                () => creatureStats.Hitpoints.OnMaxChanged += UpdateDisplay,
                () => creatureStats.Hitpoints.OnMaxChanged -= UpdateDisplay) },
            //XP
            { "Level", (
                () => creatureStats.OnLevelChanged += UpdateDisplay,
                () => creatureStats.OnLevelChanged -= UpdateDisplay) },
            { "Experience", (
                () => creatureStats.OnEXPChanged += UpdateDisplay,
                () => creatureStats.OnEXPChanged -= UpdateDisplay) },
            { "MaxExperience", (
                () => creatureStats.OnMaxExperienceChanged += UpdateDisplay,
                () => creatureStats.OnMaxExperienceChanged -= UpdateDisplay) },
            //STA
            { "Stamina", (
                () => creatureStats.Stamina.OnCurrentChanged += UpdateDisplay,
                () => creatureStats.Stamina.OnCurrentChanged -= UpdateDisplay) },
            { "MaxStamina", (
                () => creatureStats.Stamina.OnMaxChanged += UpdateDisplay,
                () => creatureStats.Stamina.OnMaxChanged -= UpdateDisplay) },
            //MAN
            { "Mana", (
                () => creatureStats.Mana.OnCurrentChanged += UpdateDisplay,
                () => creatureStats.Mana.OnCurrentChanged -= UpdateDisplay) },
            { "MaxMana", (
                () => creatureStats.Mana.OnMaxChanged += UpdateDisplay,
                () => creatureStats.Mana.OnMaxChanged -= UpdateDisplay) },
            //STR
            { "StrengthBase", (
                () => creatureStats.Strength.OnChanged += UpdateDisplay,
                () => creatureStats.Strength.OnChanged -= UpdateDisplay) },
            { "StrengthRace", (
                () => creatureStats.Strength.OnChanged += UpdateDisplay,
                () => creatureStats.Strength.OnChanged -= UpdateDisplay) },
            { "StrengthClass", (
                () => creatureStats.Strength.OnChanged += UpdateDisplay,
                () => creatureStats.Strength.OnChanged -= UpdateDisplay) },
            { "StrengthEquipment", (
                () => equipment.OnEquipmentStatsChanged += (bonuses) => UpdateDisplay(bonuses.StrengthBonus),
                () => equipment.OnEquipmentStatsChanged -= (bonuses) => UpdateDisplay(bonuses.StrengthBonus)) },
            { "StrengthScore", (
                () => creatureStats.Strength.OnChanged += UpdateDisplay,
                () => creatureStats.Strength.OnChanged -= UpdateDisplay) },
            { "StrengthModifier", (
                () => creatureStats.Strength.OnChanged += UpdateDisplay,
                () => creatureStats.Strength.OnChanged -= UpdateDisplay) },
            //DEX
            { "DexterityBase", (
                () => creatureStats.Dexterity.OnChanged += UpdateDisplay,
                () => creatureStats.Dexterity.OnChanged -= UpdateDisplay) },
            { "DexterityRace", (
                () => creatureStats.Dexterity.OnChanged += UpdateDisplay,
                () => creatureStats.Dexterity.OnChanged -= UpdateDisplay) },
            { "DexterityClass", (
                () => creatureStats.Dexterity.OnChanged += UpdateDisplay,
                () => creatureStats.Dexterity.OnChanged -= UpdateDisplay) },
            { "DexterityEquipment", (
                () => equipment.OnEquipmentStatsChanged += (bonuses) => UpdateDisplay(bonuses.DexterityBonus),
                () => equipment.OnEquipmentStatsChanged -= (bonuses) => UpdateDisplay(bonuses.DexterityBonus)) },
            { "DexterityScore", (
                () => creatureStats.Dexterity.OnChanged += UpdateDisplay,
                () => creatureStats.Dexterity.OnChanged -= UpdateDisplay) },
            { "DexterityModifier", (
                () => creatureStats.Dexterity.OnChanged += UpdateDisplay,
                () => creatureStats.Dexterity.OnChanged -= UpdateDisplay) },
            //CON
            { "ConstitutionBase", (
                () => creatureStats.Constitution.OnChanged += UpdateDisplay,
                () => creatureStats.Constitution.OnChanged -= UpdateDisplay) },
            { "ConstitutionRace", (
                () => creatureStats.Constitution.OnChanged += UpdateDisplay,
                () => creatureStats.Constitution.OnChanged -= UpdateDisplay) },
            { "ConstitutionClass", (
                () => creatureStats.Constitution.OnChanged += UpdateDisplay,
                () => creatureStats.Constitution.OnChanged -= UpdateDisplay) },
            { "ConstitutionEquipment", (
                () => equipment.OnEquipmentStatsChanged += (bonuses) => UpdateDisplay(bonuses.ConstitutionBonus),
                () => equipment.OnEquipmentStatsChanged -= (bonuses) => UpdateDisplay(bonuses.ConstitutionBonus)) },
            { "ConstitutionScore", (
                () => creatureStats.Constitution.OnChanged += UpdateDisplay,
                () => creatureStats.Constitution.OnChanged -= UpdateDisplay) },
            { "ConstitutionModifier", (
                () => creatureStats.Constitution.OnChanged += UpdateDisplay,
                () => creatureStats.Constitution.OnChanged -= UpdateDisplay) },
            //INT
            { "IntelligenceBase", (
                () => creatureStats.Intelligence.OnChanged += UpdateDisplay,
                () => creatureStats.Intelligence.OnChanged -= UpdateDisplay) },
            { "IntelligenceRace", (
                () => creatureStats.Intelligence.OnChanged += UpdateDisplay,
                () => creatureStats.Intelligence.OnChanged -= UpdateDisplay) },
            { "IntelligenceClass", (
                () => creatureStats.Intelligence.OnChanged += UpdateDisplay,
                () => creatureStats.Intelligence.OnChanged -= UpdateDisplay) },
            { "IntelligenceEquipment", (
                () => equipment.OnEquipmentStatsChanged += (bonuses) => UpdateDisplay(bonuses.IntelligenceBonus),
                () => equipment.OnEquipmentStatsChanged -= (bonuses) => UpdateDisplay(bonuses.IntelligenceBonus)) },
            { "IntelligenceScore", (
                () => creatureStats.Intelligence.OnChanged += UpdateDisplay,
                () => creatureStats.Intelligence.OnChanged -= UpdateDisplay) },
            { "IntelligenceModifier", (
                () => creatureStats.Intelligence.OnChanged += UpdateDisplay,
                () => creatureStats.Intelligence.OnChanged -= UpdateDisplay) },
            //WIS
            { "WisdomBase", (
                () => creatureStats.Wisdom.OnChanged += UpdateDisplay,
                () => creatureStats.Wisdom.OnChanged -= UpdateDisplay) },
            { "WisdomRace", (
                () => creatureStats.Wisdom.OnChanged += UpdateDisplay,
                () => creatureStats.Wisdom.OnChanged -= UpdateDisplay) },
            { "WisdomClass", (
                () => creatureStats.Wisdom.OnChanged += UpdateDisplay,
                () => creatureStats.Wisdom.OnChanged -= UpdateDisplay) },
            { "WisdomEquipment", (
                () => equipment.OnEquipmentStatsChanged += (bonuses) => UpdateDisplay(bonuses.WisdomBonus),
                () => equipment.OnEquipmentStatsChanged -= (bonuses) => UpdateDisplay(bonuses.WisdomBonus)) },
            { "WisdomScore", (
                () => creatureStats.Wisdom.OnChanged += UpdateDisplay,
                () => creatureStats.Wisdom.OnChanged -= UpdateDisplay) },
            { "WisdomModifier", (
                () => creatureStats.Wisdom.OnChanged += UpdateDisplay,
                () => creatureStats.Wisdom.OnChanged -= UpdateDisplay) },
            //CHA
            { "CharismaBase", (
                () => creatureStats.Charisma.OnChanged += UpdateDisplay,
                () => creatureStats.Charisma.OnChanged -= UpdateDisplay) },
            { "CharismaRace", (
                () => creatureStats.Charisma.OnChanged += UpdateDisplay,
                () => creatureStats.Charisma.OnChanged -= UpdateDisplay) },
            { "CharismaClass", (
                () => creatureStats.Charisma.OnChanged += UpdateDisplay,
                () => creatureStats.Charisma.OnChanged -= UpdateDisplay) },
            { "CharismaEquipment", (
                () => equipment.OnEquipmentStatsChanged += (bonuses) => UpdateDisplay(bonuses.CharismaBonus),
                () => equipment.OnEquipmentStatsChanged -= (bonuses) => UpdateDisplay(bonuses.CharismaBonus)) },
            { "CharismaScore", (
                () => creatureStats.Charisma.OnChanged += UpdateDisplay,
                () => creatureStats.Charisma.OnChanged -= UpdateDisplay) },
            { "CharismaModifier", (
                () => creatureStats.Charisma.OnChanged += UpdateDisplay,
                () => creatureStats.Charisma.OnChanged -= UpdateDisplay) },
            //AC
            { "ArmorClass", (
                () => creatureStats.OnArmorClassChanged += UpdateDisplay,
                () => creatureStats.OnArmorClassChanged -= UpdateDisplay) },
            { "ArmorClassBase", (
                () => creatureStats.OnArmorClassChanged += UpdateDisplay,
                () => creatureStats.OnArmorClassChanged -= UpdateDisplay) },
            { "EquipmentAC", (
                () => equipment.OnEquipmentStatsChanged += (bonuses) => UpdateDisplay(bonuses.ArmorAC),
                () => equipment.OnEquipmentStatsChanged -= (bonuses) => UpdateDisplay(bonuses.ArmorAC)) },
            { "SizeAcBonus", (
                () => creatureStats.OnArmorClassChanged += UpdateDisplay,
                () => creatureStats.OnArmorClassChanged -= UpdateDisplay) },
        };
    }

    void InitializeStatGetters()
    {
        statGetters = new Dictionary<string, Func<int>>()
        {
            { "Hitpoints", () => creatureStats.Hitpoints.Current },
            { "MaxHitpoints", () => creatureStats.Hitpoints.Max },
            { "Level", () => creatureStats.characterLevel },
            { "Experience", () => creatureStats.experience },
            { "MaxExperience", () => creatureStats.maxExperience },
            { "Stamina", () => creatureStats.Stamina.Current },
            { "MaxStamina", () => creatureStats.Stamina.Max },
            { "Mana", () => creatureStats.Mana.Current },
            { "MaxMana", () => creatureStats.Mana.Max },
            { "StrengthBase", () => creatureStats.Strength.BaseValue },
            { "StrengthRace", () => creatureStats.Strength.RaceBonus },
            { "StrengthClass", () => creatureStats.Strength.ClassBonus },
            { "StrengthEquipment", () => equipment != null ? equipment.StrengthBonus : 0 },
            { "StrengthScore", () => creatureStats.Strength.Score },
            { "StrengthModifier", () => creatureStats.Strength.Modifier },
            { "DexterityBase", () => creatureStats.Dexterity.BaseValue },
            { "DexterityRace", () => creatureStats.Dexterity.RaceBonus },
            { "DexterityClass", () => creatureStats.Dexterity.ClassBonus },
            { "DexterityEquipment", () => equipment != null ? equipment.DexterityBonus : 0 },
            { "DexterityScore", () => creatureStats.Dexterity.Score },
            { "DexterityModifier", () => creatureStats.Dexterity.Modifier },
            { "ConstitutionBase", () => creatureStats.Constitution.BaseValue },
            { "ConstitutionRace", () => creatureStats.Constitution.RaceBonus },
            { "ConstitutionClass", () => creatureStats.Constitution.ClassBonus },
            { "ConstitutionEquipment", () => equipment != null ? equipment.ConstitutionBonus : 0 },
            { "ConstitutionScore", () => creatureStats.Constitution.Score },
            { "ConstitutionModifier", () => creatureStats.Constitution.Modifier },
            { "IntelligenceBase", () => creatureStats.Intelligence.BaseValue },
            { "IntelligenceRace", () => creatureStats.Intelligence.RaceBonus },
            { "IntelligenceClass", () => creatureStats.Intelligence.ClassBonus },
            { "IntelligenceEquipment", () => equipment != null ? equipment.IntelligenceBonus : 0 },
            { "IntelligenceScore", () => creatureStats.Intelligence.Score },
            { "IntelligenceModifier", () => creatureStats.Intelligence.Modifier },
            { "WisdomBase", () => creatureStats.Wisdom.BaseValue },
            { "WisdomRace", () => creatureStats.Wisdom.RaceBonus },
            { "WisdomClass", () => creatureStats.Wisdom.ClassBonus },
            { "WisdomEquipment", () => equipment != null ? equipment.WisdomBonus : 0 },
            { "WisdomScore", () => creatureStats.Wisdom.Score },
            { "WisdomModifier", () => creatureStats.Wisdom.Modifier },
            { "CharismaBase", () => creatureStats.Charisma.BaseValue },
            { "CharismaRace", () => creatureStats.Charisma.RaceBonus },
            { "CharismaClass", () => creatureStats.Charisma.ClassBonus },
            { "CharismaEquipment", () => equipment != null ? equipment.CharismaBonus : 0 },
            { "CharismaScore", () => creatureStats.Charisma.Score },
            { "CharismaModifier", () => creatureStats.Charisma.Modifier },
            { "ArmorClass", () => creatureStats.armorClass },
            { "ArmorClassBase", () => creatureStats.armorClassBase },
            { "EquipmentAC", () => equipment != null ? equipment.ArmorAC :  0 },
            { "SizeAcBonus", () => creatureStats.characterRace.sizeAcBonus },
        };
    }

}
