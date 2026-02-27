using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContainerPanelSlot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    private RectTransform rectTransform;
    [SerializeField] private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalAnchoredPosition;
    private Transform originalParent;
    private int originalSiblingIndex;
    private Transform dragLayer;

    //player reference
    public Transform player;

    //array references
    Inventory inventory;
    Container container;

    //panel references
    public InventoryPanel inventoryPanel;
    public EquipmentPanel equipmentPanel;
    public ContainerPanel containerPanel;

    Interactable focus;

    public int slotNumber;  //manually set on the interface

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();

        inventory = player.GetComponent<Inventory>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        dragLayer = GameObject.FindWithTag("DragLayer").transform;
    }

    void Update()
    {
        // Check if focus has changed and subscribe to new container
        if (focus != null)
        {
            var newContainer = focus.GetComponent<Container>();
            if (newContainer != container)
            {
                // Unsubscribe from old container
                if (container != null)
                {
                    container.OnContainerSlotChanged -= OnContainerSlotChanged;
                }

                // Subscribe to new container
                container = newContainer;
                if (container != null)
                {
                    container.OnContainerSlotChanged += OnContainerSlotChanged;
                    UpdateSlotIcons();
                }
            }
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (container != null)
        {
            container.OnContainerSlotChanged -= OnContainerSlotChanged;
        }
    }

    void OnContainerSlotChanged(int changedSlot)
    {
        // Only update if this is the slot that changed
        if (changedSlot == slotNumber)
        {
            UpdateSlotIcons();
        }
    }

    private void UpdateSlotIcons()
    {
        if (container.containerItem[slotNumber] != null)
        {
            GetComponent<Image>().sprite = container.containerItem[slotNumber].sprite;
            GetComponent<Image>().color = new Color(255, 255, 255, 1);
        }
        if (container.containerItem[slotNumber] == null)
        {
            GetComponent<Image>().sprite = null;
            GetComponent<Image>().color = new Color(255, 255, 255, 0);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        containerPanel.fromSlot = slotNumber;
        containerPanel.fromPanel = "Container";
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
            if (inventoryPanel != null && inventoryPanel.fromPanel == "Inventory")
            {
                container.UnLootItem(inventory, inventoryPanel.fromSlot, slotNumber);
            }
            if (equipmentPanel != null && equipmentPanel.fromPanel == "Armor")
            {
                var equipment = player.GetComponent<Equipment>();
                if (equipment != null && equipment.armorSOs[equipmentPanel.fromSlot] != null)
                {
                    // Get the slotType from the armor being unequipped
                    var armorSlotType = equipment.armorSOs[equipmentPanel.fromSlot].slotType;
                    container.UnEquipArmorToContainer(equipment, equipmentPanel.fromSlot, slotNumber, armorSlotType);
                }
            }
            if (equipmentPanel != null && equipmentPanel.fromPanel == "Weapon")
            {
                var equipment = player.GetComponent<Equipment>();
                if (equipment != null && equipment.weaponSOs[equipmentPanel.fromSlot] != null)
                {
                    // Get the slotType from the weapon being unequipped
                    var weaponSlotType = equipment.weaponSOs[equipmentPanel.fromSlot].slotType;
                    container.UnEquipWeaponToContainer(equipment, equipmentPanel.fromSlot, slotNumber, weaponSlotType);
                }
            }
            if (containerPanel != null && containerPanel.fromPanel == "Container")
            {
                container.MoveItem(containerPanel.fromSlot, slotNumber);
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