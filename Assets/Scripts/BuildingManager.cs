using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public GameObject castlePrefab;
    public GameObject towerPrefab;
    
    public void Initialize()
    {
       
    }

    public void BuildCastle(Hex hex)
    {
        if (hex.HasBuilding)
        {
            Debug.LogError("Hex already has a building.");
            return;
        }

        Vector3 castlePosition = new Vector3(hex.Visual.transform.position.x, 1f, hex.Visual.transform.position.z);
        GameObject castle = Instantiate(castlePrefab, castlePosition, Quaternion.identity);
        int ownerId = hex.Owner != null ? hex.Owner.OwnerId : 0;
        hex.Building = new Building(castle, CalculateStrength(BuildingType.Castle), BuildingType.Castle, ownerId);
        hex.HasBuilding = true;
    }

    public void BuildTower(Hex hex)
    {
        if (hex.HasBuilding)
        {
            Debug.LogError("Hex already has a building.");
            return;
        }

        Vector3 towerPosition = new Vector3(hex.Visual.transform.position.x, 1f, hex.Visual.transform.position.z);
        GameObject tower = Instantiate(towerPrefab, towerPosition, Quaternion.identity);
        int ownerId = hex.Owner != null ? hex.Owner.OwnerId : 0;
        hex.Building = new Building(tower, CalculateStrength(BuildingType.Tower), BuildingType.Tower, ownerId);
        hex.HasBuilding = true;
    }

    private int CalculateStrength(BuildingType type)
    {
        switch (type)
        {
            case BuildingType.Castle:
                return 3;
            case BuildingType.Tower:
                return 2;
            default:
                return 0;
        }
    }
}