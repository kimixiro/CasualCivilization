using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    private GameManager gameManager;
    private MapManager mapManager;
    private UnitManager unitManager;
    private EconomyManager economyManager;
    private ProvincialManager provincialManager;
    private BattleManager battleManager;

    private int aiPlayerId;

    public void Initialize(GameManager gameManager, MapManager mapManager, UnitManager unitManager, EconomyManager economyManager, ProvincialManager provincialManager, BattleManager battleManager, int aiPlayerId)
    {
        this.gameManager = gameManager;
        this.mapManager = mapManager;
        this.unitManager = unitManager;
        this.economyManager = economyManager;
        this.provincialManager = provincialManager;
        this.battleManager = battleManager;
        this.aiPlayerId = aiPlayerId;
    }

    public IEnumerator MakeDecisions()
    {
        int currentPlayerId = gameManager.currentPlayerIndex;

        if (!gameManager.IsHumanPlayer(currentPlayerId))
        {
            Province aiProvince = provincialManager.GetProvinceById(currentPlayerId);

            BuyUnits(aiProvince);
            ReactToThreatsAndMoveUnits(aiProvince);

            yield return new WaitForSeconds(1.0f);
        }
    }

    private void BuyUnits(Province aiProvince)
    {
        while (aiProvince.Treasury >= unitManager.GetUnitCost(DecideUnitGradeToProduce()))
        {
            int unitGradeToProduce = DecideUnitGradeToProduce();
            unitManager.SpawnUnitInProvince(aiProvince, unitGradeToProduce);
        }
    }

    private int DecideUnitGradeToProduce()
    {
        return 0;
    }

    private void ReactToThreatsAndMoveUnits(Province aiProvince)
    {
        if (IsCastleUnderThreat(aiProvince))
        {
            ReactToCastleThreat(aiProvince);
        }
        

        MoveUnitsBasedOnStrategy(aiProvince);
        MergeAndMoveUnits(aiProvince);
    }

    private bool IsCastleUnderThreat(Province aiProvince)
    {
        Hex castleHex = mapManager.GetHex(aiProvince.Center);
        return IsHexUnderThreat(castleHex);
    }

    private bool IsHexUnderThreat(Hex hex)
    {
        return GetEnemyUnitsNearHex(hex).Any(unit => unit.Strength >= battleManager.CalculateDefenseStrength(hex, unit.OwnerId));
    }

    private void ReactToCastleThreat(Province aiProvince)
    {
        Unit nearestEnemy = FindNearestEnemyUnit(aiProvince.Center);
        if (nearestEnemy != null)
        {
            Unit defender = FindStrongestUnit(aiProvince);
            if (defender != null)
            {
                Hex enemyHex = mapManager.GetHex(nearestEnemy.CurrentHex.Coordinates);
                unitManager.MoveUnit(defender, enemyHex);
                defender.HasMoved = true;
            }
        }
    }

    private void MoveUnitsBasedOnStrategy(Province aiProvince)
    {
        List<Unit> units = unitManager.GetUnitsInProvince(aiProvince.Center).Where(u => !u.HasMoved).ToList();
        
        foreach (Unit unit in units)
        {
            Hex targetHex = FindStrategicTarget(unit, aiProvince);
            
            if (targetHex != null)
            {
                unitManager.MoveUnit(unit, targetHex);
                unit.HasMoved = true;
            }
        }
    }
    
    private Hex FindStrategicTarget(Unit unit, Province aiProvince)
    {
        Hex neutralTarget = FindNeutralExpansionTarget(aiProvince);
        if (neutralTarget != null)
        {
            return neutralTarget;
        }
        
        Hex weakPoint = FindWeakPointInEnemyLines(aiProvince);
        if (weakPoint != null)
        {
            return weakPoint;
        }
        
        Hex defensivePosition = FindDefensivePosition(aiProvince);
        return defensivePosition;
    }

    private Hex DecideUnitMovement(Unit unit, Province aiProvince)
    {
        return FindNearestTarget(unit, aiProvince);
    }

    private Hex FindNearestTarget(Unit unit, Province aiProvince)
    {
        Hex nearestTarget = null;
        float nearestDistance = float.MaxValue;

        var hexesInRange = mapManager.GetHexesInRange(unit.CurrentHex.Coordinates, 3);
        foreach (var hexEntry in hexesInRange)
        {
            Hex hex = hexEntry.Value;
            float distance = Vector3.Distance(unit.CurrentHex.Visual.transform.position, hex.Visual.transform.position);
            if (distance < nearestDistance && (hex.Owner == null || hex.Owner.OwnerId != aiPlayerId))
            {
                nearestTarget = hex;
                nearestDistance = distance;
            }
        }

        return nearestTarget;
    }

    private IEnumerable<Unit> GetEnemyUnitsNearHex(Hex hex)
    {
        List<Unit> enemyUnits = new List<Unit>();

        foreach (CubeIndex neighborIndex in mapManager.GetNeighbors(hex.Coordinates))
        {
            Hex nearbyHex = mapManager.GetHex(neighborIndex);
            if (nearbyHex != null && nearbyHex.unit != null && nearbyHex.unit.OwnerId != aiPlayerId)
            {
                enemyUnits.Add(nearbyHex.unit);
            }
        }

        return enemyUnits;
    }

    private Unit FindNearestEnemyUnit(CubeIndex center)
    {
        Unit nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (var province in provincialManager.provinces.Values)
        {
            foreach (Unit unit in unitManager.GetUnitsInProvince(province.Center))
            {
                if (unit.OwnerId != aiPlayerId)
                {
                    Hex unitHex = mapManager.GetHex(unit.CurrentHex.Coordinates);
                    float distance = Vector3.Distance(mapManager.GetHexPosition(center), unitHex.Visual.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestEnemy = unit;
                        nearestDistance = distance;
                    }
                }
            }
        }

        return nearestEnemy;
    }

    private Unit FindStrongestUnit(Province aiProvince)
    {
        Unit strongestUnit = null;
        int maxStrength = 0;

        foreach (Unit unit in unitManager.GetUnitsInProvince(aiProvince.Center))
        {
            if (!unit.HasMoved && unit.Strength > maxStrength)
            {
                strongestUnit = unit;
                maxStrength = unit.Strength;
            }
        }

        return strongestUnit;
    }

    private void MergeAndMoveUnits(Province aiProvince)
    {

        HashSet<Unit> processedUnits = new HashSet<Unit>();
        List<Unit> unitsInProvince = unitManager.GetUnitsInProvince(aiProvince.Center);
        unitsInProvince.Sort((a, b) => b.Strength.CompareTo(a.Strength));

        for (int i = 0; i < unitsInProvince.Count; i++)
        {
            for (int j = i + 1; j < unitsInProvince.Count; j++)
            {
                Unit unit1 = unitsInProvince[i];
                Unit unit2 = unitsInProvince[j];
                
                if (processedUnits.Contains(unit1) || processedUnits.Contains(unit2))
                {
                    continue;
                }

                if (unit1.OwnerId == unit2.OwnerId && CanUnitsMerge(unit1, unit2))
                {
                    unitManager.MergeUnits(unit1, unit2);
                    processedUnits.Add(unit1);
                    processedUnits.Add(unit2);
                    
                    Unit mergedUnit = unitManager.GetUnitAtHex(unit1.CurrentHex);
                    if (mergedUnit != null && !mergedUnit.HasMoved)
                    {
                        Hex targetHex = DecideUnitMovement(mergedUnit, aiProvince);
                        if (targetHex != null)
                        {
                            unitManager.MoveUnit(mergedUnit, targetHex);
                        }
                        processedUnits.Add(mergedUnit);
                    }
                    
                    break;
                }
            }
        }
    }
    
    private bool CanUnitsMerge(Unit unit1, Unit unit2)
    {
        int grade1 = unitManager.GetGradeFromStrength(unit1.Strength);
        int grade2 = unitManager.GetGradeFromStrength(unit2.Strength);
        return grade1 == grade2 && grade1 < unitManager.GetUnitPrefabsLength() - 1;
    }
    
    private Hex FindNeutralExpansionTarget(Province aiProvince)
    {
        return mapManager.GetAllHexes()
            .Where(hexEntry => hexEntry.Value.Owner == null)
            .OrderBy(hexEntry => Vector3.Distance(mapManager.GetHexPosition(aiProvince.Center), hexEntry.Value.Visual.transform.position))
            .Select(hexEntry => hexEntry.Value)
            .FirstOrDefault();
    }
    private Hex FindWeakPointInEnemyLines(Province aiProvince)
    {
        return mapManager.GetAllHexes()
            .Where(hexEntry => hexEntry.Value.Owner != null && hexEntry.Value.Owner.OwnerId != aiPlayerId && IsHexWeak(hexEntry.Value))
            .OrderBy(hexEntry => Vector3.Distance(mapManager.GetHexPosition(aiProvince.Center), hexEntry.Value.Visual.transform.position))
            .Select(hexEntry => hexEntry.Value)
            .FirstOrDefault();
    }
    private Hex FindDefensivePosition(Province aiProvince)
    {
        return aiProvince.Territory
            .Where(hex => IsBorderOrCritical(hex, aiProvince))
            .OrderBy(hex => hex.unit == null ? 0 : 1)
            .ThenBy(hex => Vector3.Distance(mapManager.GetHexPosition(aiProvince.Center), hex.Visual.transform.position))
            .FirstOrDefault();
    }
    
    private bool IsHexWeak(Hex hex)
    {
        int defenseStrength = battleManager.CalculateDefenseStrength(hex, aiPlayerId);
        return defenseStrength < CalculateAverageEnemyStrengthNearHex(hex);
    }
    
    private bool IsBorderOrCritical(Hex hex, Province aiProvince)
    {
        bool isBorder = mapManager.GetNeighbors(hex.Coordinates)
            .Select(index => mapManager.GetHex(index))
            .Any(neighborHex => neighborHex.Owner != null && neighborHex.Owner.OwnerId != aiProvince.OwnerId);
        bool isCritical = hex.HasBuilding;
        return isBorder || isCritical;
    }
    
    private int CalculateAverageEnemyStrengthNearHex(Hex hex)
    {
        var enemyUnits = GetEnemyUnitsNearHex(hex);
        if (enemyUnits.Any())
        {
            return (int)enemyUnits.Average(unit => unit.Strength);
        }
        return 0;
    }
}
