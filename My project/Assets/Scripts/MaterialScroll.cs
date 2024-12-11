using UnityEngine;

public class OceanTextureAnimator : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float baseScrollSpeedX = 0.1f;
    public float baseScrollSpeedY = 0.1f;

    [Header("Wave Settings")]
    public float waveAmplitudeX = 0.05f;
    public float waveAmplitudeY = 0.05f;
    public float waveFrequencyX = 1f;
    public float waveFrequencyY = 1f;

    private Renderer rend;
    private Vector2 savedOffset;
    private float timeCounter = 0f;

    void Start()
    {
        rend = GetComponent<Renderer>();
        savedOffset = rend.material.GetTextureOffset("_MainTex");
    }

    void Update()
    {
        // Increment time counter
        timeCounter += Time.deltaTime;

        // Calculate base scroll
        float offsetX = timeCounter * baseScrollSpeedX;
        float offsetY = timeCounter * baseScrollSpeedY;

        // Add wave-like motion using sine waves
        float waveX = Mathf.Sin(timeCounter * waveFrequencyX) * waveAmplitudeX;
        float waveY = Mathf.Cos(timeCounter * waveFrequencyY) * waveAmplitudeY;

        // Combine base scroll with wave motion
        Vector2 offset = new Vector2(offsetX + waveX, offsetY + waveY);
        
        rend.material.SetTextureOffset("_MainTex", offset);
    }

    void OnDisable()
    {
        rend.material.SetTextureOffset("_MainTex", savedOffset);
    }
}