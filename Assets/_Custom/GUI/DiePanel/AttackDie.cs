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
    }

    public void SetDieValue(int value)
    {
        dieText.text = value.ToString();
    }
}
