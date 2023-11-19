using UnityEngine;

public class UnitDragHandler : MonoBehaviour
{
    public Camera mainCamera;
    private GameObject currentlyDragging;
    public MapManager mapManager;
    public UnitManager unitManager;
    public ProvincialManager provincialManager;
    public BattleManager battleManager; 
    public int playerID; 
    private Hex currentHex;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag();
        }

        if (currentlyDragging != null)
        {
            DragUnit();
        }

        if (Input.GetMouseButtonUp(0) && currentlyDragging != null)
        {
            TryDropUnit();
        }
    }

    private void TryStartDrag()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.CompareTag("Hex"))
            {
                Hex hex = mapManager?.GetHex(ParseCoordinatesFromName(hit.collider.gameObject.name));
                if (hex != null && hex.unit != null && hex.unit.OwnerId == playerID)
                {
                    StartDraggingUnit(hex);
                }
            }
        }
    }

    private void StartDraggingUnit(Hex hex)
    {
        currentHex = hex;
        currentlyDragging = hex.unit.Visual;
    }

    private void DragUnit()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) 
        {
            Vector3 newPosition = hit.point;
            newPosition.y += 0.5f;
            currentlyDragging.transform.position = newPosition;
        }
    }

    private void TryDropUnit()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.CompareTag("Hex"))
            {
                MoveOrMergeUnit(hit.collider.gameObject.name);
            }
        }

        currentlyDragging = null;
    }


    private void MoveOrMergeUnit(string hexName)
    {
        CubeIndex hexIndex = ParseCoordinatesFromName(hexName);
        Hex targetHex = mapManager?.GetHex(hexIndex);
        Unit movingUnit = currentHex.unit;

        if (targetHex != null && movingUnit != null && !movingUnit.HasMoved)
        {
            if (targetHex.unit != null && targetHex.unit.Strength == movingUnit.Strength)
            {
                unitManager.MergeUnits(movingUnit, targetHex.unit);
            }
            else if (battleManager.CanCapture(targetHex, movingUnit))
            {
                PerformUnitMove(targetHex, movingUnit);
            }
            else
            {
                ResetUnitPosition();
                Debug.LogError("Cannot capture or merge with this hex.");
            }
        }
        else
        {
            ResetUnitPosition();
            Debug.LogError("Invalid move.");
        }
    }
    
    private void PerformUnitMove(Hex targetHex, Unit movingUnit)
    {
        Vector3 newPosition = targetHex.Visual.transform.position;
        newPosition.y = 1f;
        movingUnit.Visual.transform.position = newPosition;

        currentHex.unit = null;
        targetHex.unit = movingUnit;
        movingUnit.CurrentHex = targetHex;
        movingUnit.HasMoved = true;
        
        if (targetHex.Owner == null || targetHex.Owner.OwnerId != playerID)
        {
            provincialManager?.ChangeProvinceOwnership(targetHex, playerID);
        }
    }

    private void ResetUnitPosition()
    {
        if (currentHex != null && currentHex.unit != null)
        {
            Vector3 originalPosition = currentHex.Visual.transform.position;
            originalPosition.y = 1f; 
            currentHex.unit.Visual.transform.position = originalPosition;
        }
    }

    private CubeIndex ParseCoordinatesFromName(string name)
    {
        string[] parts = name.Split('_');
        if (parts.Length == 4)
        {
            int.TryParse(parts[1], out int q);
            int.TryParse(parts[2], out int r);
            int.TryParse(parts[3], out int s);
            return new CubeIndex(q, r, s);
        }
        return new CubeIndex(); 
    }
}
