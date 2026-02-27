using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : Interactable
{
    public ItemSO[] containerItem = new ItemSO[8];

    public event Action<string> OnNameChanged;
    public event Action<int> OnContainerSlotChanged;

    Inventory inventory;
    public Transform player;

    public void TriggerContainerSlotChanged(int slotNumber)
    {
        OnContainerSlotChanged?.Invoke(slotNumber);
    }



    void OnEnable()
    {
        OnNameChanged?.Invoke(interactableName);
    }

    internal void UnLootItem(Inventory inventory, int inventorySlot, int containerSlot)
    {
        var buffer = containerItem[containerSlot];
        containerItem[containerSlot] = inventory.inventoryItem[inventorySlot];
        inventory.inventoryItem[inventorySlot] = buffer;
        
        OnContainerSlotChanged?.Invoke(containerSlot);
        inventory.TriggerInventorySlotChanged(inventorySlot);
    }

    internal void MoveItem(int fromSlot, int toSlot)
    {
        var buffer = containerItem[toSlot];
        containerItem[toSlot] = containerItem[fromSlot];
        containerItem[fromSlot] = buffer;
        
        OnContainerSlotChanged?.Invoke(fromSlot);
        OnContainerSlotChanged?.Invoke(toSlot);
    }

    internal void UnEquipArmorToContainer(Equipment equipment, int equipmentSlot, int containerSlot, SlotType slotType)
    {
        var buffer = containerItem[containerSlot];
        // Only swap if the container item is null or compatible with the armor slot
        if (buffer == null || buffer.slotType == slotType)
        {
            containerItem[containerSlot] = equipment.armorSOs[equipmentSlot];
            equipment.armorSOs[equipmentSlot] = (ArmorSO)buffer;
            
            OnContainerSlotChanged?.Invoke(containerSlot);
            equipment.TriggerArmorSlotChanged(equipmentSlot);
        }
    }

    internal void UnEquipWeaponToContainer(Equipment equipment, int equipmentSlot, int containerSlot, SlotType slotType)
    {
        var buffer = containerItem[containerSlot];
        // Only swap if the container item is null or compatible with the weapon slot
        if (buffer == null || buffer.slotType == slotType)
        {
            containerItem[containerSlot] = equipment.weaponSOs[equipmentSlot];
            equipment.weaponSOs[equipmentSlot] = (WeaponSO)buffer;
            
            OnContainerSlotChanged?.Invoke(containerSlot);
            equipment.TriggerWeaponSlotChanged(equipmentSlot);
        }
    }
}