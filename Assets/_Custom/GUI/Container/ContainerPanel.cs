using TMPro;
using UnityEngine;

public class ContainerPanel : MonoBehaviour
{
    public int fromSlot; //Tells us where the item came from when we drag/drop
    public string fromPanel; //tells us what panel it came from

    public TextMeshProUGUI targetNameText;
    public Container container;

    void OnEnable()
    {
    }

    public void SetNewTarget(Container newContainer)
    {
        // Unsubscribe from old target
        if (container != null)
        {
            container.OnNameChanged -= SetTextValue;
        }

        container = newContainer;

        if (container != null)
        {
            container.OnNameChanged += SetTextValue;
            // Manually update the text immediately since the event already fired
            SetTextValue(container.interactableName);
        }
    }

    void OnDisable()
    {
        // Unsubscribe when panel closes
        if (container != null)
        {
            container.OnNameChanged -= SetTextValue;
        }
    }

    void SetTextValue(string value)
    {
        targetNameText.text = value;
    }
}
        