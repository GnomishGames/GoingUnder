using System;
using UnityEngine;

public class Stat
{
    public event Action<int> OnChanged;

    public int BaseValue { get; private set; }
    public int RaceBonus { get; private set; }
    public int ClassBonus { get; private set; }
    public int EquipmentBonus { get; private set; }
    public int LevelBonus { get; private set; }

    public int Score => BaseValue + RaceBonus + ClassBonus + EquipmentBonus + LevelBonus;
    public int Modifier => Mathf.Max(1, (Score - 10) / 2);

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