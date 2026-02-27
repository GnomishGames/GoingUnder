using System;
using UnityEngine;

/*
This script goes on the player at the root level, and is the main script for the inventory. 

It holds the array of items that is the inventory, and has functions for moving items, 
destroying items, equipping and unequipping items, and picking up items. 

It also has events for when an item is equipped or when an inventory slot is changed, 
so that the UI can update accordingly.
*/

public class Inventory : MonoBehaviour
{
    // this is the array of items that IS the inventory
    public ItemSO[] inventoryItem = new ItemSO[12];

    //references
    Equipment equipment;
    Container container;
    Interactable focus;

    public event Action<string> OnEquippedItemChanged;
    public event Action<int> OnInventorySlotChanged;

    private void Awake()
    {
        equipment = GetComponent<Equipment>();
    }

    public void TriggerInventorySlotChanged(int slotNumber)
    {
        OnInventorySlotChanged?.Invoke(slotNumber);
    }

    public void MoveItem(int from, int to)
    {
        var buffer = inventoryItem[to];
        inventoryItem[to] = inventoryItem[from];
        inventoryItem[from] = buffer;
        
        OnInventorySlotChanged?.Invoke(from);
        OnInventorySlotChanged?.Invoke(to);
    }

    public void DestroyItem(int from)
    {
        inventoryItem[from] = null;
        OnInventorySlotChanged?.Invoke(from);
    }

    public void UnEquipArmor(int inventorySlot, int equipmentSlot)
    {
        if (inventoryItem[inventorySlot] == null)
        {
            var buffer = inventoryItem[inventorySlot];
            inventoryItem[inventorySlot] = equipment.armorSOs[equipmentSlot];
            equipment.armorSOs[equipmentSlot] = (ArmorSO)buffer;

            OnEquippedItemChanged?.Invoke(inventoryItem[inventorySlot]?.itemName ?? "");
            OnInventorySlotChanged?.Invoke(inventorySlot);
            equipment.TriggerArmorSlotChanged(equipmentSlot);
        }
        else if (inventoryItem[inventorySlot].slotType == equipment.armorSOs[equipmentSlot].slotType)
        {
            var buffer = inventoryItem[inventorySlot];
            inventoryItem[inventorySlot] = equipment.armorSOs[equipmentSlot];
            equipment.armorSOs[equipmentSlot] = (ArmorSO)buffer;
            
            OnEquippedItemChanged?.Invoke(equipment.armorSOs[equipmentSlot]?.itemName ?? "");
            OnInventorySlotChanged?.Invoke(inventorySlot);
            equipment.TriggerArmorSlotChanged(equipmentSlot);
        }

        equipment.CalculateArmorClass();
    }

    public void UnEquipWeapon(int inventorySlot, int equipmentSlot)
    {
        if (inventoryItem[inventorySlot] == null)
        {
            var buffer = inventoryItem[inventorySlot];
            inventoryItem[inventorySlot] = equipment.weaponSOs[equipmentSlot];
            equipment.weaponSOs[equipmentSlot] = (WeaponSO)buffer;
            
            OnEquippedItemChanged?.Invoke(inventoryItem[inventorySlot]?.itemName ?? "");
            OnInventorySlotChanged?.Invoke(inventorySlot);
            equipment.TriggerWeaponSlotChanged(equipmentSlot);
        }
        else if (inventoryItem[inventorySlot].slotType == equipment.weaponSOs[equipmentSlot].slotType)
        {
            var buffer = inventoryItem[inventorySlot];
            inventoryItem[inventorySlot] = equipment.weaponSOs[equipmentSlot];
            equipment.weaponSOs[equipmentSlot] = (WeaponSO)buffer;
            
            OnEquippedItemChanged?.Invoke(equipment.weaponSOs[equipmentSlot]?.itemName ?? "");
            OnInventorySlotChanged?.Invoke(inventorySlot);
            equipment.TriggerWeaponSlotChanged(equipmentSlot);
        }

        equipment.CalculateArmorClass();
    }

    internal void LootItem(int inventorySlot, int containerSlot)
    {
        
    }

    public void PickupItem(Item item)
    {
        for (int i = 0; i < inventoryItem.Length; i++)
        {
            if (inventoryItem[i] == null)
            {
                inventoryItem[i] = item.GetComponent<Item>().item;
                Destroy(item.gameObject);
                OnInventorySlotChanged?.Invoke(i);
                break;
            }
        }
    }
}