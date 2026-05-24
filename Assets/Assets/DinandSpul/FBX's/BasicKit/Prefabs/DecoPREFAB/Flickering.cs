using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlickerHDRP : MonoBehaviour
{
    [Header("Light Flicker")]
    public float minIntensity = 2;
    public float maxIntensity = 100;
    public float flickerSpeed = 0.05f;

    [Header("Emissive Material")]
    public Renderer emissiveRenderer;

    [Tooltip("HDRP usually uses _EmissiveColor")]
    public string emissionProperty = "_EmissiveColor";

    [Tooltip("How bright the emission gets")]
    public float emissionMultiplier = 10f;

    private Light flickerLight;
    private Material emissiveMat;
    private float targetIntensity;

    void Start()
    {
        flickerLight = GetComponent<Light>();

        if (emissiveRenderer != null)
        {
            emissiveMat = emissiveRenderer.material;
        }
    }

    void Update()
    {
        // Generate target flicker
        targetIntensity = Random.Range(minIntensity, maxIntensity);

        // Smooth flicker
        flickerLight.intensity = Mathf.Lerp(
            flickerLight.intensity,
            targetIntensity,
            Time.deltaTime / flickerSpeed
        );

        // Sync emissive material
        if (emissiveMat != null)
        {
            float normalized = flickerLight.intensity / maxIntensity;

            Color emission = Color.white * normalized * emissionMultiplier;

            emissiveMat.SetColor(emissionProperty, emission);
        }
    }
}