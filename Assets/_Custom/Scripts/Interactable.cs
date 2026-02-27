using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string interactableName;
    public Sprite icon;
    bool isFocus = false;

    public void OnFocused(Transform item)
    {
        isFocus = true;
    }

    public virtual void Interact(GameObject interactor)
    {
        // This method is meant to be overridden by derived classes to define specific interaction behavior.
        // The base implementation does nothing, allowing derived classes to implement their own logic.
    }

    public void OnDeFocus()
    {
        //playerTransform = null;
        isFocus = false;
    }
}

