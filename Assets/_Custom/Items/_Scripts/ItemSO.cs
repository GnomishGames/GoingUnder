using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Object/Item/New Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public Sprite sprite;
    public float itemWeight;
    public int itemHealth;
    public SlotType slotType;
}