using UnityEngine;
using TMPro;

/*
Put this script on the name panel of the character info panel. It is used to set the name of the character in the panel.
*/

public class NamePanel : MonoBehaviour
{
    CreatureStats creatureStats;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI raceText;
    [SerializeField] private TextMeshProUGUI classText;

    void Start()
    {
        creatureStats = GetComponentInParent<CreatureStats>();

        if (creatureStats != null)
        {
            nameText.text = creatureStats.interactableName;
            raceText.text = creatureStats.characterRace.raceName;
            classText.text = creatureStats.characterClass.className;
        }

        //subscribe to name change event
        creatureStats.OnNameChanged += UpdateName;
        //subscribe to race change event
        creatureStats.OnRaceChanged += UpdateRace;
        //subscribe to class change event
        creatureStats.OnClassChanged += UpdateClass;
    }

    void OnDestroy()
    {
        if (creatureStats != null)
        {
            creatureStats.OnNameChanged -= UpdateName;
            creatureStats.OnRaceChanged -= UpdateRace;
            creatureStats.OnClassChanged -= UpdateClass;
        }
    }

    private void UpdateName(string newName)
    {
        if (nameText != null)
            nameText.text = newName;
    }

    private void UpdateRace(RaceSO newRace)
    {
        // Update race-related UI elements here
        if (raceText != null)
            raceText.text = newRace.raceName;
    }

    private void UpdateClass(ClassSO newClass)
    {
        // Update class-related UI elements here
        if (classText != null)
            classText.text = newClass.className;
    }
}
