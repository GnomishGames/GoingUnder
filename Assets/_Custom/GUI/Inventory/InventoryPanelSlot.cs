using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
This script goes on the inventory panel slot prefab, 
and is used to handle dragging and dropping items in the inventory. 

It also updates the slot's icon when the inventory changes. 

It implements the necessary interfaces for drag and drop functionality, 
and has references to the inventory, player, and other panels so that it can 
update them accordingly when an item is dragged and dropped. It also has a reference 
to a drag layer to ensure that dragged items render on top of other UI elements.
*/

public class InventoryPanelSlot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
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

    //array references
    public Inventory inventory;

    //panel references
    InventoryPanel inventoryPanel;
    EquipmentPanel equipmentPanel;
    ContainerPanel containerPanel;

    public int slotNumber;  //manually set on the interface

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();

        inventoryPanel = GetComponentInParent<InventoryPanel>();
        equipmentPanel = transform.root.GetComponentInChildren<EquipmentPanel>();
        containerPanel = GetComponentInParent<ContainerPanel>();

        player = GameObject.FindWithTag("Player").transform;
        inventory = player.GetComponent<Inventory>();

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        //draglayer keeps icons on top when dragging
        dragLayer = GameObject.FindWithTag("DragLayer").transform;

        // Subscribe to inventory change events
        if (inventory != null)
        {
            inventory.OnInventorySlotChanged += OnInventorySlotChanged;
        }

        // Initial update
        UpdateSlotIcons();
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
        if (changedSlot == slotNumber)
        {
            UpdateSlotIcons();
        }
    }

    void UpdateSlotIcons()
    {
        if (inventory == null) return;

        if (inventory.inventoryItem[slotNumber] != null)
        {
            GetComponent<Image>().sprite = inventory.inventoryItem[slotNumber].sprite;
            GetComponent<Image>().color = new Color(255, 255, 255, 1);
            GetComponent<Image>().preserveAspect = true;
        }
        if (inventory.inventoryItem[slotNumber] == null)
        {
            GetComponent<Image>().sprite = null;
            GetComponent<Image>().color = new Color(255, 255, 255, 0);
            GetComponent<Image>().preserveAspect = true;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        inventoryPanel.fromSlot = slotNumber;
        inventoryPanel.fromPanel = "Inventory";
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
        // Convert screen point to local point in the canvas, properly handling Canvas Scaler
        if (canvas != null)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out localPoint);
            rectTransform.position = canvas.transform.TransformPoint(localPoint);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            if (inventoryPanel.fromPanel == "Inventory")
            {
                inventory.MoveItem(inventoryPanel.fromSlot, slotNumber);
            }

            if (equipmentPanel.fromPanel == "Armor")
            {
                inventory.UnEquipArmor(slotNumber, equipmentPanel.fromSlot);
            }

            if (equipmentPanel.fromPanel == "Weapon")
            {
                inventory.UnEquipWeapon(slotNumber, equipmentPanel.fromSlot);
            }

            if (containerPanel != null)
            {
                if (containerPanel.fromPanel == "Container")
                {
                    inventory.LootItem(slotNumber, containerPanel.fromSlot);
                }
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
}