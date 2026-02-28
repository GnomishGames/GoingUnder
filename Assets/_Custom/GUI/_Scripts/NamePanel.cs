using UnityEngine;
using TMPro;

/*
Put this script on the name panel of the character info panel. It is used to set the name of the character in the panel.
*/

public class NamePanel : MonoBehaviour
{
    TextMeshProUGUI nameText;
    CreatureStats creatureStats;

    void Start()
    {
        nameText = GetComponentInChildren<TextMeshProUGUI>();
        creatureStats = GetComponentInParent<CreatureStats>();

        if (creatureStats != null)
            nameText.text = creatureStats.interactableName;
        else
            Debug.LogError("NamePanel: No CreatureStats found in parent! Cannot set name text.");
    }

}
