using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsPanel : MonoBehaviour
{
    Camera cam;

    //events
    public event Action<float> OnDetailDistanceChanged;
    public event Action<float> OnDetailDensityChanged;
    public event Action<int> OnTextureQualityChanged;
    public event Action<int> OnShadowQualityChanged;
    public event Action<int> OnAntialiasingChanged;
    public event Action<bool> OnAnisotropicFilteringChanged;
    public event Action<bool> OnVsyncChanged;
    public event Action<float> OnFieldOfViewChanged;
    public event Action<float> OnClippingPlaneChanged;

    //clipping plane
    [Header("Clipping Plane")]
    public Slider clippingSlider;
    public TextMeshProUGUI clippingValueText;

    //field of view
    [Header("Field of View")]
    public Slider fovSlider;
    public TextMeshProUGUI fovValueText;

    //vsync
    [Header("VSync")]
    public Toggle vSyncToggle;

    //antialiasing
    [Header("Antialiasing")]
    public TMP_Dropdown antiAliasingDropdown;

    //anisotropic filtering
    [Header("Anisotropic Filtering")]
    public Toggle anisotropicFilteringToggle;

    //shadow quality
    [Header("Shadow Quality")]
    public TMP_Dropdown shadowQualityDropdown;

    //texture quality
    [Header("Texture Quality")]
    public TMP_Dropdown textureQualityDropdown;

    //fps counter
    [Header("FPS Counter")]
    public Toggle fpsToggle;
    public FPSCounter fpsCounter;

    //terrain
    [Header("Terrain Settings")]
    public Terrain targetTerrain;

    //terrain detail distance
    [Header("Terrain Detail Distance")]
    public Slider detailDistanceSlider;
    public TMP_Text detailDistanceValueText;

    //terrain detail density
    [Header("Terrain Detail Density")]
    public Slider detailDensitySlider;
    public TMP_Text detailDensityValueText;


    void Start()
    {
        // Find camera by tag
        GameObject eyeCameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (eyeCameraObj != null)
        {
            cam = eyeCameraObj.GetComponent<Camera>();
        }
        else
        {
            Debug.LogError("[GraphicsPanel] No GameObject found with tag 'MainCamera'.");
        }

        //fps counter
        fpsCounter = FindFirstObjectByType<FPSCounter>();
        if (fpsToggle != null && fpsCounter != null)
        {
            fpsToggle.isOn = true;
            fpsToggle.onValueChanged.AddListener(OnFPSToggleChanged);
        }

        // Get the terrain component if not already assigned
        if (targetTerrain == null)
        {
            targetTerrain = Terrain.activeTerrain;
        }

        detailDistanceSlider.onValueChanged.AddListener(OnDetailDistanceSliderChanged);
        detailDensitySlider.onValueChanged.AddListener(OnDetailDensitySliderChanged);
        textureQualityDropdown.onValueChanged.AddListener(OnTextureQualityDropdownChanged);
        shadowQualityDropdown.onValueChanged.AddListener(OnShadowQualityDropdownChanged);
        antiAliasingDropdown.onValueChanged.AddListener(OnAntiAliasingDropdownChanged);
        anisotropicFilteringToggle.onValueChanged.AddListener(OnAnisotropicFilteringToggleChanged);
        vSyncToggle.onValueChanged.AddListener(OnVsyncToggleChanged);
        fovSlider.onValueChanged.AddListener(OnFieldOfViewSliderChanged);
        clippingSlider.onValueChanged.AddListener(OnClippingPlaneSliderChanged);

        // low settings for first time
        targetTerrain.detailObjectDistance = 50f;
        targetTerrain.detailObjectDensity = 0.5f;
        cam.farClipPlane = 200f;
        cam.fieldOfView = 60f;


        //update the slider values to match current settings
        if (cam != null)
        {
            fovSlider.value = cam.fieldOfView;
            fovValueText.text = cam.fieldOfView.ToString();
            clippingSlider.value = cam.farClipPlane;
            clippingValueText.text = cam.farClipPlane.ToString();
        }

        if (targetTerrain != null)
        {
            detailDensitySlider.value = targetTerrain.detailObjectDensity;
            detailDensityValueText.text = targetTerrain.detailObjectDensity.ToString();
            detailDistanceSlider.value = targetTerrain.detailObjectDistance;
            detailDistanceValueText.text = targetTerrain.detailObjectDistance.ToString();
        }

    }

    void OnFPSToggleChanged(bool isOn)
    {
        if (fpsCounter != null)
        {
            fpsCounter.SetFPSVisibility(isOn);
        }
    }

    //terrain detail distance
    private void OnDetailDistanceSliderChanged(float value)
    {
        OnDetailDistanceChanged?.Invoke(value);
        detailDistanceValueText.text = value.ToString();
        if (targetTerrain != null)
        {
            targetTerrain.detailObjectDistance = value; // Update the terrain's detail distance immediately
        }
    }

    //terrain detail density
    private void OnDetailDensitySliderChanged(float value)
    {
        OnDetailDensityChanged?.Invoke(value);
        detailDensityValueText.text = value.ToString();
        if (targetTerrain != null)
        {
            targetTerrain.detailObjectDensity = value; // Update the terrain's detail density immediately
        }
    }

    //texture quality
    private void OnTextureQualityDropdownChanged(int value)
    {
        OnTextureQualityChanged?.Invoke(value);

    }

    //shadow quality
    private void OnShadowQualityDropdownChanged(int value)
    {
        OnShadowQualityChanged?.Invoke(value);
    }

    //antialiasing
    private void OnAntiAliasingDropdownChanged(int value)
    {
        OnAntialiasingChanged?.Invoke(value);
    }

    //anisotropic filtering
    private void OnAnisotropicFilteringToggleChanged(bool isOn)
    {
        OnAnisotropicFilteringChanged?.Invoke(isOn);
    }

    //vsync
    private void OnVsyncToggleChanged(bool isOn)
    {
        OnVsyncChanged?.Invoke(isOn);
    }

    //field of view
    private void OnFieldOfViewSliderChanged(float value)
    {
        OnFieldOfViewChanged?.Invoke(value);
        UpdateFOVText(value.ToString());
        if (cam != null)
        {
            cam.fieldOfView = value; // Update the camera's field of view immediately
        }
    }

    public void UpdateFOVText(string value)
    {
        if (fovValueText != null)
            fovValueText.text = value;
    }

    //clipping plane
    private void OnClippingPlaneSliderChanged(float value)
    {
        cam.farClipPlane = value; // Update the camera's far clipping plane immediately
        clippingValueText.text = value.ToString(); // Update the UI text to reflect the new value
        OnClippingPlaneChanged?.Invoke(value); // Notify listeners of the change
    }
}