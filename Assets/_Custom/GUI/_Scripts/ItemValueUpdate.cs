using UnityEngine;
using TMPro;

public class ItemValueUpdate : MonoBehaviour
{
    /*
    This script is attached to the item value text in the inventory UI.
    It updates the item value text when the item value changes.
    */

    public TextMeshProUGUI itemValueText; // Reference to the TextMeshProUGUI component for displaying the item value
    [SerializeField] private InventoryPanelSlot parentSlot; // Assign this in the inspector
    private Inventory inventory;

    void Start()
    {
        // Get the text component on the item value text object
        itemValueText = GetComponent<TextMeshProUGUI>();
        
        if (parentSlot == null)
        {
            Debug.LogError($"ItemValueUpdate on {gameObject.name}: parentSlot not assigned in inspector");
            return;
        }

        // Get the player's inventory
        GameObject player = GameObject.FindWithTag("Player");
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
            itemValueText.text = "";
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
