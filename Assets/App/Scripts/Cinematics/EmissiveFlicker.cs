using UnityEngine;

public class EmissiveFlicker : MonoBehaviour
{
    [Header("Material Reference")]
    public Material targetMaterial;

    [Header("Color and Intensities")]
    public Color baseEmissiveColor = Color.white;

    [Header("Flicker Ranges")]
    public float flicker1Min = 0.5f;
    public float flicker1Max = 1.5f;
    public float flicker2Min = 4.0f;
    public float flicker2Max = 7.0f;

    [Header("Flicker Settings")]
    public float minFlickerDuration = 0.05f;
    public float maxFlickerDuration = 0.3f;
    public float smoothSpeed = 2f;
    public float minFlickerSpeed = 0.5f;
    public float maxFlickerSpeed = 2f;
    [Range(0f, 1f)]
    public float flickerThreshold = 0.5f;

    private float targetIntensity;
    private float smoothedIntensity;
    private float flickerTimer;
    private float noiseOffset;
    private bool nextShouldBeEmissive;
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    private void Start()
    {
        noiseOffset = Random.Range(0f, 999f);
        if (targetMaterial != null) targetMaterial.EnableKeyword("_EMISSION");
        // commencer aléatoirement sur noir ou emissive
        nextShouldBeEmissive = Random.value > 0.5f;
        GenerateNewFlicker();
        smoothedIntensity = 0f;
    }

    private void Update()
    {
        if (targetMaterial == null) return;

        flickerTimer -= Time.deltaTime;
        if (flickerTimer <= 0f)
        {
            GenerateNewFlicker();
        }

        smoothedIntensity = Mathf.Lerp(smoothedIntensity, targetIntensity, Time.deltaTime * smoothSpeed);
        targetMaterial.SetColor(EmissionColorId, baseEmissiveColor * smoothedIntensity);
    }

    private void GenerateNewFlicker()
    {
        // Durée aléatoire pour ce flicker
        flickerTimer = Random.Range(minFlickerDuration, maxFlickerDuration);

        if (nextShouldBeEmissive)
        {
            // Choisit une intensité emissive aléatoire à partir des deux plages en utilisant Perlin noise
            float currentSpeed = Mathf.Lerp(minFlickerSpeed, maxFlickerSpeed, Random.value);
            float noise = Mathf.PerlinNoise(Time.time * currentSpeed, noiseOffset);

            if (noise > flickerThreshold)
                targetIntensity = Random.Range(flicker1Min, flicker1Max);
            else
                targetIntensity = Random.Range(flicker2Min, flicker2Max);
        }
        else
        {
            // Noir
            targetIntensity = 0f;
        }

        // Décider aléatoirement si le prochain état sera emissive ou noir (donne un flicker irrégulier)
        nextShouldBeEmissive = Random.value > 0.5f;
    }
}
