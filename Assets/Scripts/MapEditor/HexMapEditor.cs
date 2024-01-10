using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class HexMapEditor : EditorWindow
{

    private HexMapData currentMap;
    private HexMapData selectedMap;
    private int mapRadius = 5;
    private float hexRadius = 1.15f;
    private HexType selectedHexType = HexType.Plain;

    private GameObject plainHexPrefab;
    private GameObject forestHexPrefab;
    private GameObject waterHexPrefab;
    private List<GameObject> instantiatedHexes = new List<GameObject>();

    [MenuItem("Window/Hex Map Editor")]
    public static void ShowWindow()
    {
        GetWindow<HexMapEditor>("Hex Map Editor");
    }

    void OnEnable()
    {
        LoadOrCreateMap();
    }

    void OnDestroy()
    {
        UnloadMapVisuals();
    }

    void OnGUI()
    {
        GUILayout.Label("Hex Map Editor", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        selectedMap = (HexMapData) EditorGUILayout.ObjectField("Select Map", selectedMap, typeof(HexMapData), false);
        if (EditorGUI.EndChangeCheck() && selectedMap != null)
        {
            currentMap = selectedMap;
        }

        plainHexPrefab = (GameObject)EditorGUILayout.ObjectField("Plain Hex Prefab", plainHexPrefab, typeof(GameObject), false);
        forestHexPrefab = (GameObject)EditorGUILayout.ObjectField("Forest Hex Prefab", forestHexPrefab, typeof(GameObject), false);
        waterHexPrefab = (GameObject)EditorGUILayout.ObjectField("Water Hex Prefab", waterHexPrefab, typeof(GameObject), false);
        
        GUILayout.Space(10);
        GUILayout.Label("Create New Map", EditorStyles.boldLabel);
        mapRadius = EditorGUILayout.IntField("Map Radius", mapRadius);
        if (GUILayout.Button("Create New Map"))
        {
            CreateNewMapWithRadius(mapRadius);
        }

        if (GUILayout.Button("Save Map"))
        {
            SaveCurrentMap();
        }

        if (GUILayout.Button("Load Map Visuals"))
        {
            LoadMapVisuals();
        }

        if (GUILayout.Button("Unload Map Visuals"))
        {
            UnloadMapVisuals();
        }

        GUILayout.Space(10);
        GUILayout.Label("Hex Type Selection", EditorStyles.boldLabel);
        selectedHexType = (HexType) EditorGUILayout.EnumPopup("Select Hex Type", selectedHexType);

        if (GUI.changed && currentMap != null)
        {
            EditorUtility.SetDirty(currentMap);
        }
    }

    private void LoadOrCreateMap()
    {
        if (selectedMap == null)
        {
            string path = "Assets/Config/HexMapData.asset";
            selectedMap = AssetDatabase.LoadAssetAtPath<HexMapData>(path);
            currentMap = selectedMap;
        }
    }

    private void CreateNewMapWithRadius(int radius) {
        currentMap = ScriptableObject.CreateInstance<HexMapData>();

        for (int q = -radius; q <= radius; q++) 
        {
            int r1 = Mathf.Max(-radius, -q - radius);
            int r2 = Mathf.Min(radius, -q + radius);
            for (int r = r1; r <= r2; r++) 
            {
                int s = -q - r;
                CubeIndex index = new CubeIndex(q, r, s);
                CellConfig config = new CellConfig { Type = HexType.Plain }; 
                currentMap.AddCellConfig(index, config); 
            }
        }

        string path = GetUniqueAssetPath("Assets/Config/HexMapData.asset");
        AssetDatabase.CreateAsset(currentMap, path);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = currentMap;

        selectedMap = currentMap;
    }

    private string GetUniqueAssetPath(string originalPath)
    {
        string path = originalPath;
        int counter = 1;

        while (AssetDatabase.LoadAssetAtPath<HexMapData>(path) != null)
        {
            path = originalPath.Insert(originalPath.Length - 6, "_" + counter.ToString());
            counter++;
        }

        return path;
    }

    private void SaveCurrentMap()
    {
        if (currentMap == null) return;

        EditorUtility.SetDirty(currentMap);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void LoadMapVisuals()
    {
        if (currentMap == null) return;

        UnloadMapVisuals();

        foreach (var kvp in currentMap.GetMapData())
        {
            CubeIndex index = kvp.Key;
            CellConfig cellConfig = kvp.Value;
            Vector3 position = HexCoordToPosition(index);

            GameObject prefab = GetPrefabForHexType(cellConfig.Type);
            if (prefab != null)
            {
                instantiatedHexes.Add(Instantiate(prefab, position, Quaternion.identity));
            }
        }
    }


    private GameObject GetPrefabForHexType(HexType type)
    {
        switch (type)
        {
            case HexType.Plain:
                return plainHexPrefab;
            case HexType.Forest:
                return forestHexPrefab;
            case HexType.Water:
                return waterHexPrefab;
            default:
                return null;
        }
    }

    private void UnloadMapVisuals()
    {
        foreach (var hex in instantiatedHexes)
        {
            if (hex != null)
            {
                DestroyImmediate(hex);
            }
        }

        instantiatedHexes.Clear();
    }
    
    private Vector3 HexCoordToPosition(CubeIndex index) {
        //1.15 hexRadius
        float hexWidth = Mathf.Sqrt(3) * 1.15f; 
        float hexHeight = 1.5f * 1.15f;
        float x = hexWidth * (index.x + index.z * 0.5f);
        float z = hexHeight * index.z;

        return new Vector3(x, 0, z);
    }
    
}
