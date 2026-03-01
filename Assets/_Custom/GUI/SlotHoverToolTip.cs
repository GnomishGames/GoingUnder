using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotHoverToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /*
        This script is attached to UI elements that have a hover tooltip.
        It shows the tooltip when the pointer enters the element and hides it when the pointer exits.
        */

    //panel that is hovered over
    public GameObject hoverBoxPanel;

    //get player's inventory to access item data
    private Inventory inventory;

    //bool for enabling/disabling the hover box
    public bool isHoverBoxEnabled = true;

    // look for the image component on this GO
    private Image image;

    // Reference to the parent slot
    private InventoryPanelSlot inventoryParentSlot;
    private EquipmentPanelSlot equipmentParentSlot;
    private WeaponsPanelSlot weaponsPanelSlot;

    void Start()
    {
        if (hoverBoxPanel != null)
        {
            hoverBoxPanel.SetActive(false);
        }

        // Get the player's inventory        
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            inventory = player.GetComponent<Inventory>();
        }
        else
        {
            Debug.LogError("SlotHoverToolTip: Player not found in scene!", gameObject);
        }

        // Look for the Image component on this GameObject
        image = GetComponent<Image>();

        // Find the parent slots
        inventoryParentSlot = GetComponentInParent<InventoryPanelSlot>();
        equipmentParentSlot = GetComponentInParent<EquipmentPanelSlot>();
        weaponsPanelSlot = GetComponentInParent<WeaponsPanelSlot>();

        if (inventoryParentSlot == null && equipmentParentSlot == null && weaponsPanelSlot == null)
        {
            Debug.LogError("SlotHoverToolTip: Could not find InventoryPanelSlot, EquipmentPanelSlot, or WeaponsPanelSlot in parent hierarchy!", gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverBoxPanel == null || !isHoverBoxEnabled || image.sprite == null || inventory == null)
        {
            return;
        }

        int slotNumber = inventoryParentSlot != null ? inventoryParentSlot.slotNumber : equipmentParentSlot != null ? equipmentParentSlot.slotNumber : weaponsPanelSlot != null ? weaponsPanelSlot.slotNumber : -1;
        if (slotNumber == -1)
        {
            return;
        }

        ItemSO item = inventory.inventoryItem[slotNumber];
        if (item == null)
        {
            return;
        }

        hoverBoxPanel.SetActive(true);
        PopulateHoverBoxWithStats(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverBoxPanel != null)
        {
            hoverBoxPanel.SetActive(false);
        }
    }

    private void PopulateHoverBoxWithStats(ItemSO item)
    {
        if (item == null)
            return;

        // Display item name
        UpdateHoverBoxText("ItemName", item.itemName);
        UpdateHoverBoxText("ItemNameSuffix", item.itemNameSuffix);

        // Display all potential stat bonuses
        if (item is EquipmentSO equipmentItem)
        {
            EquipmentSO equipment = (EquipmentSO)item;

            if (equipment.ArmorBonus != 0)
                UpdateHoverBoxText("ArmorBonus", "ac: " + equipment.ArmorBonus.ToString());
            else
                DisableHoverBoxField("ArmorBonus");

            if (equipment.StrengthBonus != 0)
                UpdateHoverBoxText("StrengthBonus", "str: " + equipment.StrengthBonus.ToString());
            else
                DisableHoverBoxField("StrengthBonus");

            if (equipment.ConstitutionBonus != 0)
                UpdateHoverBoxText("ConstitutionBonus", "con: " + equipment.ConstitutionBonus.ToString());
            else
                DisableHoverBoxField("ConstitutionBonus");

            if (equipment.DexterityBonus != 0)
                UpdateHoverBoxText("DexterityBonus", "dex: " + equipment.DexterityBonus.ToString());
            else
                DisableHoverBoxField("DexterityBonus");

            if (equipment.IntelligenceBonus != 0)
                UpdateHoverBoxText("IntelligenceBonus", "int: " + equipment.IntelligenceBonus.ToString());
            else
                DisableHoverBoxField("IntelligenceBonus");

            if (equipment.WisdomBonus != 0)
                UpdateHoverBoxText("WisdomBonus", "wis: " + equipment.WisdomBonus.ToString());
            else
                DisableHoverBoxField("WisdomBonus");

            if (equipment.CharismaBonus != 0)
                UpdateHoverBoxText("CharismaBonus", "cha: " + equipment.CharismaBonus.ToString());
            else
                DisableHoverBoxField("CharismaBonus");

            if (equipment.StaminaBonus != 0)
                UpdateHoverBoxText("StaminaBonus", "sta: " + equipment.StaminaBonus.ToString());
            else
                DisableHoverBoxField("StaminaBonus");

            if (equipment.ManaBonus != 0)
                UpdateHoverBoxText("ManaBonus", "mana: " + equipment.ManaBonus.ToString());
            else
                DisableHoverBoxField("ManaBonus");

            if (equipment.CriticalChanceBonus != 0 || equipment.CriticalDamageBonus != 0)
                UpdateHoverBoxText("Critical", "crit: " + equipment.CriticalChanceBonus.ToString() + "% @ " + equipment.CriticalDamageBonus.ToString() + "x");
            else
                DisableHoverBoxField("Critical");

            // If it's a weapon, show weapon-specific stats
            if (item is WeaponSO weapon)
            {
                UpdateHoverBoxText("DamageType", weapon.damageType.ToString());
                
                if (weapon.DieBonus > 0)
                    UpdateHoverBoxText("Die", weapon.DieMultiplier.ToString() + "d" + weapon.Die.ToString() + "+" + weapon.DieBonus.ToString());
                else if (weapon.DieBonus < 0)
                    UpdateHoverBoxText("Die", weapon.DieMultiplier.ToString() + "d" + weapon.Die.ToString() + weapon.DieBonus.ToString());
                else
                    UpdateHoverBoxText("Die", weapon.DieMultiplier.ToString() + "d" + weapon.Die.ToString());
            }
            else
            {
                DisableHoverBoxField("DamageType");
                DisableHoverBoxField("Die");
            }
        }
        else
        {
            // If not equipment, disable all equipment stat fields
            DisableHoverBoxField("ArmorBonus");
            DisableHoverBoxField("StrengthBonus");
            DisableHoverBoxField("ConstitutionBonus");
            DisableHoverBoxField("DexterityBonus");
            DisableHoverBoxField("IntelligenceBonus");
            DisableHoverBoxField("WisdomBonus");
            DisableHoverBoxField("CharismaBonus");
            DisableHoverBoxField("StaminaBonus");
            DisableHoverBoxField("ManaBonus");
            DisableHoverBoxField("Critical");
            DisableHoverBoxField("DamageType");
            DisableHoverBoxField("Die");
        }

        // Display weight and value
        UpdateHoverBoxText("ItemWeight", item.itemWeight.ToString());
        UpdateHoverBoxText("ItemValue", item.itemValue.ToString());
    }

    public void UpdateHoverBoxText(string statName, string statValue)
    {
        // Recursively search for the child by name
        Transform statTextTransform = FindDeepChild(hoverBoxPanel.transform, statName);
        
        if (statTextTransform != null)
        {
            TextMeshProUGUI textMesh = statTextTransform.GetComponent<TextMeshProUGUI>();
            if (textMesh != null)
            {
                textMesh.text = statValue;
                // Enable the parent GameObject when setting text
                if (statTextTransform.parent != null)
                {
                    statTextTransform.parent.gameObject.SetActive(true);
                }
                Debug.Log($"Updated {statName} to: {statValue}");
            }
            else
            {
                Debug.LogWarning($"Found {statName} but it has no TextMeshProUGUI component");
            }
        }
        else
        {
            Debug.LogWarning($"Could not find child named '{statName}' in hoverBoxPanel hierarchy");
        }
    }

    public void DisableHoverBoxField(string statName)
    {
        // Recursively search for the child by name
        Transform statTextTransform = FindDeepChild(hoverBoxPanel.transform, statName);
        
        if (statTextTransform != null && statTextTransform.parent != null)
        {
            // Disable the parent GameObject
            statTextTransform.parent.gameObject.SetActive(false);
        }
    }

    // Recursively search for a child transform by name
    private Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;
            
            Transform result = FindDeepChild(child, childName);
            if (result != null)
                return result;
        }
        return null;
    }
}
