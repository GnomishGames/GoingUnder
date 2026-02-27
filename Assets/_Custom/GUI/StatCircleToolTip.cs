using UnityEngine;
using UnityEngine.EventSystems;

public class StatCircleToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //panel that is hovered over
    public GameObject hoverBoxPanel;

    //bool for enabling/disabling the hover box
    public bool isHoverBoxEnabled = true;

    void Start()
    {
        if (hoverBoxPanel != null)
        {
            hoverBoxPanel.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverBoxPanel != null && isHoverBoxEnabled)
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
