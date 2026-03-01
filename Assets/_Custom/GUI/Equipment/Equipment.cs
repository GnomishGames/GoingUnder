using System;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public struct EquipmentStatBonuses
    {
        public int ArmorAC;
        public int StrengthBonus;
        public int ConstitutionBonus;
        public int DexterityBonus;
        public int IntelligenceBonus;
        public int WisdomBonus;
        public int CharismaBonus;
        public int StaminaBonus;
        public int ManaBonus;
    }

    public ArmorSO[] armorSOs = new ArmorSO[9];
    public WeaponSO[] weaponSOs = new WeaponSO[4];

    //references
    Inventory inventory;

    //events
    public event Action<EquipmentStatBonuses> OnEquipmentStatsChanged;
    public event Action<string> OnEquippedItemChanged;
    public event Action<int> OnArmorSlotChanged;
    public event Action<int> OnWeaponSlotChanged;

    //vars
    public int ArmorAC;
    public int StrengthBonus { get; private set; }
    public int ConstitutionBonus { get; private set; }
    public int DexterityBonus { get; private set; }
    public int IntelligenceBonus { get; private set; }
    public int WisdomBonus { get; private set; }
    public int CharismaBonus { get; private set; }
    public int StaminaBonus { get; private set; }
    public int ManaBonus { get; private set; }

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
            CalculateStatChanges();
            timer = 1;
        }
    }

    public void CalculateStatChanges()
    {
        ArmorAC = 0;
        StrengthBonus = 0;
        ConstitutionBonus = 0;
        DexterityBonus = 0;
        IntelligenceBonus = 0;
        WisdomBonus = 0;
        CharismaBonus = 0;
        StaminaBonus = 0;
        ManaBonus = 0;

        for (int i = 0; i < armorSOs.Length; i++)
        {
            if (armorSOs[i] != null)
            {
                AddBonusesFromItem(armorSOs[i]);
            }
        }

        for (int i = 0; i < weaponSOs.Length; i++)
        {
            if (weaponSOs[i] != null)
            {
                AddBonusesFromItem(weaponSOs[i]);
            }
        }

        OnEquipmentStatsChanged?.Invoke(new EquipmentStatBonuses
        {
            ArmorAC = ArmorAC,
            StrengthBonus = StrengthBonus,
            ConstitutionBonus = ConstitutionBonus,
            DexterityBonus = DexterityBonus,
            IntelligenceBonus = IntelligenceBonus,
            WisdomBonus = WisdomBonus,
            CharismaBonus = CharismaBonus,
            StaminaBonus = StaminaBonus,
            ManaBonus = ManaBonus
        });
    }

    void AddBonusesFromItem(EquipmentSO item)
    {
        ArmorAC += item.ArmorBonus;
        StrengthBonus += item.StrengthBonus;
        ConstitutionBonus += item.ConstitutionBonus;
        DexterityBonus += item.DexterityBonus;
        IntelligenceBonus += item.IntelligenceBonus;
        WisdomBonus += item.WisdomBonus;
        CharismaBonus += item.CharismaBonus;
        StaminaBonus += item.StaminaBonus;
        ManaBonus += item.ManaBonus;
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

            CalculateStatChanges();
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

            CalculateStatChanges();
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

            CalculateStatChanges();
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

            CalculateStatChanges();
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
