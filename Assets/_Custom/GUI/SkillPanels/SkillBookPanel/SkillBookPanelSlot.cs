using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillBookPanelSlot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    //player references
    public Transform player;

    //ui elements
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalAnchoredPosition;
    private Transform originalParent;
    private int originalSiblingIndex;
    private Transform dragLayer;

    //panels
    public SkillBookPanel skillBookPanel;

    //array references
    private SkillBook skillBook;

    //slot stuff
    public int slotNumber; //Tells us which skill number this is
    public SlotType slotType;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        player = GameObject.FindWithTag("Player").transform;

        //set ui elements
        rectTransform = GetComponent<RectTransform>();

        //draglayer keeps icons on top when dragging
        dragLayer = GameObject.FindWithTag("DragLayer").transform;

        skillBookPanel = GetComponentInParent<SkillBookPanel>();
        //get skillBook from player (not from UI)
        skillBook = player.GetComponent<SkillBook>();

        // Subscribe to skill change events
        if (skillBook != null)
        {
            skillBook.OnSkillChanged += OnSkillChanged;
        }

        // Initial update
        UpdateSlotIcons();
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (skillBook != null)
        {
            skillBook.OnSkillChanged -= OnSkillChanged;
        }
    }

    void OnSkillChanged(int changedSlot)
    {
        // Only update if this is the slot that changed
        if (changedSlot == slotNumber)
        {
            UpdateSlotIcons();
        }
    }

    private void UpdateSlotIcons()
    {
        if (skillBook.skillSOs[slotNumber] != null)
        {
            GetComponent<Image>().sprite = skillBook.skillSOs[slotNumber].sprite;
            GetComponent<Image>().color = new Color(255, 255, 255, 1);
        }
        if (skillBook.skillSOs[slotNumber] == null)
        {
            GetComponent<Image>().sprite = null;
            GetComponent<Image>().color = new Color(255, 255, 255, 0);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        skillBookPanel.fromSlot = slotNumber;
        skillBookPanel.fromPanel = "SkillBook";
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
            // Try to get the source as a SkillBookPanelSlot
            var sourceSkillBookSlot = eventData.pointerDrag.GetComponent<SkillBookPanelSlot>();
            if (sourceSkillBookSlot != null && sourceSkillBookSlot.skillBookPanel.fromPanel == "SkillBook")
            {
                skillBook.MoveItem(sourceSkillBookSlot.skillBookPanel.fromSlot, slotNumber);
            }
            
            // Try to get the source as a SkillBarPanelSlot
            var sourceSkillBarSlot = eventData.pointerDrag.GetComponent<SkillBarPanelSlot>();
            if (sourceSkillBarSlot != null && sourceSkillBarSlot.skillBarPanel.fromPanel == "SkillBar")
            {
                skillBook.UnEquipSkill(slotNumber, sourceSkillBarSlot.skillBarPanel.fromSlot);
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
    }
}
