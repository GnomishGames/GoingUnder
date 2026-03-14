using System;
using UnityEngine;

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