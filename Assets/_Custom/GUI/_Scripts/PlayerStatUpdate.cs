using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class PlayerStatUpdate : MonoBehaviour
{
    /*
    This script is attached to text objects in the character panel and updates 
    their display when the corresponding stat changes.
    It uses the name of the GameObject it's attached to determine which stat to 
    display (e.g. "Health", "Mana", "StrengthScore", etc.)
    This is only for the player. There is a separate script that updates the 
    target panel stats when the player targets an enemy.
    */

    Dictionary<string, (Action subscribe, Action unsubscribe)> subscriptions;
    Dictionary<string, Func<int>> statGetters;

    CreatureStats creatureStats;
    Equipment equipment;
    TextMeshProUGUI statText;
    string statName;

    void Awake()
    {
        creatureStats = transform.root.GetComponentInChildren<CreatureStats>(); // get the CreatureStats component from the root of this UI element (which should be the player)
        equipment = transform.root.GetComponentInChildren<Equipment>(); // get the Equipment component from the root of this UI element (which should be the player)
        statName = gameObject.name;// use the gameobjects name as the stat name
        statText = GetComponent<TextMeshProUGUI>(); // get the TextMeshProUGUI component on this gameobject to update the text

        CreateSubscriptions();
        SubscribeToEvent();
        InitializeStatGetters();
    }

    void OnDisable()
    {
        UnsubscribeToEvent();
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

    void SubscribeToEvent()
    {
        if (subscriptions.TryGetValue(statName, out var subscription))
        {
            subscription.subscribe();
        }
    }

    void UnsubscribeToEvent()
    {
        if (subscriptions.TryGetValue(statName, out var subscription))
        {
            subscription.unsubscribe();
        }
    }

    void UpdateDisplayString(string value)
    {
        if (statText != null)
        {
            statText.text = value;
        }
    }

    void UpdateDisplayClass(ClassSO classSO)
    {
        if (statText != null && classSO != null)
        {
            statText.text = classSO.name;
        }
    }

    void UpdateDisplayRace(RaceSO raceSO)
    {
        if (statText != null && raceSO != null)
        {
            statText.text = raceSO.name;
        }
    }

    void OnDestroy()
    {
        UnsubscribeToEvent();
    }

    void UpdateEmptyDisplay()
    {
        if (statText != null)
        {
            statText.text = "-";
        }
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

    int GetCurrentStatValue()
    {
        if (creatureStats == null)
            return 0;

        if (statGetters.TryGetValue(statName, out var getter))
            return getter();

        return 0;
    }
}
