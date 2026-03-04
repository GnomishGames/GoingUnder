using System;
using UnityEngine;

public class Stat
{
    public int BaseValue { get; private set; }
    public int RaceBonus { get; private set; }
    public int ClassBonus { get; private set; }
    public int EquipmentBonus { get; private set; }

    public int Score => BaseValue + RaceBonus + ClassBonus + EquipmentBonus;
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

        InitializeStats();
        InitializeArmorClass();

        // Subscribe to equipment changes to update stats and armor class
        equipment.OnEquipmentStatsChanged += OnEquipmentStatsChanged;
        equipment.CalculateStatChanges(); // Calculate initial equipment bonuses
    }

    private void InitializeStats()
    {
        // set initial stats
        InitializeStat(Strength, 10, characterRace.strengthBonus, characterClass.strengthBonus);
        InitializeStat(Dexterity, 10, characterRace.dexterityBonus, characterClass.dexterityBonus);
        InitializeStat(Constitution, 10, characterRace.constitutionBonus, characterClass.constitutionBonus);
        InitializeStat(Intelligence, 10, characterRace.intelligenceBonus, characterClass.intelligenceBonus);
        InitializeStat(Wisdom, 10, characterRace.wisdomBonus, characterClass.wisdomBonus);
        InitializeStat(Charisma, 10, characterRace.charismaBonus, characterClass.charismaBonus);

        Hitpoints.ModifyMax(Constitution.Modifier * characterLevel + 10);
        Hitpoints.ModifyCurrent(Hitpoints.Max);
        Stamina.ModifyMax(10 + (Constitution.Modifier * characterLevel));
        Stamina.ModifyCurrent(Stamina.Max);
        Mana.ModifyMax(10 + (Intelligence.Modifier * characterLevel));
        Mana.ModifyCurrent(Mana.Max);

        armorClass = 10 + equipmentAc + Dexterity.Modifier; // Size modifier can be added later when we have different creature sizes
        OnArmorClassChanged?.Invoke(armorClass);
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

    private void RecalculateArmorClass()
    {
        int newArmorClass = 10 + equipmentAc + Dexterity.Modifier;
        if (newArmorClass == armorClass) return;

        armorClass = newArmorClass;
        OnArmorClassChanged?.Invoke(armorClass);
    }

    void InitializeStat(Stat stat, int baseValue, int raceBonus, int classBonus)
    {
        stat.SetBaseValue(baseValue);
        stat.SetRaceBonus(raceBonus);
        stat.SetClassBonus(classBonus);
        stat.SetEquipmentBonus(0); // Start with no equipment bonus, will be updated when equipment is initialized
    }

    void InitializeArmorClass()
    {
        armorClass = 10 + equipment.ArmorAC + Dexterity.Modifier; // Size modifier can be added later when we have different creature sizes
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

        }
    }
}
