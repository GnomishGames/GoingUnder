using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponsPanelSlot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler, IPointerClickHandler
{
    private RectTransform rectTransform;
    [SerializeField] private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalAnchoredPosition;
    private Transform originalParent;
    private int originalSiblingIndex;
    [Tooltip("Optional transform (e.g. top-level canvas) to reparent the dragged item to while dragging. If empty, script uses the assigned Canvas.transform.")]
    private Transform dragLayer;

    //player reference
    public Transform player;
    PlayerTargeting playerTargeting;

    //panels
    public InventoryPanel inventoryPanel;
    public EquipmentPanel equipmentPanel;
    public WeaponPanel weaponPanel;
    public ContainerPanel containerPanel;

    //class references
    Equipment equipment;

    //dice
    AttackDie attackDie;
    DamageDie damageDie;

    public int slotNumber; //manually set on the interface
    public SlotType slotType;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();

        inventoryPanel = transform.root.GetComponentInChildren<InventoryPanel>(true);
        equipmentPanel = transform.root.GetComponentInChildren<EquipmentPanel>(true);
        weaponPanel = transform.root.GetComponentInChildren<WeaponPanel>(true);
        containerPanel = transform.root.GetComponentInChildren<ContainerPanel>(true);

        if (weaponPanel == null)
        {
            Debug.LogError("WeaponsPanelSlot: WeaponPanel not found in parent hierarchy!");
            return;
        }

        player = GameObject.FindWithTag("Player").transform;

        //set arrays
        equipment = player.GetComponent<Equipment>();
        playerTargeting = player.GetComponent<PlayerTargeting>();

        //set ui elements
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Find AttackDie and DamageDie in the scene
        attackDie = FindFirstObjectByType<AttackDie>();
        damageDie = FindFirstObjectByType<DamageDie>();

        //draglayer keeps icons on top when dragging
        dragLayer = GameObject.FindWithTag("DragLayer").transform;

        // Subscribe to weapon change events
        if (equipment != null)
        {
            equipment.OnWeaponSlotChanged += OnWeaponSlotChanged;
        }

        // Initial update
        UpdateSlotIcons();
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (equipment != null)
        {
            equipment.OnWeaponSlotChanged -= OnWeaponSlotChanged;
        }
    }

    void OnWeaponSlotChanged(int changedSlot)
    {
        // Only update if this is the slot that changed
        if (changedSlot == slotNumber)
        {
            UpdateSlotIcons();
        }
    }

    private void UpdateSlotIcons()
    {
        if (equipment == null) return;

        if (equipment.weaponSOs[slotNumber] != null)
        {
            GetComponent<Image>().sprite = equipment.weaponSOs[slotNumber].sprite;
            GetComponent<Image>().color = new Color(255, 255, 255, 1);
            GetComponent<Image>().preserveAspect = true;
        }
        if (equipment.weaponSOs[slotNumber] == null)
        {
            GetComponent<Image>().sprite = null;
            GetComponent<Image>().color = new Color(255, 255, 255, 0);
            GetComponent<Image>().preserveAspect = true;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        equipmentPanel.fromSlot = slotNumber;
        equipmentPanel.fromPanel = "Weapon";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = .6f;
        // remember the original anchored position so we can restore if the drag is cancelled
        originalAnchoredPosition = rectTransform.anchoredPosition;
        // remember original parent and sibling index so we can restore later
        originalParent = rectTransform.parent;
        originalSiblingIndex = rectTransform.GetSiblingIndex();

        // choose drag layer
        if (dragLayer == null)
        {
            dragLayer = (canvas != null) ? canvas.transform : rectTransform.root;
        }

        // preserve world position and reparent to drag layer so it renders on top
        Vector3 worldPos = rectTransform.position;
        rectTransform.SetParent(dragLayer, false);
        rectTransform.position = worldPos;
        rectTransform.SetAsLastSibling();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        // restore this slot's parent and rect position (in case the drag was not dropped on another slot)
        if (originalParent != null && rectTransform.parent != originalParent)
        {
            // preserve world pos while reparenting
            Vector3 worldPos = rectTransform.position;
            rectTransform.SetParent(originalParent, false);
            rectTransform.position = worldPos;
        }

        rectTransform.anchoredPosition = originalAnchoredPosition;
        rectTransform.SetSiblingIndex(originalSiblingIndex);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out Vector2 localPoint);
            rectTransform.position = canvas.transform.TransformPoint(localPoint);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            if (inventoryPanel.fromPanel == "Inventory")
            {
                equipment.EquipWeapon(inventoryPanel.fromSlot, slotNumber, slotType);
            }

            if (equipmentPanel.fromPanel == "Weapon")
            {
                equipment.MoveWeapon(equipmentPanel.fromSlot, slotNumber, slotType);
            }

            // snap the dragged item's RectTransform to this slot's anchored position and reparent it back
            var draggedRect = eventData.pointerDrag.GetComponent<RectTransform>();
            if (draggedRect != null)
            {
                // reparent to this slot's parent so anchoredPosition aligns correctly
                var targetParent = rectTransform.parent;
                Vector3 worldPos = draggedRect.position;
                draggedRect.SetParent(targetParent, false);
                // preserve world pos to avoid jump, then set anchored to slot
                draggedRect.position = worldPos;
                draggedRect.anchoredPosition = rectTransform.anchoredPosition;
                // restore sibling so it sits in the same slot place
                draggedRect.SetSiblingIndex(rectTransform.GetSiblingIndex());
            }
        }

        if (inventoryPanel != null)
            inventoryPanel.fromPanel = null;
        if (equipmentPanel != null)
            equipmentPanel.fromPanel = null;
        if (containerPanel != null)
            containerPanel.fromPanel = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (equipment.weaponSOs[slotNumber] != null)
        {
            // Do attack
            //check for target null
            if (playerTargeting.currentTarget == null)
            {
                Debug.Log($"Weapon: No target selected.");
                return;
            }

            // target dead?
            if (playerTargeting.currentTarget.GetComponent<CreatureStats>().isDead)
            {
                Debug.Log($"Weapon: Target is too dead for attack.");
                return;
            }

            // Am I dead?
            if (player.GetComponent<CreatureStats>().isDead)
            {
                Debug.Log($"Weapon: Player is too dead to attack.");
                return;
            }

            // is it my turn?
            if (!player.GetComponent<CreatureStats>().inCombat)
            {
                Debug.Log($"Weapon: It's not the player's turn to attack.");
                return;
            }

            // check if attack hits
            int attackRoll = player.GetComponent<CreatureStats>().AttackRoll();
            attackDie.SetDieValue(attackRoll); // Update attack die display

            // calculate damage
            if (attackRoll >= playerTargeting.currentTarget.GetComponent<CreatureStats>().armorClass)
            {
                Debug.Log($"Weapon: Attack hit!" + $" Attack Roll: {attackRoll} vs Target AC: {playerTargeting.currentTarget.GetComponent<CreatureStats>().armorClass}");
                //first get the weapon
                var weapon = equipment.weaponSOs[0]; // Assuming the first weapon slot is used for the attack

                // Now calculate the damage based on the weapon's stats
                int damage = 0;
                for (int i = 0; i < weapon.DieMultiplier; i++)
                {
                    damage += UnityEngine.Random.Range(1, weapon.Die + 1);
                }

                damage += weapon.DieBonus;

                // Apply damage to the target
                playerTargeting.currentTarget.GetComponent<CreatureStats>().SubtractHealth(damage);
                damageDie.SetDieValue(damage); // Update damage die display
            }
            else
            {
                Debug.Log($"Weapon: Attack missed!" + $" Attack Roll: {attackRoll} vs Target AC: {playerTargeting.currentTarget.GetComponent<CreatureStats>().armorClass}");
                damageDie.SetDieValue(0); // Clear damage die display on miss
            }
        }

        Initiative initiative = FindAnyObjectByType<Initiative>();
        if (initiative != null)
        {
            initiative.NextTurn(); // Move to the next combatant in initiative order
        }
    }
}