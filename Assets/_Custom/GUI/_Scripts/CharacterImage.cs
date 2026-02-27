using UnityEngine;
using UnityEngine.UI;

public class CharacterImage : MonoBehaviour
{
    public Image iconImageComponent; //Assign in inspector only
    private CreatureStats creatureStats;
    private CharacterPanel characterPanel;
    private Transform player;

    void Start()
    {
        //find character panel in parent hierarchy because it holds the playerTag
        characterPanel = GetComponentInParent<CharacterPanel>();


        //get characterstats
        creatureStats = GetComponent<CreatureStats>();

        // Set portrait
        if (iconImageComponent != null && creatureStats.icon != null)
        {
            iconImageComponent.sprite = creatureStats.icon;
        }
        else
        {
            Debug.LogWarning("CharacterImage: Missing iconImageComponent or creatureStats.icon for " + gameObject.name);
        }
    }
}
