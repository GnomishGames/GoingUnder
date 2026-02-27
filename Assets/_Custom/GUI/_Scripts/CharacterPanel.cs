using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour
{
    public Image backgroundHighlight;
    
    public void SetSelected(bool selected)
    {
        if (backgroundHighlight != null)
        {
            // Set the appropriate sprite
            if (selected)
            {
                backgroundHighlight.gameObject.SetActive(true);
            }
            else
            {
                backgroundHighlight.gameObject.SetActive(false);
            }
        }
    }

}
