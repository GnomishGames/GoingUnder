using TMPro;
using UnityEngine;

public class AttackDie : MonoBehaviour
{
    // display the results of die roll on the die face
    TextMeshProUGUI dieText;

    SkillBar skillBar;

    private void Awake()
    {
        dieText = GetComponentInChildren<TextMeshProUGUI>();
        skillBar = FindFirstObjectByType<SkillBar>();
    }

    void OnEnable()
    {
        //subscribe to the event that triggers when dice are rolled for a skill
        skillBar.OnAttackRoll += SetDieValue;
    }

    void OnDisable()
    {
        //unsubscribe from the event when the die is disabled
        skillBar.OnAttackRoll -= SetDieValue;
    }

    public void SetDieValue(int value)
    {
        dieText.text = value.ToString();
    }
}
