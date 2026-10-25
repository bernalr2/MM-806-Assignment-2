
using UnityEngine;
using UnityEngine.UI;

public class CardboardVRSettings : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider fovSlider;
    [SerializeField] private Slider eyeSeparationSlider;

    [Header("Camera")]
    [SerializeField] private Camera vrCamera;

    [Header("Settings")]
    [SerializeField] private float defaultFOV = 80f;
    [SerializeField] private float defaultEyeSeparation = 0.064f;

    public float CurrentFOV { get; private set; }
    public float CurrentEyeSeparation { get; private set; }

    private void Start()
    {
        if (vrCamera == null)
            vrCamera = Camera.main;

        fovSlider.minValue = 40f;
        fovSlider.maxValue = 120f;
        fovSlider.value = defaultFOV;

        eyeSeparationSlider.minValue = 0f;
        eyeSeparationSlider.maxValue = 0.12f;
        eyeSeparationSlider.value = defaultEyeSeparation;

        fovSlider.onValueChanged.AddListener(UpdateFOV);
        eyeSeparationSlider.onValueChanged.AddListener(UpdateEyeSeparation);

        UpdateFOV(fovSlider.value);
        UpdateEyeSeparation(eyeSeparationSlider.value);
    }

    private void UpdateFOV(float value)
    {
        CurrentFOV = value;

        if (vrCamera != null)
            vrCamera.fieldOfView = value;
    }

    private void UpdateEyeSeparation(float value)
    {
        CurrentEyeSeparation = value;

        // Stores the requested separation.
        // Stock Cardboard XR does not use this value
        // to change the actual left/right eye poses.
    }

    private void OnDestroy()
    {
        fovSlider.onValueChanged.RemoveListener(UpdateFOV);
        eyeSeparationSlider.onValueChanged.RemoveListener(UpdateEyeSeparation);
    }
}

