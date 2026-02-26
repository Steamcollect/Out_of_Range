using TMPro;
using UnityEngine;

public class StatsDebug : MonoBehaviour
{
    [Header("Stats Settings")]
    [Tooltip("Durée de l'historique en secondes")]
    public float HistoryDuration = 5f;

    [Header("UI References")]
    public TMP_Text FpsText;
    public TMP_Text AvgText;
    public TMP_Text MinText;
    public TMP_Text MaxText;

    private float[] m_FrameTimes;
    private int m_FrameCount;
    private int m_FrameIndex;

    void Start()
    {
        int bufferSize = Mathf.CeilToInt(HistoryDuration / Mathf.Max(Time.fixedDeltaTime, 0.001f));
        m_FrameTimes = new float[bufferSize];
        m_FrameCount = 0;
        m_FrameIndex = 0;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        if (m_FrameTimes == null || m_FrameTimes.Length == 0) return;
        if (m_FrameCount == m_FrameTimes.Length)
        {
            // Rien à faire, on écrase simplement l'ancien
        }
        else
        {
            m_FrameCount++;
        }
        m_FrameTimes[m_FrameIndex] = dt;
        m_FrameIndex = (m_FrameIndex + 1) % m_FrameTimes.Length;

        // Calcul des stats
        float min = float.MaxValue;
        float max = float.MinValue;
        float sum = 0f;
        for (int i = 0; i < m_FrameCount; i++)
        {
            float t = m_FrameTimes[i];
            sum += t;
            if (t < min) min = t;
            if (t > max) max = t;
        }
        float avg = sum / m_FrameCount;
        float fps = 1f / avg;
        float minMs = min * 1000f;
        float maxMs = max * 1000f;
        float avgMs = avg * 1000f;
        if (FpsText != null) FpsText.text = $"FPS: {fps:F1}";
        if (AvgText != null) AvgText.text = $"Moyenne: {avgMs:F2} ms";
        if (MinText != null) MinText.text = $"Min: {minMs:F2} ms";
        if (MaxText != null) MaxText.text = $"Max: {maxMs:F2} ms";}
}
