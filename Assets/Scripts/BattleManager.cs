using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private MapManager mapManager;
    
    public void Initialize(MapManager mapManager)
    {
        this.mapManager = mapManager;
    }

    public bool CanCapture(Hex targetHex, Unit attackingUnit)
    {
        if (IsHexOwnedBy(attackingUnit.OwnerId, targetHex))
        {
            return true;
        }

        int totalDefenseStrength = CalculateDefenseStrength(targetHex, attackingUnit.OwnerId);
        return attackingUnit.Strength > totalDefenseStrength;
    }

    private bool IsHexOwnedBy(int ownerId, Hex hex)
    {
        return (hex.Owner != null && hex.Owner.OwnerId == ownerId) ||
               (hex.unit != null && hex.unit.OwnerId == ownerId);
    }

    private int CalculateDefenseStrength(Hex hex, int attackingOwnerId)
    {
        int defenseStrength = 0;
        
        if (hex.unit != null && hex.unit.OwnerId != attackingOwnerId)
        {
            defenseStrength += hex.unit.Strength;
        }
        
        if (hex.HasBuilding && (hex.Owner == null || hex.Owner.OwnerId != attackingOwnerId))
        {
            defenseStrength += hex.Building.Strength;
        }
        
        foreach (CubeIndex neighborIndex in mapManager.GetNeighbors(hex.Coordinates))
        {
            Hex neighborHex = mapManager.GetHex(neighborIndex);
            if (neighborHex != null && neighborHex.Owner == hex.Owner)
            {
                if (neighborHex.unit != null && neighborHex.unit.OwnerId != attackingOwnerId)
                {
                    defenseStrength += neighborHex.unit.Strength;
                }

                if (neighborHex.HasBuilding && neighborHex.Building.OwnerId != attackingOwnerId)
                {
                    defenseStrength += neighborHex.Building.Strength;
                }
            }
        }

        return defenseStrength;
    }

    
}