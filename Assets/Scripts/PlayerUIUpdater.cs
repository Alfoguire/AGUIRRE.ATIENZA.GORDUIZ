using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIUpdater : MonoBehaviour
{
    [Header("Script References")]
    public SimpleFPC playerController; 

    [Header("Stamina Bar")]
    public Slider staminaSlider; 

    [Header("Noise Meter")]
    public Slider noiseSlider; 
    public float maxNoise = 20f; 

    [Header("Microphone Icon")]
    public Image micIcon;
    public Color micOffColor = new Color(1, 1, 1, 0.5f);
    public Color micOnColor = Color.red;

    void Start()
    {
        if (playerController == null)
        {
            Debug.LogError("PlayerUIUpdater: SimpleFPC script not assigned!");
            return;
        }

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = playerController.maxStamina;
            staminaSlider.value = playerController.maxStamina; // Start full
        }
        
        if (noiseSlider != null)
        {
            noiseSlider.maxValue = maxNoise;
            noiseSlider.value = 0f;
        }

        if (micIcon != null)
        {
            micIcon.color = micOffColor;
        }
    }

    void Update()
    {
        if (playerController == null) return;

        if (staminaSlider != null)
        {
            staminaSlider.value = playerController.currentStamina;
        }

        if (noiseSlider != null)
        {
            noiseSlider.value = Mathf.Lerp(noiseSlider.value, playerController.currentNoiseLevel, Time.deltaTime * 10f);
        }

        if (micIcon != null)
        {
            if (playerController.currentMicrophoneNoise > 0)
            {
                micIcon.color = micOnColor;
            }
            else
            {
                micIcon.color = micOffColor;
            }
        }
    }
}