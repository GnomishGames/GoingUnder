using System;
using UnityEngine;

public class CreatureStats : Creature
{
    //General
    public event Action<string> OnNameChanged;
    public event Action<ClassSO> OnClassChanged;
    public event Action<RaceSO> OnRaceChanged;

    //health events
    public event Action<float> OnHealthChanged;
    public event Action<float> OnMaxHealthChanged;

    //experience events
    public event Action<float> OnEXPChanged;
    public event Action<float> OnLevelChanged;

    //stamina events
    public event Action<float> OnStaminaChanged;
    public event Action<float> OnMaxStaminaChanged;

    //mana events
    public event Action<float> OnManaChanged;
    public event Action<float> OnMaxManaChanged;

    //strength events
    public event Action<float> OnStrengthBaseChanged;
    public event Action<float> OnStrengthRaceChanged;
    public event Action<float> OnStrengthClassChanged;
    public event Action<float> OnStrengthScoreChanged;
    public event Action<float> OnStrengthModifierChanged;

    //dexterity events
    public event Action<float> OnDexterityBaseChanged;
    public event Action<float> OnDexterityRaceChanged;
    public event Action<float> OnDexterityClassChanged;
    public event Action<float> OnDexterityScoreChanged;
    public event Action<float> OnDexterityModifierChanged;

    //constitution events
    public event Action<float> OnConstitutionBaseChanged;
    public event Action<float> OnConstitutionRaceChanged;
    public event Action<float> OnConstitutionClassChanged;
    public event Action<float> OnConstitutionScoreChanged;
    public event Action<float> OnConstitutionModifierChanged;

    //intelligence events
    public event Action<float> OnIntelligenceBaseChanged;
    public event Action<float> OnIntelligenceRaceChanged;
    public event Action<float> OnIntelligenceClassChanged;
    public event Action<float> OnIntelligenceScoreChanged;
    public event Action<float> OnIntelligenceModifierChanged;

    //wisdom events
    public event Action<float> OnWisdomBaseChanged;
    public event Action<float> OnWisdomRaceChanged;
    public event Action<float> OnWisdomClassChanged;
    public event Action<float> OnWisdomScoreChanged;
    public event Action<float> OnWisdomModifierChanged;

    //charisma events
    public event Action<float> OnCharismaBaseChanged;
    public event Action<float> OnCharismaRaceChanged;
    public event Action<float> OnCharismaClassChanged;
    public event Action<float> OnCharismaScoreChanged;
    public event Action<float> OnCharismaModifierChanged;

    //armor class event
    public event Action<float> OnArmorClassChanged;

    //references
    private Equipment equipment;

    //xp Tracking
    public bool gaveXP = false;

    //Max Attributes
    public float maxHitpoints { get; private set; }
    public float maxStamina { get; private set; }
    public float maxMana { get; private set; }

    //current stats
    public float currentHitPoints { get; private set; }
    public float currentStamina { get; private set; }
    public float currentMana { get; private set; }
    public float armorClass { get; private set; } //10 + armorBonus + dexMod + sizeMod

    //base stats these are the initial attribute choices during character creation for now default to 10
    public float strengthBase { get; private set; } = 10;
    public float dexterityBase { get; private set; } = 10;
    public float constitutionBase { get; private set; } = 10;
    public float intelligenceBase { get; private set; } = 10;
    public float wisdomBase { get; private set; } = 10;
    public float charismaBase { get; private set; } = 10;

    //Attribute modifiers
    public float strengthModifier { get; private set; } //score minus 10 divided by 2
    public float dexterityModifier { get; private set; }
    public float constitutionModifier { get; private set; }
    public float intelligenceModifier { get; private set; }
    public float wisdomModifier { get; private set; }
    public float charismaModifier { get; private set; }

    //Stat Scores
    public float strengthScore { get; private set; }
    public float dexterityScore { get; private set; }
    public float constitutionScore { get; private set; }
    public float intelligenceScore { get; private set; }
    public float wisdomScore { get; private set; }
    public float charismaScore { get; private set; }

    //Progression
    public float characterLevel { get; private set; } = 1;
    public float experience { get; private set; } // Current XP in this level
    public float maxExperience { get; private set; } // XP needed to reach next level

    //armor and size
    public float equipmentAc { get; private set; }
    public float equipmentStrengthBonus { get; private set; }
    public float equipmentConstitutionBonus { get; private set; }
    public float equipmentDexterityBonus { get; private set; }
    public float equipmentIntelligenceBonus { get; private set; }
    public float equipmentWisdomBonus { get; private set; }
    public float equipmentCharismaBonus { get; private set; }
    public float equipmentStaminaBonus { get; private set; }
    public float equipmentManaBonus { get; private set; }

    // death
    public bool isDead { get; private set; } = false;

    //initiative
    public bool inCombat = false;

    void Awake()
    {
        equipment = GetComponent<Equipment>();
        if (equipment != null)
        {
            equipment.OnEquipmentStatsChanged += UpdateEquipmentStats;
            equipment.CalculateStatChanges();
        }
        else
        {
            CalculateStats();
        }

        ModifyHealth(maxHitpoints);
        ModifyStamina(maxStamina);
        ModifyMana(maxMana);
    }

    void CalculateStats()
    {
        CalculateLevelFromExp();

        //name race and class changes
        OnNameChanged?.Invoke(interactableName);
        OnClassChanged?.Invoke(characterClass);
        OnRaceChanged?.Invoke(characterRace);

        //Strength
        strengthScore = CalculateStatScore(strengthBase, characterRace.strengthBonus, characterClass.strengthBonus, characterLevel) + equipmentStrengthBonus;
        strengthModifier = CalculateStatModifier(strengthScore, characterLevel);

        OnStrengthRaceChanged?.Invoke(characterRace.strengthBonus);
        OnStrengthClassChanged?.Invoke(characterClass.strengthBonus);
        OnStrengthScoreChanged?.Invoke(strengthScore);
        OnStrengthModifierChanged?.Invoke(strengthModifier);
        OnStrengthBaseChanged?.Invoke(strengthBase);

        //Constitution
        constitutionScore = CalculateStatScore(constitutionBase, characterRace.constitutionBonus, characterClass.constitutionBonus, characterLevel) + equipmentConstitutionBonus;
        constitutionModifier = CalculateStatModifier(constitutionScore, characterLevel);

        OnConstitutionRaceChanged?.Invoke(characterRace.constitutionBonus);
        OnConstitutionClassChanged?.Invoke(characterClass.constitutionBonus);
        OnConstitutionScoreChanged?.Invoke(constitutionScore);
        OnConstitutionModifierChanged?.Invoke(constitutionModifier);
        OnConstitutionBaseChanged?.Invoke(constitutionBase);

        //Dexterity
        dexterityScore = CalculateStatScore(dexterityBase, characterRace.dexterityBonus, characterClass.dexterityBonus, characterLevel) + equipmentDexterityBonus;
        dexterityModifier = CalculateStatModifier(dexterityScore, characterLevel);

        OnDexterityRaceChanged?.Invoke(characterRace.dexterityBonus);
        OnDexterityClassChanged?.Invoke(characterClass.dexterityBonus);
        OnDexterityScoreChanged?.Invoke(dexterityScore);
        OnDexterityModifierChanged?.Invoke(dexterityModifier);
        OnDexterityBaseChanged?.Invoke(dexterityBase);

        //Intelligence
        intelligenceScore = CalculateStatScore(intelligenceBase, characterRace.intelligenceBonus, characterClass.intelligenceBonus, characterLevel) + equipmentIntelligenceBonus;
        intelligenceModifier = CalculateStatModifier(intelligenceScore, characterLevel);

        OnIntelligenceRaceChanged?.Invoke(characterRace.intelligenceBonus);
        OnIntelligenceClassChanged?.Invoke(characterClass.intelligenceBonus);
        OnIntelligenceScoreChanged?.Invoke(intelligenceScore);
        OnIntelligenceModifierChanged?.Invoke(intelligenceModifier);
        OnIntelligenceBaseChanged?.Invoke(intelligenceBase);

        //Wisdom
        wisdomScore = CalculateStatScore(wisdomBase, characterRace.wisdomBonus, characterClass.wisdomBonus, characterLevel) + equipmentWisdomBonus;
        wisdomModifier = CalculateStatModifier(wisdomScore, characterLevel);

        OnWisdomRaceChanged?.Invoke(characterRace.wisdomBonus);
        OnWisdomClassChanged?.Invoke(characterClass.wisdomBonus);
        OnWisdomScoreChanged?.Invoke(wisdomScore);
        OnWisdomBaseChanged?.Invoke(wisdomBase);
        OnWisdomModifierChanged?.Invoke(wisdomModifier);

        //Charisma  
        charismaScore = CalculateStatScore(charismaBase, characterRace.charismaBonus, characterClass.charismaBonus, characterLevel) + equipmentCharismaBonus;
        charismaModifier = CalculateStatModifier(charismaScore, characterLevel);

        OnCharismaRaceChanged?.Invoke(characterRace.charismaBonus);
        OnCharismaClassChanged?.Invoke(characterClass.charismaBonus);
        OnCharismaScoreChanged?.Invoke(charismaScore);
        OnCharismaBaseChanged?.Invoke(charismaBase);
        OnCharismaModifierChanged?.Invoke(charismaModifier);

        //Hitpoints
        maxHitpoints = (characterClass.hitDie * characterLevel) + constitutionModifier; //(level * base) + con modifier
        OnMaxHealthChanged?.Invoke(maxHitpoints);

        //Max Stamina
        maxStamina = (characterClass.hitDie * characterLevel) + constitutionModifier + equipmentStaminaBonus; //(level * base) + con modifier
        OnMaxStaminaChanged?.Invoke(maxStamina);

        //Max Mana
        maxMana = (characterClass.manaDie * characterLevel) + intelligenceModifier + equipmentManaBonus; //(level * base) + int modifier
        OnMaxManaChanged?.Invoke(maxMana);

        //Armor Class
        armorClass = 10 + equipmentAc + characterRace.naturalAcBonus + dexterityModifier + characterRace.sizeAcBonus;
        OnArmorClassChanged?.Invoke(armorClass);
    }

    void CalculateLevelFromExp()
    {
        // Calculate XP needed for next level
        float xpForCurrentLevel = GetXPRequiredForLevel(characterLevel);
        float xpForNextLevel = GetXPRequiredForLevel(characterLevel + 1);
        
        maxExperience = xpForNextLevel - xpForCurrentLevel;

        OnLevelChanged?.Invoke(characterLevel);
        OnEXPChanged?.Invoke(experience);
    }

    // Calculate XP required to reach a specific level
    float GetXPRequiredForLevel(float level)
    {
        // Total XP needed to reach a level: N × (N - 1) × 500
        return level * (level - 1) * 500;
    }

    float CalculateStatModifier(float statScore, float characterLevel)
    {
        float statModifier = (statScore - 10) / 2f;

        if (statModifier < 1) //statModifier can't be less than 1
            statModifier = 1;

        return statModifier;
    }

    float CalculateStatScore(float statBase, float raceBonus, float classBonus, float characterLevel)
    {
        float statScore = statBase + characterLevel * raceBonus * classBonus;
        return statScore;
    }

    void UpdateEquipmentStats(Equipment.EquipmentStatBonuses bonuses)
    {
        equipmentAc = bonuses.ArmorAC;
        equipmentStrengthBonus = bonuses.StrengthBonus;
        equipmentConstitutionBonus = bonuses.ConstitutionBonus;
        equipmentDexterityBonus = bonuses.DexterityBonus;
        equipmentIntelligenceBonus = bonuses.IntelligenceBonus;
        equipmentWisdomBonus = bonuses.WisdomBonus;
        equipmentCharismaBonus = bonuses.CharismaBonus;
        equipmentStaminaBonus = bonuses.StaminaBonus;
        equipmentManaBonus = bonuses.ManaBonus;

        CalculateStats(); // Recalculate armor class with new equipment AC
    }

    public void ModifyHealth(float amount)
    {
        currentHitPoints += amount;
        currentHitPoints = Mathf.Clamp(currentHitPoints, 0, maxHitpoints); //keep it between 0 and max

        OnHealthChanged?.Invoke(currentHitPoints);
        OnMaxHealthChanged?.Invoke(maxHitpoints);
    }

    public void SubtractHealth(float amount)
    {
        ModifyHealth(-amount);
    }

    public void AddHealth(float amount)
    {
        ModifyHealth(amount);
    }

    public void ModifyMana(float amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana); //keep it between 0 and max

        OnManaChanged?.Invoke(currentMana);
    }

    public void SubtractMana(float amount)
    {
        ModifyMana(-amount);
    }

    public void AddMana(float amount)
    {
        ModifyMana(amount);
    }

    public void ModifyStamina(float amount)
    {
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina); //keep it between 0 and max

        OnStaminaChanged?.Invoke(currentStamina);
    }

    public void SubtractStamina(float amount)
    {
        ModifyStamina(-amount);
    }

    public void AddStamina(float amount)
    {
        ModifyStamina(amount);
    }

    public int AttackRoll()
    {
        int attackRoll = UnityEngine.Random.Range(1, 21) + (int)characterLevel + (int)strengthModifier;
        return attackRoll;
    }

    internal int RollInitiative()
    {
        int initiativeRoll = UnityEngine.Random.Range(1, 21) + (int)characterLevel + (int)dexterityModifier;
        return initiativeRoll;
    }
}
