using UnityEngine;
using TMPro;

public class TargetName : MonoBehaviour
{
    //get the tmp text component
    private TextMeshProUGUI targetNameText;
    private PlayerTargeting playerTargeting;
    private CreatureStats targetStats;

    void OnEnable()
    {
        targetNameText = GetComponent<TextMeshProUGUI>();
        playerTargeting = transform.root.GetComponentInChildren<PlayerTargeting>();

        UpdateName();
    }

    void UpdateName()
    {
        // get the current target's name and display it
        targetStats = playerTargeting.currentTarget.GetComponent<CreatureStats>();

        if (targetStats != null)
        {
            targetNameText.text = targetStats.interactableName;
        }
        else
        {
            targetNameText.text = "NoName";
        }
    }

}
