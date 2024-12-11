using UnityEngine;

public class OceanTextureRandomizer : MonoBehaviour
{
    [Header("Tiling Randomization")]
    public Vector2 tileRangeX = new Vector2(1f, 3f);
    public Vector2 tileRangeY = new Vector2(1f, 3f);

    [Header("Offset Randomization")]
    public Vector2 offsetRangeX = new Vector2(0f, 1f);
    public Vector2 offsetRangeY = new Vector2(0f, 1f);

    [Header("Additional Variation")]
    public bool randomizeOnStart = true;

    private Renderer rend;
    private Material materialInstance;

    void Start()
    {
        rend = GetComponent<Renderer>();
        
        // Create a material instance to avoid modifying the original
        materialInstance = new Material(rend.sharedMaterial);
        rend.material = materialInstance;

        if (randomizeOnStart)
        {
            RandomizeTiling();
        }
    }

    public void RandomizeTiling()
    {
        // Randomize Tiling
        float randomTileX = Random.Range(tileRangeX.x, tileRangeX.y);
        float randomTileY = Random.Range(tileRangeY.x, tileRangeY.y);
        materialInstance.mainTextureScale = new Vector2(randomTileX, randomTileY);

        // Randomize Offset
        float randomOffsetX = Random.Range(offsetRangeX.x, offsetRangeX.y);
        float randomOffsetY = Random.Range(offsetRangeY.x, offsetRangeY.y);
        materialInstance.mainTextureOffset = new Vector2(randomOffsetX, randomOffsetY);
    }

    // Optional method to manually randomize again
    [ContextMenu("Randomize Texture")]
    public void ForceRandomize()
    {
        RandomizeTiling();
    }
}