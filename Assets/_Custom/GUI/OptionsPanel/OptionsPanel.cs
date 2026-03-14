using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsPanel : MonoBehaviour
{
    //events
    public event Action<string> OnAllAudioVolumeChanged;
    public event Action<string> OnMusicVolumeChanged;
    public event Action<string> OnSFXVolumeChanged;

    //audio
    [Header("Audio Settings")]
    public Slider allAudioSlider;
    public TextMeshProUGUI allAudioValueText;
    public Slider musicSlider;
    public TextMeshProUGUI musicValueText;
    public Slider sfxSlider;
    public TextMeshProUGUI sfxValueText;

    //reference to audio stuff
    public AudioMixer audioMixer;

    [Header("Mixer Parameters")]
    [SerializeField] private string masterVolumeParameter = "Master";
    [SerializeField] private string musicVolumeParameter = "Music";
    [SerializeField] private string sfxVolumeParameter = "SFX";
    [SerializeField] private float mixerMinDb = -80f;
    [SerializeField] private float mixerMaxDb = 0f;

    private void OnEnable()
    {
        RegisterSliderListeners();
    }

    private void OnDisable()
    {
        UnregisterSliderListeners();
    }

    void Start()
    {
        if (audioMixer == null)
        {
            return;
        }

        InitializeSliderFromMixer(allAudioSlider, allAudioValueText, masterVolumeParameter);
        InitializeSliderFromMixer(musicSlider, musicValueText, musicVolumeParameter);
        InitializeSliderFromMixer(sfxSlider, sfxValueText, sfxVolumeParameter);
    }

    public void OnAllAudioSliderChanged(float sliderValue)
    {
        if (TryApplySliderToMixer(allAudioSlider, allAudioValueText, masterVolumeParameter, sliderValue, out int percent))
        {
            OnAllAudioVolumeChanged?.Invoke(percent.ToString());
        }
    }

    public void OnMusicSliderChanged(float sliderValue)
    {
        if (TryApplySliderToMixer(musicSlider, musicValueText, musicVolumeParameter, sliderValue, out int percent))
        {
            OnMusicVolumeChanged?.Invoke(percent.ToString());
        }
    }

    public void OnSFXSliderChanged(float sliderValue)
    {
        if (TryApplySliderToMixer(sfxSlider, sfxValueText, sfxVolumeParameter, sliderValue, out int percent))
        {
            OnSFXVolumeChanged?.Invoke(percent.ToString());
        }
    }

    private void RegisterSliderListeners()
    {
        RegisterSliderListener(allAudioSlider, OnAllAudioSliderChanged);
        RegisterSliderListener(musicSlider, OnMusicSliderChanged);
        RegisterSliderListener(sfxSlider, OnSFXSliderChanged);
    }

    private void UnregisterSliderListeners()
    {
        UnregisterSliderListener(allAudioSlider, OnAllAudioSliderChanged);
        UnregisterSliderListener(musicSlider, OnMusicSliderChanged);
        UnregisterSliderListener(sfxSlider, OnSFXSliderChanged);
    }

    private static void RegisterSliderListener(Slider slider, UnityEngine.Events.UnityAction<float> callback)
    {
        if (slider == null)
        {
            return;
        }

        // Remove before add to avoid duplicate callbacks after re-enabling.
        slider.onValueChanged.RemoveListener(callback);
        slider.onValueChanged.AddListener(callback);
    }

    private static void UnregisterSliderListener(Slider slider, UnityEngine.Events.UnityAction<float> callback)
    {
        if (slider == null)
        {
            return;
        }

        slider.onValueChanged.RemoveListener(callback);
    }

    private void InitializeSliderFromMixer(Slider slider, TextMeshProUGUI valueText, string mixerParameter)
    {
        if (slider == null || string.IsNullOrEmpty(mixerParameter))
        {
            return;
        }

        if (audioMixer.GetFloat(mixerParameter, out float currentDb))
        {
            // AudioMixer stores volume in dB, so convert to the slider range.
            float normalized = Mathf.InverseLerp(mixerMinDb, mixerMaxDb, currentDb);
            slider.value = Mathf.Lerp(slider.minValue, slider.maxValue, normalized);
            UpdateValueText(valueText, normalized);
        }
    }

    private bool TryApplySliderToMixer(Slider slider, TextMeshProUGUI valueText, string mixerParameter, float sliderValue, out int percent)
    {
        percent = 0;

        if (audioMixer == null || slider == null || string.IsNullOrEmpty(mixerParameter))
        {
            return false;
        }

        float normalized = Mathf.InverseLerp(slider.minValue, slider.maxValue, sliderValue);
        float dbValue = Mathf.Lerp(mixerMinDb, mixerMaxDb, normalized);
        percent = Mathf.RoundToInt(normalized * 100f);

        audioMixer.SetFloat(mixerParameter, dbValue);
        UpdateValueText(valueText, normalized);
        return true;
    }

    private static void UpdateValueText(TextMeshProUGUI valueText, float normalized)
    {
        if (valueText == null)
        {
            return;
        }

        valueText.text = $"{Mathf.RoundToInt(normalized * 100f)}%";
    }
}
