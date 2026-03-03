using TMPro;
using UnityEngine;

public class AttackDie : MonoBehaviour
{
    // display the results of die roll on the die face
    TextMeshProUGUI dieText;

    private void Awake()
    {
        dieText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetDieValue(int value)
    {
        if (value > 0)
        {
            dieText.text = value.ToString();
        }
        else
        {
            dieText.text = "";
        }
    }
}
