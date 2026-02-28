using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Object/Item/New Item")]
public class ItemSO : ScriptableObject
{
    [Header("Base Item Stats")]
    public string itemName;
    public Sprite sprite;
    public float itemWeight;
    public SlotType slotType;
}