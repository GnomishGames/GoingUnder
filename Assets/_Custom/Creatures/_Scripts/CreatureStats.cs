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
    public bool isDead { get; private set; } = false;

    //initiative
    public bool inCombat = false;

    void Awake()
    {
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

        // set initial stats
        Hitpoints.ModifyMax(Constitution.Modifier * characterLevel + 10);
        Hitpoints.ModifyCurrent(Hitpoints.Max);
        Stamina.ModifyMax(10 + (Constitution.Modifier * characterLevel));
        Stamina.ModifyCurrent(Stamina.Max);
        Mana.ModifyMax(10 + (Intelligence.Modifier * characterLevel));
        Mana.ModifyCurrent(Mana.Max);
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
}
