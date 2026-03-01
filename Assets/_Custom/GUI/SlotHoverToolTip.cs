using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotHoverToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
/*
    This script is attached to UI elements that have a hover tooltip.
    It shows the tooltip when the pointer enters the element and hides it when the pointer exits.
    */
    
    //panel that is hovered over
    public GameObject hoverBoxPanel;

    //bool for enabling/disabling the hover box
    public bool isHoverBoxEnabled = true;

    // look for the image component on this GO
    private Image image;

    void Start()
    {
        if (hoverBoxPanel != null)
        {
            hoverBoxPanel.SetActive(false);
        }

        // Look for the Image component on this GameObject
        image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverBoxPanel != null && isHoverBoxEnabled && image.sprite != null)
        {
            hoverBoxPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverBoxPanel != null && isHoverBoxEnabled)
        {
            hoverBoxPanel.SetActive(false);
        }
    }
}
