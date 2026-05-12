// Script by Lance (Ontiablo) Nicholls
using TMPro;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    #region Variables
    enum LightFlickerType { simple, noise }

    // The UI section is used PURELY for demonstration purposes in the video. If you wish to remove it, go for it.
    [Header("UI")]
    [SerializeField] TextMeshProUGUI flickerTypeText;
    [SerializeField] TextMeshProUGUI lightOnOffText;
    [SerializeField] TextMeshProUGUI onDurationText;
    [SerializeField] TextMeshProUGUI offDurationText;
    [SerializeField] TextMeshProUGUI lightIntensityText;

    [Header("Light References")]
    [SerializeField] Light pointLight;
    [SerializeField] Light spotLight;

    [Header("Exposed Light Variables")]
    [Tooltip("The light's intensity will take on the value set by this variable")]
    [SerializeField] float maxIntensity = 2f;
    [Tooltip("Max On Time is the maximum duration that the light can be 'On' for when in 'Simple' flicker mode.")]
    [SerializeField] float maxOnTime = 5f;
    [Tooltip("Max Off Time is the maximum duration that the light can be 'Off' for when in 'Simple' flicker mode.")]
    [SerializeField] float maxOffTime = 0.5f;
    [Tooltip("Min On Time is the minimum duration that the light can be 'On' for when in 'Simple' flicker mode.")]
    [SerializeField] float minOnTime = 1f;
    [Tooltip("Min Off Time is the minimum duration that the light can be 'off' for when in 'Simple' flicker mode.")]
    [SerializeField] float minOffTime = 0.1f;

    [Header("Noise Flicker References")]
    [Tooltip("This is where you will select the Flicker Mode. Simple is just turning off and on with the time constraints. Noise uses Perlin noise to flicker the light.")]
    [SerializeField] LightFlickerType lightFlickerType;
    [Tooltip("Frequency determines how fast the light will flicker.")]
    [SerializeField, Range(0f, 10f)] float frequency = 1f;
    [Tooltip("Bottom Cutoff determines at what % the light will go from dim to off instantly. Maximum 49%, Default 25%.")]
    [SerializeField, Range(0, 0.49f)] float bottomCutoff = 0.25f;
    [Tooltip("Top Cutoff determines at what % the light will go from dim to maximum intensity instantly. Minimum 50%, Default 75%.")]
    [SerializeField, Range(0.5f, 1)] float topCutoff = 0.75f;

    float intensitySeed;
    float randomOnTime = 5f;    // a float value representing how long the light must stay on for, set to a random value every time the light needs to turn on.
    float randomOffTime = 0.1f; // a float value representing how long the light must stay off for, set to a random value every time the light needs to turn off.
    float currentTime = 0;      // a float value that we increment every frame to count the time that has passed from when the light either turns on or off to measure against the randomOnTime or randomOffTime.
    bool isLightOn = true;      // a bool used for checking whether the light is on or off to then do the corresponding action.
    #endregion

    #region Initialisation
    void Start()
    {
        if (pointLight != null) pointLight.intensity = maxIntensity;
        else Log("assign point light in inspector!");

        if (spotLight != null) spotLight.intensity = maxIntensity;
        else Log("assign spot light in inspector!");

        intensitySeed = Random.Range(0f, 1000f); // Randomising where the seed starts on the Perlin noise to ensure that if you're using this script on multiple lights, you don't have identical flicker patterns.
        randomOffTime = maxOffTime;
        randomOnTime = maxOnTime;
    }
    #endregion

    void Update()
    {
        currentTime += Time.deltaTime; // incrementing currentTime with the time between each frame. Same as: currentTime = currentTime + Time.deltaTime;

        if (lightFlickerType == LightFlickerType.simple) SimpleFlicker(); // simply checking which Flicker mode has been selected in the inspector so that the code runs the correct one.
        else NoiseFlicker();

        ApplyUIUpdates();
    }

    #region Simple Flicker
    void SimpleFlicker()
    {
        if (isLightOn)
        {
            if (currentTime >= randomOnTime) // checking if currentTime is greater than or equal to randomOnTime
            {
                randomOffTime = Random.Range(minOffTime, maxOffTime); // if it is, I'll set the randomOffTime
                ToggleLight();                                        // and then turn off the light
            }
        }
        else
        {
            if (currentTime >= randomOffTime) // checking if currentTime is greater than or equal to randomOffTime
            {
                randomOnTime = Random.Range(minOnTime, maxOnTime);  // if it is, I'll set the randomOnTime
                ToggleLight();                                      // and then turn on the light.
            }
        }
    }

    void ToggleLight() // Using this method as a toggle to prevent duplicate code written in the SimpleFlicker method.
    {
        currentTime = 0;        // resetting currentTime to 0 so that we can see how long the light spends in the new state.

        isLightOn = !isLightOn; // Simplified from 
                                // if (isLightOn) isLightOn = false ; 
                                // else isLightOn = true;

        pointLight.intensity = isLightOn ? maxIntensity : 0;    // Simplified from
        spotLight.intensity = isLightOn ? maxIntensity : 0;     // if (isLightOn) pointLight.intensity = maxIntensity;
                                                                // else pointLight.intensity = 0;
    }
    #endregion

    #region Noise Flicker
    void NoiseFlicker()
    {
        float _intensityNoise = Mathf.PerlinNoise(intensitySeed, Time.time * frequency); // here I get intensityNoise as a value determined by the seed, time and the frequency that gets a value from the noise image.
                                                                                         // it effectively scrolls through the noise.
        float _intensity = maxIntensity * _intensityNoise;

        if (_intensity > maxIntensity * topCutoff) _intensity = maxIntensity; // Here I handle the cutoff for when the light is bright enough, I simply just make it jump to max intensity for a bit of solid time.
        else if (_intensity < maxIntensity * bottomCutoff) _intensity = 0;    // Here I handle the cutoff for when the light is dim enough, I simply just make it jump to 0 for a bit of solid off time.

        pointLight.intensity = _intensity; // Here I set the intensity of the light to the value I got from the noise.
        spotLight.intensity = _intensity;
    }
    #endregion

    #region UI Section
    // If you do end up using this script in your project, I recommend removing the UI code below. It was purely used for demonstration purposes in the video.
    void ApplyUIUpdates()
    {
        if (flickerTypeText == null || lightOnOffText == null || offDurationText == null || onDurationText == null || lightIntensityText == null)
        {
            Log("Remember to assign ALL text fields in the inspector");
            return;
        }

        if (lightFlickerType == LightFlickerType.simple) flickerTypeText.text = "Flicker Type: Simple";
        else flickerTypeText.text = "Flicker Type: Noise";

        if (isLightOn)
        {
            lightOnOffText.text = "Light: On";
            onDurationText.text = "On Duration: " + (Mathf.Round(currentTime * 100) / 100).ToString();
            offDurationText.text = "Off Duration: 0";
        }
        else
        {
            lightOnOffText.text = "Light: Off";
            onDurationText.text = "On Duration: 0";
            offDurationText.text = "Off Duration: " + (Mathf.Round(currentTime * 100) / 100).ToString();
        }

        lightIntensityText.text = "Light Intensity: " + (Mathf.Round(pointLight.intensity * 100) / 100).ToString();
    }
    #endregion

    #region Debugging
    void Log(string message) => Debug.Log($"[LightFlicker]: {message}"); // I like using a method specifically for debug logging so that I can have consistent messaging. Something I picked up recently.

    #endregion
}


// Script by Lance (Ontiablo) Nicholls