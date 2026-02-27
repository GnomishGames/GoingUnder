using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentPanelSlot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    //player reference
    public Transform player;

    //ui elements
    private RectTransform rectTransform;
    [SerializeField] private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalAnchoredPosition;
    private Transform originalParent;
    private int originalSiblingIndex;
    private Transform dragLayer;

    //panels
    public InventoryPanel inventoryPanel;
    public EquipmentPanel equipmentPanel;
    public ContainerPanel containerPanel;

    //source references for drag/drop
    Inventory sourceInventory;
    Equipment sourceEquipment;
    Transform sourcePlayer;
    Container container;

    //source slot references for drag/drop
    EquipmentPanelSlot sourceEquipmentSlot;
    InventoryPanelSlot sourceSlot;

    //array reference
    Equipment equipment;

    //slot stuff
    public int slotNumber; //manually set on the interface
    public SlotType slotType;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();

        inventoryPanel = transform.root.GetComponentInChildren<InventoryPanel>();
        equipmentPanel = GetComponentInParent<EquipmentPanel>();
        containerPanel = GetComponentInParent<ContainerPanel>();

        if (equipmentPanel == null)
        {
            Debug.LogError("EquipmentPanelSlot: EquipmentPanel not found in parent hierarchy!");
            return;
        }

        player = GameObject.FindWithTag("Player").transform;

        //set arrays
        equipment = player.GetComponent<Equipment>();

        //set ui elements
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        //draglayer keeps icons on top when dragging
        dragLayer = GameObject.FindWithTag("DragLayer").transform;

        // Subscribe to equipment change events
        if (equipment != null)
        {
            equipment.OnArmorSlotChanged += OnArmorSlotChanged;
        }

        // Initial update
        UpdateSlotIcons();
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (equipment != null)
        {
            equipment.OnArmorSlotChanged -= OnArmorSlotChanged;
        }
    }

    void OnArmorSlotChanged(int changedSlot)
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

        if (equipment.armorSOs[slotNumber] != null)
        {
            GetComponent<Image>().sprite = equipment.armorSOs[slotNumber].sprite;
            GetComponent<Image>().color = new Color(255, 255, 255, 1);
        }
        if (equipment.armorSOs[slotNumber] == null)
        {
            GetComponent<Image>().sprite = null;
            GetComponent<Image>().color = new Color(255, 255, 255, 0);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        equipmentPanel.fromSlot = slotNumber;
        equipmentPanel.fromPanel = "Armor";
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
                equipment.EquipArmor(inventoryPanel.fromSlot, slotNumber, slotType);
            }

            if (equipmentPanel.fromPanel == "Armor")
            {
                equipment.MoveArmor(equipmentPanel.fromSlot, slotNumber, slotType);
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