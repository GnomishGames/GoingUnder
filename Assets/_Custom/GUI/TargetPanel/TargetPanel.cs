using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TargetPanel : MonoBehaviour
{
    public TextMeshProUGUI targetNameText;
    public Slider hpSlider;
    public Slider staminaSlider;
    public Image icon;
    //public Slider manaSlider;
    private CreatureStats currentTargetStats;

    void OnEnable()
    {
        SetNewTarget(currentTargetStats);
    }

    public void SetNewTarget(CreatureStats targetStats)
    {
        // Unsubscribe from old target
        if (currentTargetStats != null)
        {
            currentTargetStats.Hitpoints.OnCurrentChanged -= (value) => SetSliderValue(value, hpSlider);
            currentTargetStats.Hitpoints.OnMaxChanged -= (value) => SetSliderMax(value, hpSlider);
            currentTargetStats.Stamina.OnCurrentChanged -= (value) => SetSliderValue(value, staminaSlider);
            currentTargetStats.Stamina.OnMaxChanged -= (value) => SetSliderMax(value, staminaSlider);
            //currentTargetStats.Mana.OnCurrentChanged -= (value) => SetSliderValue(value, manaSlider);
            //currentTargetStats.Mana.OnMaxChanged -= (value) => SetSliderMax(value, manaSlider);
            currentTargetStats.OnNameChanged -= SetTextValue;
        }

        currentTargetStats = targetStats;

        if (currentTargetStats != null)
        {
            // Subscribe to new target's events
            currentTargetStats.Hitpoints.OnCurrentChanged += (value) => SetSliderValue(value, hpSlider);
            currentTargetStats.Hitpoints.OnMaxChanged += (value) => SetSliderMax(value, hpSlider);
            currentTargetStats.Stamina.OnCurrentChanged += (value) => SetSliderValue(value, staminaSlider);
            currentTargetStats.Stamina.OnMaxChanged += (value) => SetSliderMax(value, staminaSlider);
            //currentTargetStats.Mana.OnCurrentChanged += (value) => SetSliderValue(value, manaSlider);
            //currentTargetStats.Mana.OnMaxChanged += (value) => SetSliderMax(value, manaSlider);
            currentTargetStats.OnNameChanged += SetTextValue;

            // Get initial values
            SetSliderValue(currentTargetStats.Hitpoints.Current, hpSlider);
            SetSliderMax(currentTargetStats.Hitpoints.Max, hpSlider);
            SetSliderValue(currentTargetStats.Stamina.Current, staminaSlider);
            SetSliderMax(currentTargetStats.Stamina.Max, staminaSlider);
            //SetSliderValue(currentTargetStats.Mana.Current, manaSlider);
            //SetSliderMax(currentTargetStats.Mana.Max, manaSlider);
            SetTextValue(currentTargetStats.interactableName);
            UpdatePortrait(currentTargetStats);
        }
    }

    void OnDisable()
    {
        // Unsubscribe when panel closes
        if (currentTargetStats != null)
        {
            currentTargetStats.Hitpoints.OnCurrentChanged -= (value) => SetSliderValue(value, hpSlider);
            currentTargetStats.Hitpoints.OnMaxChanged -= (value) => SetSliderMax(value, hpSlider);
            currentTargetStats.Stamina.OnCurrentChanged -= (value) => SetSliderValue(value, staminaSlider);
            currentTargetStats.Stamina.OnMaxChanged -= (value) => SetSliderMax(value, staminaSlider);
            //currentTargetStats.Mana.OnCurrentChanged -= (value) => SetSliderValue(value, manaSlider);
            //currentTargetStats.Mana.OnMaxChanged -= (value) => SetSliderMax(value, manaSlider);
            currentTargetStats.OnNameChanged -= SetTextValue;
        }
    }

    void SetTextValue(string value)
    {
        targetNameText.text = value;
    }

    void SetSliderValue(float value, Slider slider)
    {
        slider.value = value;
    }

    void SetSliderMax(float maxValue, Slider slider)
    {
        slider.maxValue = maxValue;
    }

    void UpdatePortrait(CreatureStats creatureStats)
    {
        if (icon != null && creatureStats != null && creatureStats.icon != null)
        {
            icon.sprite = creatureStats.icon;
        }
        else
        {
            Debug.LogWarning($"[TargetPanel] Cannot update portrait - icon: {icon != null}, creatureStats: {creatureStats != null}, creatureStats.icon: {(creatureStats != null ? (creatureStats.icon != null).ToString() : "N/A")}");
        }
    }

}