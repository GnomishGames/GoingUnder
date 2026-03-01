using UnityEngine;
using TMPro;

public class ItemValueUpdate : MonoBehaviour
{
    /*
    This script is attached to the item value text in the inventory UI.
    It updates the item value text when the item value changes.
    */

    public TextMeshProUGUI itemValueText; // Reference to the TextMeshProUGUI component for displaying the item value
    
    [Tooltip("parentSlot is the slot that holds the item. I usually just call it \"Slot\".")]
    [SerializeField] private InventoryPanelSlot parentSlot; // Assign this in the inspector
    
    private Inventory inventory;

    void Start()
    {
        // Get the text component on the item value text object
        itemValueText = GetComponent<TextMeshProUGUI>();

        // Get the player's inventory
        GameObject player = GameObject.FindWithTag("Player");

        // Get the player's inventory.
        if (player != null)
        {
            inventory = player.GetComponent<Inventory>();
            
            if (inventory != null)
            {
                // Subscribe to inventory change events
                inventory.OnInventorySlotChanged += OnInventorySlotChanged;
                
                // Initial update
                UpdateText();
            }
            else
            {
                Debug.LogError("ItemValueUpdate could not find Inventory component on Player");
            }
        }
        else
        {
            Debug.LogError("ItemValueUpdate could not find Player GameObject");
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (inventory != null)
        {
            inventory.OnInventorySlotChanged -= OnInventorySlotChanged;
        }
    }

    void OnInventorySlotChanged(int changedSlot)
    {
        // Only update if this is the slot that changed
        if (parentSlot != null && changedSlot == parentSlot.slotNumber)
        {
            UpdateText();
        }
    }

    void UpdateText()
    {
        if (inventory == null || parentSlot == null)
        {
            itemValueText.text = "0";
            return;
        }

        // Get the item in this slot
        ItemSO item = inventory.inventoryItem[parentSlot.slotNumber];

        // Use the GameObject's name as the stat name
        string statName = this.gameObject.name;

        // Update the text based on the GameObject's name
        if (item != null)
        {
            switch (statName)
            {
                case "ItemName":
                    itemValueText.text = item.itemName;
                    break;
                case "ItemWeight":
                    itemValueText.text = item.itemWeight.ToString();
                    break;
                case "ItemValue":
                    itemValueText.text = item.itemValue.ToString();
                    break;
                case "ItemDescription":
                    itemValueText.text = item.itemDescription;
                    break;
                case "ItemNameSuffix":
                    itemValueText.text = item.itemNameSuffix;
                    break;
                case "SlotType":
                    itemValueText.text = item.slotType.ToString();
                    break;
                case "ArmorBonus":
                    if (item is EquipmentSO equipment && equipment.ArmorBonus != 0)
                    {
                        itemValueText.text = "ac: " + equipment.ArmorBonus.ToString();
                    }
                    else
                    {
                        this.gameObject.transform.parent.gameObject.SetActive(false);
                    }
                    break;
                case "StaminaBonus":
                    if (item is EquipmentSO equipment2 && equipment2.StaminaBonus != 0)
                    {
                        itemValueText.text = "sta: " + equipment2.StaminaBonus.ToString();
                    }
                    else
                    {
                        this.gameObject.transform.parent.gameObject.SetActive(false);
                    }
                    break;
                case "ManaBonus":
                    if (item is EquipmentSO equipment3 && equipment3.ManaBonus != 0)
                    {
                        itemValueText.text = "mana: " + equipment3.ManaBonus.ToString();
                    }
                    else
                    {
                        this.gameObject.transform.parent.gameObject.SetActive(false);
                    }
                    break;
                case "StrengthBonus":
                    if (item is EquipmentSO equipment4 && equipment4.StrengthBonus != 0)
                    {
                        itemValueText.text = "str: " + equipment4.StrengthBonus.ToString();
                    }
                    else
                    {
                        //disable the parent GO if the item doesn't have this stat
                        this.gameObject.transform.parent.gameObject.SetActive(false);
                    }
                    break;
                case "ConstitutionBonus":
                    if (item is EquipmentSO equipment5 && equipment5.ConstitutionBonus != 0)
                    {
                        itemValueText.text = "con: " + equipment5.ConstitutionBonus.ToString();
                    }
                    else
                    {
                        //disable the parent GO if the item doesn't have this stat
                        this.gameObject.transform.parent.gameObject.SetActive(false);
                    }
                    break;
                case "DexterityBonus":
                    if (item is EquipmentSO equipment6 && equipment6.DexterityBonus != 0)
                    {
                        itemValueText.text = "dex: " + equipment6.DexterityBonus.ToString();
                    }
                    else
                    {
                        //disable the parent GO if the item doesn't have this stat
                        this.gameObject.transform.parent.gameObject.SetActive(false);
                    }
                    break;
                case "IntelligenceBonus":
                    if (item is EquipmentSO equipment7 && equipment7.IntelligenceBonus != 0)
                    {
                        itemValueText.text = "int: " + equipment7.IntelligenceBonus.ToString();
                    }
                    else
                    {
                        //disable the parent GO if the item doesn't have this stat
                        this.gameObject.transform.parent.gameObject.SetActive(false);
                    }
                    break;
                case "WisdomBonus":
                    if (item is EquipmentSO equipment8 && equipment8.WisdomBonus != 0)
                    {
                        itemValueText.text = "wis: " + equipment8.WisdomBonus.ToString();
                    }
                    else
                    {
                        //disable the parent GO if the item doesn't have this stat
                        this.gameObject.transform.parent.gameObject.SetActive(false);
                    }
                    break;
                case "CharismaBonus":
                    if (item is EquipmentSO equipment9 && equipment9.CharismaBonus != 0)
                    {
                        itemValueText.text = "cha: " + equipment9.CharismaBonus.ToString();
                    }
                    else
                    {
                        //disable the parent GO if the item doesn't have this stat
                        this.gameObject.transform.parent.gameObject.SetActive(false);
                    }
                    break;
                case "Critical":
                    if (item is EquipmentSO equipment12 && (equipment12.CriticalChanceBonus != 0 || equipment12.CriticalDamageBonus != 0))
                    {
                        itemValueText.text = "crit: " + equipment12.CriticalChanceBonus.ToString() + "% @ " + equipment12.CriticalDamageBonus.ToString() + "x";
                    }
                    else
                    {
                        this.gameObject.transform.parent.gameObject.SetActive(false);
                    }
                    break;
                case "DamageType":
                    itemValueText.text = item is WeaponSO weapon ? weapon.damageType.ToString() : "";
                    break;
                case "Die":
                    if (item is WeaponSO weapon3 && (weapon3.DieMultiplier != 0 || weapon3.DieBonus != 0))
                    {
                        itemValueText.text = weapon3.DieMultiplier.ToString() + "d" + weapon3.DieBonus.ToString();
                    }
                    else
                    {
                        this.gameObject.transform.parent.gameObject.SetActive(false);
                    }
                    break;
                default:
                    itemValueText.text = "";
                    break;
            }
        }
        else
        {
            itemValueText.text = "";
        }
    }
}
