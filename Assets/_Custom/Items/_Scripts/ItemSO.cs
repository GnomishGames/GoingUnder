using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Object/Item/New Item")]
public class ItemSO : ScriptableObject
{
    [Header("Base Item Stats")]
    public Sprite sprite;
    
    public string itemName;
    public string itemNameSuffix;
    
    public string itemDescription;

    public float itemWeight;
    public int itemValue;
    
    public SlotType slotType;
}