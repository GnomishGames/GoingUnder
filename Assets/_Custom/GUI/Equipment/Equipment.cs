using System;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public ArmorSO[] armorSOs = new ArmorSO[9];
    public WeaponSO[] weaponSOs = new WeaponSO[4];

    //references
    Inventory inventory;

    //events
    public event Action<float> OnAcChanged;
    public event Action<string> OnEquippedItemChanged;
    public event Action<int> OnArmorSlotChanged;
    public event Action<int> OnWeaponSlotChanged;

    //vars
    public int ArmorAC;
    float timer;
    public GameObject prefab;
    
    private void Awake()
    {
        inventory = GetComponent<Inventory>();
    }

    public void TriggerArmorSlotChanged(int slotNumber)
    {
        OnArmorSlotChanged?.Invoke(slotNumber);
    }

    public void TriggerWeaponSlotChanged(int slotNumber)
    {
        OnWeaponSlotChanged?.Invoke(slotNumber);
    }

    private void Start()
    {
        timer = 1;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            //UpdateEquipped();
            CalculateArmorClass();
            timer = 1;
        }
    }

    public void CalculateArmorClass()
    {
        ArmorAC = 0;
        for (int i = 0; i < armorSOs.Length; i++)
        {
            if (armorSOs[i] != null)
            {
                ArmorAC += armorSOs[i].ArmorBonus;
            }
        }

        for (int i = 0; i < weaponSOs.Length; i++)
        {
            if (weaponSOs[i] != null)
            {
                ArmorAC += weaponSOs[i].ArmorBonus;
            }
        }

        OnAcChanged?.Invoke(ArmorAC); //notify listeners that AC has changed
    }

    public void MoveArmor(int from, int to, SlotType slotType)
    {
        if (armorSOs[from].slotType == slotType)
        {
            var buffer = armorSOs[to];
            armorSOs[to] = armorSOs[from];
            armorSOs[from] = buffer;
            
            OnArmorSlotChanged?.Invoke(from);
            OnArmorSlotChanged?.Invoke(to);
        }
    }

    public void MoveWeapon(int from, int to, SlotType slotType)
    {
        if (weaponSOs[from].slotType == slotType)
        {
            var buffer = weaponSOs[to];
            weaponSOs[to] = weaponSOs[from];
            weaponSOs[from] = buffer;
            
            OnWeaponSlotChanged?.Invoke(from);
            OnWeaponSlotChanged?.Invoke(to);
        }
    }

    public void EquipArmor(int inventorySlot, int equipmentSlot, SlotType slotType)
    {
        if (inventory.inventoryItem[inventorySlot] != null && inventory.inventoryItem[inventorySlot].slotType == slotType)
        {
            var buffer = armorSOs[equipmentSlot];
            armorSOs[equipmentSlot] = (ArmorSO)inventory.inventoryItem[inventorySlot];
            inventory.inventoryItem[inventorySlot] = buffer;

            OnEquippedItemChanged?.Invoke(armorSOs[equipmentSlot].itemName);
            OnArmorSlotChanged?.Invoke(equipmentSlot);
            inventory.TriggerInventorySlotChanged(inventorySlot);

            CalculateArmorClass();
        }
    }
    public void EquipWeapon(int inventorySlot, int equipmentSlot, SlotType slotType)
    {
        if (inventory.inventoryItem[inventorySlot] != null && inventory.inventoryItem[inventorySlot].slotType == slotType)
        {
            var buffer = weaponSOs[equipmentSlot];
            weaponSOs[equipmentSlot] = (WeaponSO)inventory.inventoryItem[inventorySlot];
            inventory.inventoryItem[inventorySlot] = buffer;

            OnEquippedItemChanged?.Invoke(weaponSOs[equipmentSlot].itemName);
            OnWeaponSlotChanged?.Invoke(equipmentSlot);
            inventory.TriggerInventorySlotChanged(inventorySlot);

            CalculateArmorClass();
        }
    }

    public void EquipArmorFromContainer(Container container, int containerSlot, int equipmentSlot, SlotType slotType)
    {
        if (container.containerItem[containerSlot] != null && container.containerItem[containerSlot].slotType == slotType)
        {
            var buffer = armorSOs[equipmentSlot];
            armorSOs[equipmentSlot] = (ArmorSO)container.containerItem[containerSlot];
            container.containerItem[containerSlot] = buffer;

            OnEquippedItemChanged?.Invoke(armorSOs[equipmentSlot].itemName);
            OnArmorSlotChanged?.Invoke(equipmentSlot);
            container.TriggerContainerSlotChanged(containerSlot);

            CalculateArmorClass();
        }
    }

    public void EquipWeaponFromContainer(Container container, int containerSlot, int equipmentSlot, SlotType slotType)
    {
        if (container.containerItem[containerSlot] != null && container.containerItem[containerSlot].slotType == slotType)
        {
            var buffer = weaponSOs[equipmentSlot];
            weaponSOs[equipmentSlot] = (WeaponSO)container.containerItem[containerSlot];
            container.containerItem[containerSlot] = buffer;

            OnEquippedItemChanged?.Invoke(weaponSOs[equipmentSlot].itemName);
            OnWeaponSlotChanged?.Invoke(equipmentSlot);
            container.TriggerContainerSlotChanged(containerSlot);

            CalculateArmorClass();
        }
    }

    void UpdateVisuals(string name)
    {
        if (name != "" && name != null)
        {
            var children = GetComponentsInChildren<Transform>();
            
            foreach (var child in children)
            {
                if (child.name == name)
                {
                    child.GetChild(0).gameObject.SetActive(true);
                    break;
                }
            }
        }
    }
}
