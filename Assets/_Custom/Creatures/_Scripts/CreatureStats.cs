using System;
using UnityEngine;

public class Stat
{
    public int BaseValue { get; private set; }
    public int RaceBonus { get; private set; }
    public int ClassBonus { get; private set; }
    public int EquipmentBonus { get; private set; }
    public int LevelBonus { get; private set; }

    public int Score => BaseValue + RaceBonus + ClassBonus + EquipmentBonus + LevelBonus;
    public int Modifier => Mathf.Max(1, (Score - 10) / 2);

    public event Action<int> OnChanged;

    public void SetBaseValue(int value)
    {
        BaseValue = value;
        OnChanged?.Invoke(Score);
    }

    public void SetRaceBonus(int value)
    {
        RaceBonus = value;
        OnChanged?.Invoke(Score);
    }

    public void SetClassBonus(int value)
    {
        ClassBonus = value;
        OnChanged?.Invoke(Score);
    }

    public void SetEquipmentBonus(int value)
    {
        EquipmentBonus = value;
        OnChanged?.Invoke(Score);
    }

    public void SetLevelBonus(int value)
    {
        LevelBonus = value;
        OnChanged?.Invoke(Score);
    }
}

public class Resource
{
    public int Current { get; private set; }
    public int Max { get; private set; }

    public event Action<int> OnCurrentChanged;
    public event Action<int> OnMaxChanged;

    public void ModifyCurrent(int amount)
    {
        Current += amount;
        Current = Mathf.Clamp(Current, 0, Max);
        OnCurrentChanged?.Invoke(Current);
    }

    public void ModifyMax(int amount)
    {
        Max += amount;
        Max = Mathf.Max(1, Max); // Ensure max is at least 1
        OnMaxChanged?.Invoke(Max);
    }
}

public class CreatureStats : Creature
{
    //General
    public event Action<string> OnNameChanged;
    public event Action<ClassSO> OnClassChanged;
    public event Action<RaceSO> OnRaceChanged;

    //experience events
    public event Action<int> OnEXPChanged;
    public event Action<int> OnMaxExperienceChanged;
    public event Action<int> OnLevelChanged;

    //armor class event
    public event Action<int> OnArmorClassChanged;

    //references
    private Equipment equipment;
    private AnimationController animController;

    //xp Tracking
    public bool gaveXP = false;

    //Max Attributes
    public Resource Hitpoints;
    public Resource Stamina;
    public Resource Mana;

    public int armorClass { get; private set; } //10 + armorBonus + dexMod + sizeMod

    //stats
    public Stat Strength;
    public Stat Dexterity;
    public Stat Constitution;
    public Stat Intelligence;
    public Stat Wisdom;
    public Stat Charisma;

    //Progression
    public int characterLevel { get; private set; } = 1;
    public int experience { get; private set; } // Current XP in this level
    public int maxExperience { get; private set; } // XP needed to reach next level

    //armor and size
    public int equipmentAc { get; private set; }

    // death
    public bool isDead = false;

    //initiative
    public bool inCombat = false;

    void Awake()
    {
        // Get references
        equipment = GetComponent<Equipment>();
        animController = GetComponent<AnimationController>();

        // Initialize resources
        Hitpoints = new Resource();
        Stamina = new Resource();
        Mana = new Resource();

        // Initialize stats
        Strength = new Stat();
        Dexterity = new Stat();
        Constitution = new Stat();
        Intelligence = new Stat();
        Wisdom = new Stat();
        Charisma = new Stat();

        // Set initial stat values
        Strength.SetBaseValue(10);
        Strength.SetRaceBonus(characterRace.strengthBonus);
        Strength.SetClassBonus(characterClass.strengthBonus);

        Dexterity.SetBaseValue(10);
        Dexterity.SetRaceBonus(characterRace.dexterityBonus);
        Dexterity.SetClassBonus(characterClass.dexterityBonus);

        Constitution.SetBaseValue(10);
        Constitution.SetRaceBonus(characterRace.constitutionBonus);
        Constitution.SetClassBonus(characterClass.constitutionBonus);

        Intelligence.SetBaseValue(10);
        Intelligence.SetRaceBonus(characterRace.intelligenceBonus);
        Intelligence.SetClassBonus(characterClass.intelligenceBonus);

        Wisdom.SetBaseValue(10);
        Wisdom.SetRaceBonus(characterRace.wisdomBonus);
        Wisdom.SetClassBonus(characterClass.wisdomBonus);

        Charisma.SetBaseValue(10);
        Charisma.SetRaceBonus(characterRace.charismaBonus);
        Charisma.SetClassBonus(characterClass.charismaBonus);

        // Calculate derived values
        CalculateResources();
        CalculateExperience();
        RecalculateArmorClass();

        // Subscribe to equipment changes to update stats and armor class
        equipment.OnEquipmentStatsChanged += OnEquipmentStatsChanged;
        equipment.CalculateStatChanges(); // Calculate initial equipment bonuses
    }

    void Update()
    {
        //for testing to add experience and 
        if (Input.GetKeyDown(KeyCode.K))
        {
            GainExperience(500);
        }
    }

    private void OnEquipmentStatsChanged(Equipment.EquipmentStatBonuses bonuses)
    {
        equipmentAc = bonuses.ArmorAC;

        Strength.SetEquipmentBonus(bonuses.StrengthBonus);
        Constitution.SetEquipmentBonus(bonuses.ConstitutionBonus);
        Dexterity.SetEquipmentBonus(bonuses.DexterityBonus);
        Intelligence.SetEquipmentBonus(bonuses.IntelligenceBonus);
        Wisdom.SetEquipmentBonus(bonuses.WisdomBonus);
        Charisma.SetEquipmentBonus(bonuses.CharismaBonus);

        RecalculateArmorClass();
    }

    private void CalculateResources()
    {
        // Calculate and set max values for resources based on current stats and level
        int newMaxHP = Constitution.Modifier * characterLevel + 10;
        int newMaxStamina = 10 + (Constitution.Modifier * characterLevel);
        int newMaxMana = 10 + (Intelligence.Modifier * characterLevel);

        Hitpoints.ModifyMax(newMaxHP - Hitpoints.Max);
        Debug.Log($"Max HP modified. New max HP: {Hitpoints.Max} for {this}");
        Stamina.ModifyMax(newMaxStamina - Stamina.Max);
        Debug.Log($"Max Stamina modified. New max Stamina: {Stamina.Max} for {this}");
        Mana.ModifyMax(newMaxMana - Mana.Max);
        Debug.Log($"Max Mana modified. New max Mana: {Mana.Max} for {this}");

        // On first initialization, set current to max
        if (Hitpoints.Current == 0) Hitpoints.ModifyCurrent(Hitpoints.Max);
        if (Stamina.Current == 0) Stamina.ModifyCurrent(Stamina.Max);
        if (Mana.Current == 0) Mana.ModifyCurrent(Mana.Max);
    }

    private void CalculateExperience()
    {
        // Calculate XP needed for next level based on current level
        int xpForCurrentLevel = GetXPRequiredForLevel(characterLevel);
        int xpForNextLevel = GetXPRequiredForLevel(characterLevel + 1);

        maxExperience = xpForNextLevel - xpForCurrentLevel;

        OnMaxExperienceChanged?.Invoke(maxExperience);
        OnEXPChanged?.Invoke(experience);
        OnLevelChanged?.Invoke(characterLevel);
    }

    private void RecalculateArmorClass()
    {
        int newArmorClass = 10 + equipmentAc + Dexterity.Modifier;
        if (newArmorClass == armorClass) return;

        armorClass = newArmorClass;
        OnArmorClassChanged?.Invoke(armorClass);
    }

    public int AttackRoll()
    {
        int attackRoll = UnityEngine.Random.Range(1, 21) + (int)characterLevel + (int)Strength.Modifier;
        return attackRoll;
    }

    internal int RollInitiative()
    {
        int initiativeRoll = UnityEngine.Random.Range(1, 21) + (int)characterLevel + (int)Dexterity.Modifier;
        return initiativeRoll;
    }

    void GainExperience(int amount)
    {
        experience += amount;
        OnEXPChanged?.Invoke(experience);

        // Check if we leveled up (possibly multiple times)
        while (experience >= maxExperience)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        // Subtract the XP needed to level up
        experience -= maxExperience;

        // Increase level
        characterLevel++;
        OnLevelChanged?.Invoke(characterLevel);

        // Recalculate derived values for new level
        UpdateLevelBonuses();
        CalculateExperience();
        CalculateResources();
        Hitpoints.ModifyCurrent(Hitpoints.Max); // Heal to full on level up
        Stamina.ModifyCurrent(Stamina.Max); // Restore stamina to full on level up
        Mana.ModifyCurrent(Mana.Max); // Restore mana to full on level up

        Debug.Log($"{interactableName} leveled up to level {characterLevel}!");
    }

    void UpdateLevelBonuses()
    {
        // For simplicity, let's say each level gives +1 to all stats. This can be expanded to allow for different progression paths.
        int levelBonus = characterLevel - 1; // Level 1 = 0 bonus, Level 2 = +1, etc.

        Strength.SetLevelBonus(levelBonus);
        Dexterity.SetLevelBonus(levelBonus);
        Constitution.SetLevelBonus(levelBonus);
        Intelligence.SetLevelBonus(levelBonus);
        Wisdom.SetLevelBonus(levelBonus);
        Charisma.SetLevelBonus(levelBonus);
    }

    void CalculateLevelFromExp()
    {
        // Calculate XP needed for next level
        int xpForCurrentLevel = GetXPRequiredForLevel(characterLevel);
        int xpForNextLevel = GetXPRequiredForLevel(characterLevel + 1);

        maxExperience = xpForNextLevel - xpForCurrentLevel;

        OnLevelChanged?.Invoke(characterLevel);
        OnEXPChanged?.Invoke(experience);
    }

    // Calculate XP required to reach a specific level
    int GetXPRequiredForLevel(int level)
    {
        // Total XP needed to reach a level: N × (N - 1) × 500
        return level * (level - 1) * 500;
    }

    public void CheckIfDead()
    {
        if (Hitpoints.Current <= 0)
        {
            isDead = true;
            // play death animation

            if (animController != null)
            {
                animController.PlayDeath();
            }
            else
            {
                Debug.LogWarning("No AnimationController found on " + this.name + " to play death animation.");
            }

            Debug.Log($"{this.name} has died.");

            // give xp to player if this is an enemy and xp hasn't been given yet
            if (!gaveXP)
            {
                Transform player = GameObject.FindWithTag("Player").transform;
                CreatureStats playerStats = player.GetComponent<CreatureStats>();
                playerStats.GainExperience(xpToGive);
                gaveXP = true;
            }

        }
    }
}
