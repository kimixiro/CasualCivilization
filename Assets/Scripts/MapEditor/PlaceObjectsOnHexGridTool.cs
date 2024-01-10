using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public enum BrushType
{
    PlacePrefab,
    RemovePrefab
}

[EditorTool("Place Objects on Hex Grid Tool")]
public class PlaceObjectsOnHexGridTool : EditorTool
{
    static Texture2D _toolIcon;
    readonly GUIContent _iconContent = new GUIContent
    {
        image = _toolIcon,
        text = "Place Objects on Hex Grid Tool",
        tooltip = "Place Objects on Hex Grid Tool"
    };

    VisualElement _toolRootElement;
    ObjectField _prefabObjectField;
    ObjectField _hexMapDataField;
    HexMapData _currentHexMapData;
    
    [SerializeField] private GameObject plainPrefab;
    [SerializeField] private GameObject forestPrefab;
    [SerializeField] private GameObject waterPrefab;
    
    float _hexRadius = 1.0f;

    bool _receivedClickUpEvent;
    bool HasPlaceableObject => _prefabObjectField?.value != null;
    
    private BrushType _currentBrushType;

    public override GUIContent toolbarIcon => _iconContent;

    public override void OnActivated()
    {
        _toolRootElement = new VisualElement();
        _prefabObjectField = new ObjectField { allowSceneObjects = true, objectType = typeof(GameObject) };
        _hexMapDataField = new ObjectField { objectType = typeof(HexMapData) };
        var hexRadiusField = new FloatField("Hex Radius") { value = _hexRadius };

        hexRadiusField.RegisterValueChangedCallback(evt => _hexRadius = evt.newValue);
        _hexMapDataField.RegisterValueChangedCallback(evt => _currentHexMapData = evt.newValue as HexMapData);
        
        Button loadMapButton = new Button(() => LoadMap()) { text = "Load Map" };
        Button unloadMapButton = new Button(() => UnloadMap()) { text = "Unload Map" };
        Button openPrefabWindowButton = new Button(() => OpenPrefabSelectionWindow()) { text = "Select Prefabs" };

        ToolbarMenu brushSelectionMenu = new ToolbarMenu();
        brushSelectionMenu.text = "Choose Brush";
        brushSelectionMenu.menu.AppendAction("Place Prefab", a => SetBrush(BrushType.PlacePrefab));
        brushSelectionMenu.menu.AppendAction("Remove Prefab", a => SetBrush(BrushType.RemovePrefab));

        _toolRootElement.Add(new Label("Place Objects Tool"));
        _toolRootElement.Add(_prefabObjectField);
        _toolRootElement.Add(_hexMapDataField);
        _toolRootElement.Add(hexRadiusField);
        _toolRootElement.Add(loadMapButton);
        _toolRootElement.Add(unloadMapButton);
        _toolRootElement.Add(openPrefabWindowButton);
        _toolRootElement.Add(brushSelectionMenu);

        _toolRootElement.style.position = Position.Absolute;
        _toolRootElement.style.left = 10;
        _toolRootElement.style.bottom = 10;

        var sv = SceneView.lastActiveSceneView;
        sv.rootVisualElement.Add(_toolRootElement);

        SceneView.beforeSceneGui += BeforeSceneGUI;
    }

    public override void OnWillBeDeactivated()
    {
        _toolRootElement?.RemoveFromHierarchy();
        SceneView.beforeSceneGui -= BeforeSceneGUI;
    }

    void BeforeSceneGUI(SceneView sceneView)
    {
        if (!ToolManager.IsActiveTool(this))
            return;

        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            _receivedClickUpEvent = true;
            Event.current.Use();
        }
    }

    public override void OnToolGUI(EditorWindow window)
    {
        if (!(window is SceneView) || !ToolManager.IsActiveTool(this) || !HasPlaceableObject)
            return;

        // Get the current mouse position in the scene.
        Vector3 mousePositionInScene = GetCurrentMousePositionInScene(window);

        // If the mouse position is valid, snap it to the hex grid and draw the disc.
        if (mousePositionInScene != Vector3.zero)
        {
            Vector3 snappedPosition = SnapToHexGrid(mousePositionInScene, _hexRadius);
            Handles.DrawWireDisc(snappedPosition, Vector3.up, _hexRadius);

            // Place the object when the mouse is clicked.
            if (_receivedClickUpEvent)
            {
                PlaceObject(snappedPosition);
                _receivedClickUpEvent = false;
            }
        }

        window.Repaint();
    }

    Vector3 GetCurrentMousePositionInScene(EditorWindow window)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        // Set the raycast distance to a sufficiently large number.
        float maxDistance = 10000f;
        // Define a layer mask if you want the raycast to only hit certain layers.
        int layerMask = LayerMask.GetMask("YourLayerName"); // Replace "YourLayerName" with your actual layer

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
        {
            return hit.point;
        }

        // If no hit, or if you don't have a specific layer, use a default plane at y = 0
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return Vector3.zero; // Return zero if nothing is hit (as a last resort)
    }
    
    private Vector3 SnapToHexGrid(Vector3 worldPosition, float hexRadius)
    {
        CubeIndex cubeIndex = WorldPositionToCubeIndex(worldPosition, hexRadius);
        return CubeIndexToWorldPosition(cubeIndex, hexRadius);
    }

    private void PlaceObject(Vector3 position)
    {
        GameObject prefab = _prefabObjectField.value as GameObject;
        if (prefab == null)
        {
            Debug.LogError("Prefab is not assigned.");
            return;
        }

        // Find the Map parent object in the scene, or create one if it doesn't exist
        GameObject mapParent = GameObject.Find("Map") ?? new GameObject("Map");

        GameObject newObjectInstance;

        // Check if the prefab is part of a prefab asset (connected to a prefab in the project)
        if (PrefabUtility.IsPartOfAnyPrefab(prefab))
        {
            var prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefab);
            var loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            newObjectInstance = (GameObject)PrefabUtility.InstantiatePrefab(loadedPrefab, mapParent.transform);
        }
        else
        {
            newObjectInstance = Instantiate(prefab, mapParent.transform);
        }

        // Set the instantiated prefab's position to the snapped hex grid position
        newObjectInstance.transform.position = position;

        // Register the action for undo
        Undo.RegisterCreatedObjectUndo(newObjectInstance, "Place new object");

        if (_currentHexMapData != null)
        {
            CubeIndex cellIndex = WorldPositionToCubeIndex(position, _hexRadius);
            CellConfig newCellConfig = new CellConfig { Type = HexType.Plain }; // This should be determined based on the object placed
            _currentHexMapData.AddCellConfig(cellIndex, newCellConfig);

            // Register the change for undo
            Undo.RecordObject(_currentHexMapData, "Update HexMapData");
            EditorUtility.SetDirty(_currentHexMapData);
        }
    }
    
    private void LoadMap()
    {
        // First, check if there's a selected HexMapData asset.
        if (_currentHexMapData == null)
        {
            Debug.LogError("No HexMapData asset is selected to load.");
            return;
        }

        // Find or create the Map parent object in the scene to hold all the instantiated hex cells.
        GameObject mapParent = GameObject.Find("Map") ?? new GameObject("Map");

        // Clear any existing children under the map parent to start fresh.
        // This is similar to UnloadMap but targets child objects.
        foreach (Transform child in mapParent.transform)
        {
            DestroyImmediate(child.gameObject);
        }

        // Use the data from HexMapData to instantiate the prefabs.
        for (int i = 0; i < _currentHexMapData.Keys.Count; i++)
        {
            SerializableCubeIndex cubeIndex = _currentHexMapData.Keys[i];
            SerializableCellConfig cellConfig = _currentHexMapData.Values[i];

            // Convert the CubeIndex to a world position.
            Vector3 position = CubeIndexToWorldPosition(new CubeIndex(cubeIndex.x, cubeIndex.y, cubeIndex.z), _hexRadius);

            // Instantiate the prefab for this cell configuration.
            // You will need to determine how to map cellConfig.Type to a specific prefab.
            GameObject prefabToInstantiate = GetPrefabForCellConfig(cellConfig);
            if (prefabToInstantiate != null)
            {
                GameObject cellObject = PrefabUtility.InstantiatePrefab(prefabToInstantiate, mapParent.transform) as GameObject;
                if (cellObject != null)
                {
                    cellObject.transform.position = position;
                }
                else
                {
                    Debug.LogError("Failed to instantiate prefab for cell at position: " + position);
                }
            }
            else
            {
                Debug.LogError("No prefab assigned for cell type: " + cellConfig.Type);
            }
        }
    }

    private void UnloadMap()
    {
        // Find the Map GameObject in the scene.
        GameObject mapGameObject = GameObject.Find("Map");
    
        // If the Map GameObject exists, destroy it.
        if (mapGameObject != null)
        {
            // Since we are in the editor, we use DestroyImmediate instead of Destroy.
            DestroyImmediate(mapGameObject);
        
            // Additionally, if you want to clear the reference from the current HexMapData field, do so.
            _hexMapDataField.value = null;
            _currentHexMapData = null;
        }
        else
        {
            Debug.LogWarning("No 'Map' GameObject found in the scene to unload.");
        }
    }

    private void OpenPrefabSelectionWindow()
    {
        // Implement the logic to open a prefab selection window
        // This will likely involve creating a new EditorWindow-derived class that displays prefabs
    }

    private void SetBrush(BrushType brushType)
    {
        _currentBrushType = brushType;
    }
    
    private GameObject GetPrefabForCellConfig(SerializableCellConfig cellConfig)
    {
        // Use the serialized fields to return the correct prefab.
        switch (cellConfig.Type)
        {
            case HexType.Plain:
                return plainPrefab;
            case HexType.Forest:
                return forestPrefab;
            case HexType.Water:
                return waterPrefab;
            default:
                Debug.LogError("Unhandled hex type: " + cellConfig.Type);
                return null;
        }
    }
    
    private CubeIndex WorldPositionToCubeIndex(Vector3 worldPosition, float hexRadius)
    {
        // Calculate the axial coordinates (q, r) based on the world position
        float q = (worldPosition.x * Mathf.Sqrt(3) / 3 - worldPosition.z / 3) / hexRadius;
        float r = worldPosition.z * 2/3 / hexRadius;

        // Round the axial coordinates to the nearest whole number to find the hex cube coordinates
        int rx = Mathf.RoundToInt(q);
        int rz = Mathf.RoundToInt(r);
        int ry = Mathf.RoundToInt(-rx-rz);

        // Correct the rounded coordinates
        float x_diff = Mathf.Abs(rx - q);
        float y_diff = Mathf.Abs(ry + rx + rz);
        float z_diff = Mathf.Abs(rz - r);

        if (x_diff > y_diff && x_diff > z_diff)
            rx = -ry - rz;
        else if (y_diff > z_diff)
            ry = -rx - rz;
        else if (z_diff > x_diff && z_diff > y_diff)
            rz = -rx - ry;

        // Return the CubeIndex
        return new CubeIndex(rx, ry, rz);
    }
    
    private Vector3 CubeIndexToWorldPosition(CubeIndex cubeIndex, float hexRadius)
    {
        // Determine the size of the hex based on the radius
        float hexWidth = Mathf.Sqrt(3) * hexRadius;
        float hexHeight = 2f * hexRadius;

        // Convert cube coordinates back to world position
        Vector3 worldPosition = new Vector3(
            hexWidth * (cubeIndex.x + cubeIndex.z / 2f),
            0, // Assuming y is always 0 for the hex grid
            hexHeight * 3/4 * cubeIndex.z
        );

        return worldPosition;
    }

}