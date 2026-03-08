using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[RequireComponent(typeof(Terrain))]
public class TerrainTextureRules : MonoBehaviour
{
    [Header("Height & Slope Thresholds")]
    [Tooltip("Minimum slope angle (degrees) required to show slope texture. Example: 30 means slopes steeper than 30 degrees show slope texture")]
    [Range(0f, 90f)]
    public float slopeAngleMin = 30f;
    
    [Tooltip("Minimum height (world units) required to show peak texture. Example: 80 means anything above 80 units shows peak texture")]
    [Min(0f)]
    public float peakHeightMin = 80f;

    [Header("Grass Texture (Flat & Low)")]
    [Tooltip("Grass texture for gentle slopes at any elevation")]
    public Texture2D grassDiffuse;

    [Tooltip("Optional normal map for grass")]
    public Texture2D grassNormal;

    [Tooltip("Optional mask map for grass")]
    public Texture2D grassMask;

    [Tooltip("UV tile size for grass texture")]
    public Vector2 grassTileSize = new Vector2(15f, 15f);

    [Tooltip("UV tile offset for grass texture")]
    public Vector2 grassTileOffset = Vector2.zero;

    [Header("Slope Texture (Steep)")]
    [Tooltip("Slope texture for steep terrain at any elevation")]
    public Texture2D slopeDiffuse;

    [Tooltip("Optional normal map for slope")]
    public Texture2D slopeNormal;

    [Tooltip("Optional mask map for slope")]
    public Texture2D slopeMask;

    [Tooltip("UV tile size for slope texture")]
    public Vector2 slopeTileSize = new Vector2(15f, 15f);

    [Tooltip("UV tile offset for slope texture")]
    public Vector2 slopeTileOffset = Vector2.zero;

    [Header("Peak Texture (High Elevation)")]
    [Tooltip("Peak texture for high elevations (e.g., snow)")]
    public Texture2D peakDiffuse;

    [Tooltip("Optional normal map for peak")]
    public Texture2D peakNormal;

    [Tooltip("Optional mask map for peak")]
    public Texture2D peakMask;

    [Tooltip("UV tile size for peak texture")]
    public Vector2 peakTileSize = new Vector2(15f, 15f);

    [Tooltip("UV tile offset for peak texture")]
    public Vector2 peakTileOffset = Vector2.zero;

    [Header("Layer Setup")]
    [Tooltip("Automatically adds assigned textures to this terrain if they are missing")]
    public bool autoAddMissingTextures = true;

    private Terrain terrain;
    private TerrainData terrainData;
    private int grassLayerIndex = -1;
    private int slopeLayerIndex = -1;
    private int peakLayerIndex = -1;

    [SerializeField, HideInInspector] private TerrainLayer grassLayer;
    [SerializeField, HideInInspector] private TerrainLayer slopeLayer;
    [SerializeField, HideInInspector] private TerrainLayer peakLayer;

    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
        
        ApplyTextureRules();
    }

    [ContextMenu("Apply Texture Rules")]
    public void ApplyTextureRules()
    {
        if (terrain == null) terrain = GetComponent<Terrain>();
        if (terrainData == null) terrainData = terrain.terrainData;

        EnsureTerrainHasAssignedLayers();
        FindLayerIndices();

        if (!HasAnyMappedTexture())
        {
            Debug.LogWarning("No textures assigned. Assign at least one of: Grass, Slope, or Peak diffuse texture.");
            return;
        }

        int alphamapWidth = terrainData.alphamapWidth;
        int alphamapHeight = terrainData.alphamapHeight;
        int numLayers = terrainData.alphamapLayers;

        float[,,] splatmapData = new float[alphamapWidth, alphamapHeight, numLayers];

        for (int y = 0; y < alphamapHeight; y++)
        {
            for (int x = 0; x < alphamapWidth; x++)
            {
                float normX = (float)x / (alphamapWidth - 1);
                float normY = (float)y / (alphamapHeight - 1);
                
                int heightX = Mathf.RoundToInt(normX * (terrainData.heightmapResolution - 1));
                int heightY = Mathf.RoundToInt(normY * (terrainData.heightmapResolution - 1));
                
                float height = terrainData.GetHeight(heightY, heightX);
                float slope = terrainData.GetSteepness(normY, normX);

                float[] weights = CalculateTextureWeights(height, slope);

                for (int i = 0; i < numLayers && i < weights.Length; i++)
                {
                    splatmapData[x, y, i] = weights[i];
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    private void FindLayerIndices()
    {
        TerrainLayer[] terrainLayers = terrainData.terrainLayers;

        grassLayerIndex = System.Array.IndexOf(terrainLayers, grassLayer);
        slopeLayerIndex = System.Array.IndexOf(terrainLayers, slopeLayer);
        peakLayerIndex = System.Array.IndexOf(terrainLayers, peakLayer);

        if (grassDiffuse != null && grassLayerIndex == -1)
            Debug.LogWarning($"Grass texture '{grassDiffuse.name}' not found in terrain layers.");
        if (slopeDiffuse != null && slopeLayerIndex == -1)
            Debug.LogWarning($"Slope texture '{slopeDiffuse.name}' not found in terrain layers.");
        if (peakDiffuse != null && peakLayerIndex == -1)
            Debug.LogWarning($"Peak texture '{peakDiffuse.name}' not found in terrain layers.");
    }

    private void EnsureTerrainHasAssignedLayers()
    {
        EnsureLayerForTextures(ref grassLayer, grassDiffuse, grassNormal, grassMask, grassTileSize, grassTileOffset, "Grass");
        EnsureLayerForTextures(ref slopeLayer, slopeDiffuse, slopeNormal, slopeMask, slopeTileSize, slopeTileOffset, "Slope");
        EnsureLayerForTextures(ref peakLayer, peakDiffuse, peakNormal, peakMask, peakTileSize, peakTileOffset, "Peak");

        if (!autoAddMissingTextures)
        {
            return;
        }

        TerrainLayer[] existingLayers = terrainData.terrainLayers;
        List<TerrainLayer> updatedLayers = new List<TerrainLayer>(existingLayers);
        bool changed = false;

        AddLayerIfMissing(grassLayer, updatedLayers, ref changed);
        AddLayerIfMissing(slopeLayer, updatedLayers, ref changed);
        AddLayerIfMissing(peakLayer, updatedLayers, ref changed);

        if (!changed)
        {
            return;
        }

        terrainData.terrainLayers = updatedLayers.ToArray();
    }

    private void EnsureLayerForTextures(ref TerrainLayer layer, Texture2D diffuseTexture, Texture2D normalTexture, Texture2D maskTexture, Vector2 tileSize, Vector2 tileOffset, string roleName)
    {
        if (diffuseTexture == null)
        {
            if (normalTexture != null || maskTexture != null)
            {
                Debug.LogWarning($"{roleName} has normal/mask textures assigned, but no diffuse texture. Add a diffuse texture to enable this layer.");
            }

            layer = null;
            return;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            layer = GetOrCreateLayerAsset(layer, diffuseTexture, normalTexture, maskTexture, tileSize, tileOffset, roleName);
            return;
        }
#endif

        if (layer == null)
        {
            layer = new TerrainLayer();
            layer.name = $"{roleName} Layer";
        }

        layer.diffuseTexture = diffuseTexture;
        layer.normalMapTexture = normalTexture;
        layer.maskMapTexture = maskTexture;
        layer.tileSize = tileSize;
        layer.tileOffset = tileOffset;
    }

#if UNITY_EDITOR
    private TerrainLayer GetOrCreateLayerAsset(TerrainLayer existingLayer, Texture2D diffuseTexture, Texture2D normalTexture, Texture2D maskTexture, Vector2 tileSize, Vector2 tileOffset, string roleName)
    {
        if (existingLayer != null)
        {
            bool hasAnyChange = existingLayer.diffuseTexture != diffuseTexture ||
                                existingLayer.normalMapTexture != normalTexture ||
                                existingLayer.maskMapTexture != maskTexture ||
                                existingLayer.tileSize != tileSize ||
                                existingLayer.tileOffset != tileOffset;

            if (hasAnyChange)
            {
                Undo.RecordObject(existingLayer, "Update Terrain Layer");
                existingLayer.diffuseTexture = diffuseTexture;
                existingLayer.normalMapTexture = normalTexture;
                existingLayer.maskMapTexture = maskTexture;
                existingLayer.tileSize = tileSize;
                existingLayer.tileOffset = tileOffset;
                EditorUtility.SetDirty(existingLayer);
            }

            return existingLayer;
        }

        const string folderPath = "Assets/_Custom/Terrains/GeneratedLayers";
        EnsureFolderExists(folderPath);

        string terrainName = terrainData != null ? terrainData.name : "Terrain";
        string fileName = $"{SanitizeFileName(terrainName)}_{roleName}_{SanitizeFileName(diffuseTexture.name)}.terrainlayer";
        string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{fileName}");

        TerrainLayer createdLayer = new TerrainLayer();
        createdLayer.name = $"{terrainName} {roleName}";
        createdLayer.diffuseTexture = diffuseTexture;
        createdLayer.normalMapTexture = normalTexture;
        createdLayer.maskMapTexture = maskTexture;
        createdLayer.tileSize = tileSize;
        createdLayer.tileOffset = tileOffset;

        AssetDatabase.CreateAsset(createdLayer, assetPath);
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(createdLayer);

        return createdLayer;
    }

    private static void EnsureFolderExists(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            return;
        }

        string[] segments = folderPath.Split('/');
        string currentPath = segments[0];

        for (int i = 1; i < segments.Length; i++)
        {
            string nextPath = $"{currentPath}/{segments[i]}";
            if (!AssetDatabase.IsValidFolder(nextPath))
            {
                AssetDatabase.CreateFolder(currentPath, segments[i]);
            }

            currentPath = nextPath;
        }
    }

    private static string SanitizeFileName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Unnamed";
        }

        string sanitized = value;
        sanitized = sanitized.Replace('/', '_');
        sanitized = sanitized.Replace('\\', '_');
        sanitized = sanitized.Replace(':', '_');
        sanitized = sanitized.Replace('*', '_');
        sanitized = sanitized.Replace('?', '_');
        sanitized = sanitized.Replace('"', '_');
        sanitized = sanitized.Replace('<', '_');
        sanitized = sanitized.Replace('>', '_');
        sanitized = sanitized.Replace('|', '_');
        return sanitized.Trim();
    }
#endif

    private static void AddLayerIfMissing(TerrainLayer layer, List<TerrainLayer> layers, ref bool changed)
    {
        if (layer == null || layers.Contains(layer))
        {
            return;
        }

        layers.Add(layer);
        changed = true;
    }

    private bool HasAnyMappedTexture()
    {
        return grassLayerIndex >= 0 || slopeLayerIndex >= 0 || peakLayerIndex >= 0;
    }

    private float[] CalculateTextureWeights(float height, float slope)
    {
        float[] weights = new float[terrainData.alphamapLayers];

        for (int i = 0; i < weights.Length; i++)
            weights[i] = 0f;

        // Rule 1: Steep slopes override everything
        if (slope >= slopeAngleMin && slopeLayerIndex >= 0)
        {
            weights[slopeLayerIndex] = 1f;
        }
        // Rule 2: High elevation gets peak texture
        else if (height >= peakHeightMin && peakLayerIndex >= 0)
        {
            weights[peakLayerIndex] = 1f;
        }
        // Rule 3: Everything else is grass
        else if (grassLayerIndex >= 0)
        {
            weights[grassLayerIndex] = 1f;
        }

        // Normalize weights to sum to 1
        float totalWeight = 0f;
        for (int i = 0; i < weights.Length; i++)
            totalWeight += weights[i];
        
        if (totalWeight > 0f)
        {
            for (int i = 0; i < weights.Length; i++)
                weights[i] /= totalWeight;
        }

        return weights;
    }
}
