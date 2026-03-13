using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Object/Item/New Item")]
public class ItemSO : ScriptableObject
{
    [Header("Base Item Stats")]
    public Sprite sprite;
    
    public string itemName;
    public string itemNameSuffix;
    public ItemRarity itemRarity;
    
    public string itemDescription;

    public float itemWeight;
    public int itemValue;
    
    public SlotType slotType;

        public virtual List<TooltipLine> GetTooltip()
    {
        var lines = new List<TooltipLine>();

        lines.Add(new TooltipLine(itemName, itemNameSuffix, RarityColor()));
        lines.Add(new TooltipLine(itemRarity.ToString(), "", RarityColor()));
        lines.Add(new TooltipLine(itemDescription));
        lines.Add(new TooltipLine("Weight", $"{itemWeight}"));
        lines.Add(new TooltipLine("Value", $"{itemValue}"));

        return lines;
    }

    Color RarityColor()
    {
        switch (itemRarity)
        {
            case ItemRarity.Common: return Color.white;
            case ItemRarity.Uncommon: return Color.green;
            case ItemRarity.Rare: return Color.cyan;
            case ItemRarity.Epic: return new Color(0.7f, 0.3f, 1f);
        }

        return Color.white;
    }
}