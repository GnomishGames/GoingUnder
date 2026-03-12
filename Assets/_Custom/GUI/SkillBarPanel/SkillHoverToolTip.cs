using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillHoverToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /*
    This script is attached to skill slots in the skill bar.
    It shows the tooltip when the pointer enters the element and hides it when the pointer exits.
    */

    //panel that is hovered over
    public GameObject hoverBoxPanel;
    Transform hoverBoxOriginalParent;

    //get players skills to access item data
    SkillBar skillBar;
    SkillBook skillBook;

    //bool for enabling/disabling the hover box
    public bool isHoverBoxEnabled = true;

    // look for the image component on this GO
    Image image;

    //drag layer reference
    Transform dragLayer;

    //player reference
    GameObject player;

    //reference to the panel slots
    SkillBarPanelSlot skillBarPanelSlot;
    SkillBookPanelSlot skillBookPanelSlot;

    void Start()
    {
        // Set the hover box to be inactive at the start
        if (hoverBoxPanel != null)
        {
            hoverBoxPanel.SetActive(false);
            //store the original parent so we can restore it later
            hoverBoxOriginalParent = hoverBoxPanel.transform.parent;
        }

        //draglayer keeps icons on top when dragging
        dragLayer = GameObject.FindWithTag("DragLayer").transform;

        // Get the player        
        player = transform.root.gameObject;

        // get player's skills
        skillBar = player.GetComponent<SkillBar>();
        skillBook = player.GetComponent<SkillBook>();

        // Look for the Image component on this GameObject
        image = GetComponent<Image>();

        //get parent slot reference
        skillBarPanelSlot = GetComponentInParent<SkillBarPanelSlot>();
        skillBookPanelSlot = GetComponentInParent<SkillBookPanelSlot>();

        //check for nulls
        if (skillBarPanelSlot == null && skillBookPanelSlot == null)
        {
            Debug.LogError("SkillHoverToolTip: No parent slot found for " + gameObject.name);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // If the hover box is not enabled, or if there is no item in the slot, do not show the hover box
        if (hoverBoxPanel == null || !isHoverBoxEnabled || image.sprite == null || skillBar == null || skillBook == null)
            return;

        // Determine which slot this is and get the corresponding skill data
        int slotNumber = skillBarPanelSlot != null ? skillBarPanelSlot.slotNumber : skillBookPanelSlot.slotNumber;

        // Get the skill from the appropriate slot
        SkillSO skill = skillBarPanelSlot != null ? skillBar.skillSOs[slotNumber] : skillBook.skillSOs[slotNumber];
        if (skill == null)
            return;

        // Move the hover box to the drag layer so it appears on top of other UI elements
        hoverBoxPanel.transform.SetParent(dragLayer, true);
        hoverBoxPanel.transform.SetAsLastSibling();

        // Set the hover box to be active
        hoverBoxPanel.SetActive(true);
        PopulateHoverBoxWithStats(skill);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverBoxPanel != null)

            // Restore the hoverBoxPanel to its original parent
            if (hoverBoxOriginalParent != null && hoverBoxPanel.transform.parent != hoverBoxOriginalParent)
            {
                // preserve world pos while reparenting
                Vector3 worldPos = hoverBoxPanel.transform.position;
                hoverBoxPanel.transform.SetParent(hoverBoxOriginalParent, false);
                hoverBoxPanel.transform.position = worldPos;
            }
        {
            hoverBoxPanel.SetActive(false);
        }
    }

    void PopulateHoverBoxWithStats(SkillSO skill)
    {
        // This function populates the hover box with the stats of the skill
        if (skill == null)
            return;

        // display the item name
        UpdateHoverBoxText("ItemName", skill.itemName);
        UpdateHoverBoxText("ItemNameSuffix", skill.itemNameSuffix);

        // display the item description
        UpdateHoverBoxText("Description", skill.itemDescription);
    }

    public void UpdateHoverBoxText(string statName, string statValue)
    {
        // Recursively search for the child by name
        Transform statTextTransform = FindDeepChild(hoverBoxPanel.transform, statName);

        if (statTextTransform != null)
        {
            TextMeshProUGUI textMesh = statTextTransform.GetComponent<TextMeshProUGUI>();
            if (textMesh != null)
            {
                textMesh.text = statValue;
                // Enable the parent GameObject when setting text
                if (statTextTransform.parent != null)
                {
                    statTextTransform.parent.gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.LogWarning($"Found {statName} but it has no TextMeshProUGUI component");
            }
        }
        else
        {
            Debug.LogWarning($"Could not find child named '{statName}' in hoverBoxPanel hierarchy");
        }
    }
    
    // Recursively search for a child transform by name
    private Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;

            Transform result = FindDeepChild(child, childName);
            if (result != null)
                return result;
        }
        return null;
    }
}
