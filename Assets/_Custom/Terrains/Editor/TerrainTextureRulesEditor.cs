using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainTextureRules))]
[CanEditMultipleObjects]
public class TerrainTextureRulesEditor : Editor
{
    private SerializedProperty slopeAngleMin;
    private SerializedProperty peakHeightMin;
    private SerializedProperty autoAddMissingTextures;

    private bool showGlobalRules = true;
    private bool showLayerSetup = true;
    private bool showGrass = true;
    private bool showSlope;
    private bool showPeak;

    private void OnEnable()
    {
        slopeAngleMin = serializedObject.FindProperty("slopeAngleMin");
        peakHeightMin = serializedObject.FindProperty("peakHeightMin");
        autoAddMissingTextures = serializedObject.FindProperty("autoAddMissingTextures");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawFoldoutSection(ref showGlobalRules, "Height & Slope Thresholds", () =>
        {
            EditorGUILayout.PropertyField(slopeAngleMin);
            EditorGUILayout.PropertyField(peakHeightMin);
        });

        DrawRoleSection(
            ref showGrass,
            "Grass Texture",
            "grassDiffuse", "grassNormal", "grassMask",
            "grassTileSize", "grassTileOffset");

        DrawRoleSection(
            ref showSlope,
            "Slope Texture",
            "slopeDiffuse", "slopeNormal", "slopeMask",
            "slopeTileSize", "slopeTileOffset");

        DrawRoleSection(
            ref showPeak,
            "Peak Texture",
            "peakDiffuse", "peakNormal", "peakMask",
            "peakTileSize", "peakTileOffset");

        DrawFoldoutSection(ref showLayerSetup, "Layer Setup", () =>
        {
            EditorGUILayout.PropertyField(autoAddMissingTextures);
            EditorGUILayout.HelpBox("Terrain layers are generated/updated automatically under Assets/_Custom/Terrains/GeneratedLayers.", MessageType.Info);
        });

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space();

        if (GUILayout.Button("Apply Texture Rules", GUILayout.Height(30)))
        {
            foreach (UnityEngine.Object targetObject in targets)
            {
                TerrainTextureRules rules = (TerrainTextureRules)targetObject;
                if (rules == null)
                {
                    continue;
                }

                Terrain terrain = rules.GetComponent<Terrain>();

                Undo.RecordObject(rules, "Apply Terrain Texture Rules");
                if (terrain != null && terrain.terrainData != null)
                {
                    Undo.RecordObject(terrain.terrainData, "Apply Terrain Texture Rules");
                }

                rules.ApplyTextureRules();

                EditorUtility.SetDirty(rules);
                if (terrain != null && terrain.terrainData != null)
                {
                    EditorUtility.SetDirty(terrain.terrainData);
                }
            }
        }
    }

    private void DrawRoleSection(
        ref bool isExpanded,
        string title,
        string diffuseName,
        string normalName,
        string maskName,
        string tileSizeName,
        string tileOffsetName)
    {
        DrawFoldoutSection(ref isExpanded, title, () =>
        {
            SerializedProperty diffuse = serializedObject.FindProperty(diffuseName);
            SerializedProperty normal = serializedObject.FindProperty(normalName);
            SerializedProperty mask = serializedObject.FindProperty(maskName);
            SerializedProperty tileSize = serializedObject.FindProperty(tileSizeName);
            SerializedProperty tileOffset = serializedObject.FindProperty(tileOffsetName);

            EditorGUILayout.LabelField("Textures", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(diffuse, new GUIContent("Diffuse"));
            EditorGUILayout.PropertyField(normal, new GUIContent("Normal"));
            EditorGUILayout.PropertyField(mask, new GUIContent("Mask"));

            EditorGUILayout.Space(2f);
            EditorGUILayout.LabelField("Tiling", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(tileSize, new GUIContent("Tile Size"));
            EditorGUILayout.PropertyField(tileOffset, new GUIContent("Tile Offset"));
        });
    }

    private static void DrawFoldoutSection(ref bool isExpanded, string title, Action drawContent)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        isExpanded = EditorGUILayout.Foldout(isExpanded, title, true);

        if (isExpanded)
        {
            EditorGUI.indentLevel++;
            drawContent?.Invoke();
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
    }
}
