using Sirenix.OdinInspector;
using UnityEngine;

public class RippleController : MonoBehaviour
{
    [Header("Resources")]
    [SerializeField] private Material m_RippleMaterial;
    [SerializeField] private AnimationCurve m_Waveform;

    [Header("Settings")]
    [Range(0.01f, 1.0f)][SerializeField] private float m_RefractionStrength = 0.5f;
    [SerializeField] private Color m_ReflectionColor = Color.white;
    [Range(0.01f, 1.0f)][SerializeField] private float m_ReflectionStrength = 0.7f;
    [Range(0.1f, 5.0f)][SerializeField] private float m_WaveSpeed = 1.25f;

    private class Droplet
    {
        public Vector2 Pos;
        public float Time = 10f;

        public void Reset(Vector2 position)
        {
            Pos = position;
            Time = 0;
        }

        public void UpdateTime() => Time += UnityEngine.Time.deltaTime;
    }

    private Droplet[] m_droplets;
    private Texture2D m_gradTexture;
    private int m_dropCount;

    private void OnEnable()
    {
        m_droplets = new[] { new Droplet(), new Droplet(), new Droplet() };
        GenerateGradient();
    }

    private void Update()
    {
        if (m_RippleMaterial == null) return;

        bool isAnyWaveActive = false;
        foreach (var d in m_droplets)
        {
            if (d.Time < 1.5f)
            {
                d.UpdateTime();
                isAnyWaveActive = true;
            }
        }

        if (isAnyWaveActive || !Application.isPlaying)
        {
            SyncShader();
        }
    }

    public void TriggerRipple()
    {
        if (Camera.main == null) return;

        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
        Vector2 uvPos = new Vector2(screenPos.x, screenPos.y);

        m_droplets[m_dropCount % 3].Reset(uvPos);
        m_dropCount++;
    }

    private void SyncShader()
    {
        float aspect = (float)Screen.width / Screen.height;

        m_RippleMaterial.SetVector("_Params1", new Vector4(aspect, 1, 1 / m_WaveSpeed, 0));
        m_RippleMaterial.SetVector("_Params2", new Vector4(m_RefractionStrength, m_ReflectionStrength, 0, 0));
        m_RippleMaterial.SetColor("_Reflection", m_ReflectionColor);
        m_RippleMaterial.SetTexture("_GradTex", m_gradTexture);

        for (int i = 0; i < 3; i++)
        {
            m_RippleMaterial.SetVector($"_Drop{i + 1}", new Vector4(m_droplets[i].Pos.x, m_droplets[i].Pos.y, m_droplets[i].Time, 0));
        }
    }

    private void GenerateGradient()
    {
        if (m_gradTexture != null) return;

        m_gradTexture = new Texture2D(512, 1, TextureFormat.Alpha8, false);
        m_gradTexture.wrapMode = TextureWrapMode.Clamp;

        for (int i = 0; i < 512; i++)
        {
            float v = m_Waveform.Evaluate((float)i / 512);
            m_gradTexture.SetPixel(i, 0, new Color(0, 0, 0, v));
        }
        m_gradTexture.Apply();
    }
}