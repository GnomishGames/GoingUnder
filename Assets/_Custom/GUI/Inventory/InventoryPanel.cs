using System;
using UnityEngine;

/*
This script goes on the inventory panel, (root element for the inventory UI)
and is used to tell the inventory what slot an item came from when we drag and drop it, 
so that the inventory can update accordingly. It also has events for when the inventory panel is opened and closed, 
so that the UI can update accordingly.
*/

public class InventoryPanel : MonoBehaviour
{
    public int fromSlot; //Tells us where the item came from when we drag/drop
    public string fromPanel; //tells us what panel it came from

    //event when inventory panel is opened
    public event Action OnInventoryPanelOpened;
    
    //event when inventory panel is closed
    public event Action OnInventoryPanelClosed;

    private void Start()
    {
    }

    private void OnEnable()
    {
        OnInventoryPanelOpened?.Invoke();
    }

    private void OnDisable()
    {
        OnInventoryPanelClosed?.Invoke();
    }
}
