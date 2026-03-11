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

    private Dictionary<string, Action> subscriptions;

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

        InitializeStatSubscriptions();
    }

    private void InitializeStatSubscriptions()
    {
        subscriptions = new Dictionary<string, Action>()
        {
            { "Name", () => creatureStats.OnNameChanged += UpdateDisplayString },
            { "Class", () => creatureStats.OnClassChanged += UpdateDisplayClass },
            { "Race", () => creatureStats.OnRaceChanged += UpdateDisplayRace },

            { "Hitpoints", () => creatureStats.Hitpoints.OnCurrentChanged += UpdateDisplay },
            { "MaxHitpoints", () => creatureStats.Hitpoints.OnMaxChanged += UpdateDisplay },
        };
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
        UnsubscribeFromEvent(statName);
    }

    private void UnsubscribeFromEvent(string statName)
    {
        throw new NotImplementedException();
    }

    void UpdateEmptyDisplay()
    {
        if (statText != null)
        {
            statText.text = "-";
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
                return equipment != null ? equipment.StrengthBonus : 0;
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
                return equipment != null ? equipment.DexterityBonus : 0;
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
                return equipment != null ? equipment.ConstitutionBonus : 0;
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
                return equipment != null ? equipment.IntelligenceBonus : 0;
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
                return equipment != null ? equipment.WisdomBonus : 0;
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
                return equipment != null ? equipment.CharismaBonus : 0;
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
                return equipment != null ? equipment.ArmorAC : 0;
            case "SizeAcBonus":
                return creatureStats.characterRace.sizeAcBonus;

            default:
                Debug.LogWarning($"Unknown stat name: {statName}");
                return 0;
        }
    }
}
