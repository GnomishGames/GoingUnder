using UnityEngine;

public class TogglePanel : MonoBehaviour
{
    public GameObject[] panelsToToggle;
    public GameObject[] panelsToDisable;

    public void Toggle()
    {
        foreach (GameObject panel in panelsToToggle)
        {
            if (panel != null)
            {
                panel.SetActive(!panel.activeSelf);
            }
        }
        DisableOtherPanels();
    }

    private void DisableOtherPanels()
    {
        foreach (GameObject panel in panelsToDisable)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }
}
