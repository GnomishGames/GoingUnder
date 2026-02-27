using UnityEngine;

public class Item : Interactable
{
    //Items are things that can be picked up and used by the player

    // Item properties
    public float itemWeight;
    public ItemSO item;
    //public int itemHealth;

    public override void Interact(GameObject interactor)
    {
        if (interactor == null) return;

        OnFocused(interactor.transform);

        var inventory = interactor.GetComponent<Inventory>();
        if (inventory != null)
        {
            inventory.PickupItem(this);
        }
    }
}

public enum SlotType { Head, Neck, Back, Arms, Chest, Hands, Legs, Feet, Primary, Secondary, Ranged, Ammo, Finger, Skill, Item };