using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitManager : MonoBehaviour
{
    private UIManager uiManager;
    private MapManager mapManager;
    private BattleManager battleManager;
    private ProvincialManager provincialManager;
    private Dictionary<CubeIndex, List<Unit>> unitsInProvinces = new Dictionary<CubeIndex, List<Unit>>();
    public GameObject[] unitPrefabsByGrade; 
    

   
    private readonly int[] unitPurchaseCosts = { 10, 20, 40, 80 }; 
    private readonly int[] unitUpkeepCosts = { 5, 10, 15, 20 }; 
    private readonly int[] unitStrengths = { 1, 2, 3, 4 }; 

    public void Initialize(UIManager uiManager,MapManager mapManager,BattleManager battleManager,ProvincialManager provincialManager)
    {
        this.uiManager = uiManager;
        this.mapManager = mapManager;
        this.battleManager = battleManager;
        this.provincialManager = provincialManager;
    }

    public void SpawnUnitInProvince(Province province, int grade = 0)
    {
        if (grade < 0 || grade >= unitPurchaseCosts.Length)
        {
            Debug.LogError("Invalid unit grade.");
            return;
        }

        if (province.Treasury < unitPurchaseCosts[grade])
        {
            Debug.LogError("Not enough gold to spawn unit.");
            return;
        }

        province.Treasury -= unitPurchaseCosts[grade];
        
        Hex spawnHex = FindEmptyHexForUnit(province.Territory);

        if (spawnHex == null)
        {
            Debug.LogError("No empty hex available to spawn unit.");
            return;
        }

        Vector3 unitPosition = spawnHex.Visual.transform.position + Vector3.up;
        GameObject unitGO = Instantiate(unitPrefabsByGrade[grade], unitPosition, Quaternion.identity);

        Unit newUnit = new Unit(unitGO, unitUpkeepCosts[grade], unitStrengths[grade], province.OwnerId);
        newUnit.CurrentHex = spawnHex;

        AddUnit(newUnit, province.Center);
        spawnHex.unit = newUnit;
    }

    public void MergeUnits(Unit unit1, Unit unit2)
    {
        if (unit1.OwnerId != unit2.OwnerId)
        {
            Debug.LogError("Units belong to different owners and cannot be merged.");
            return;
        }

        int grade1 = GetGradeFromStrength(unit1.Strength);
        int grade2 = GetGradeFromStrength(unit2.Strength);

        if (grade1 != grade2 || grade1 >= unitPrefabsByGrade.Length - 1)
        {
            Debug.LogError($"Units cannot be merged or are already at maximum grade. unit1 {grade1} | unit2 {grade2}");
            return;
        }

        int newGrade = grade1 + 1;

        GameObject.Destroy(unit1.Visual);
        GameObject.Destroy(unit2.Visual);
        unit1.Visual = null;
        unit2.Visual = null;

        unit1.CurrentHex.unit = null;
        unit2.CurrentHex.unit = null;
        RemoveUnit(unit1, unit1.CurrentHex.Coordinates);
        RemoveUnit(unit2, unit2.CurrentHex.Coordinates);

        Vector3 unitPosition = unit1.CurrentHex.Visual.transform.position + Vector3.up;
        GameObject unitGO = Instantiate(unitPrefabsByGrade[newGrade], unitPosition, Quaternion.identity);

        Unit newUnit = new Unit(unitGO, unitUpkeepCosts[newGrade], unitStrengths[newGrade], unit1.OwnerId);
        newUnit.CurrentHex = unit1.CurrentHex;

        AddUnit(newUnit, unit1.CurrentHex.Coordinates);
        unit1.CurrentHex.unit = newUnit;
    }


    public int GetGradeFromStrength(int strength)
    {
        return Array.IndexOf(unitStrengths, strength);
    }

    public int GetUnitPrefabsLength()
    {
        return unitPrefabsByGrade.Length;
    }
    
    public void RemoveUnit(Unit unit, CubeIndex provinceCenter)
    {
        if (unitsInProvinces.ContainsKey(provinceCenter))
        {
            unitsInProvinces[provinceCenter].Remove(unit);
        }
    }


    public void AddUnit(Unit unit, CubeIndex provinceCenter)
    {
        if (!unitsInProvinces.ContainsKey(provinceCenter))
        {
            unitsInProvinces[provinceCenter] = new List<Unit>();
        }

        unitsInProvinces[provinceCenter].Add(unit);
    }

    public List<Unit> GetUnitsInProvince(CubeIndex provinceCenter)
    {
        if (unitsInProvinces.TryGetValue(provinceCenter, out List<Unit> units))
        {
            return units;
        }

        return new List<Unit>();
    }

    public void ResetUnitMovements()
    {
        foreach (var provinceUnits in unitsInProvinces.Values)
        {
            foreach (var unit in provinceUnits)
            {
                unit.HasMoved = false;
            }
        }
    }

    public void MakeUnitsNeutral(Province province)
    {
        if (province.GoldPerTurn < 0)
        {
            List<Unit> units = GetUnitsInProvince(province.Center);
            foreach (Unit unit in units)
            {
                //todo
            }
        }
    }
    
    public int GetUnitCost(int grade)
    {
        if (grade >= 0 && grade < unitPurchaseCosts.Length)
        {
            return unitPurchaseCosts[grade];
        }
        else
        {
            Debug.LogError("Invalid unit grade.");
            return 0;
        }
    }
    
    public void MoveUnit(Unit unit, Hex targetHex)
    {
        if (targetHex != null && unit != null && !unit.HasMoved)
        {
            if (targetHex.unit != null && targetHex.unit.OwnerId != unit.OwnerId)
            {
                if (unit.Strength > targetHex.unit.Strength)
                {
                    DestroyUnit(targetHex.unit);
                    PerformUnitMove(targetHex, unit);
                }
                else
                {
                    Debug.Log("Attacking unit is weaker and cannot capture the hex.");
                    return;
                }
            }
            else
            {
                PerformUnitMove(targetHex, unit);
            }
        }
        else
        {
            Debug.LogError("Invalid move.");
        }
    }

    public void DestroyUnit(Unit unit)
    {
        if (unit.Visual != null)
        {
            GameObject.Destroy(unit.Visual);
            unit.Visual = null;
        }
    
        unit.CurrentHex.unit = null;
        RemoveUnit(unit, unit.CurrentHex.Coordinates);
    }


    public void PerformUnitMove(Hex targetHex, Unit movingUnit)
    {
        if (movingUnit.Visual != null)
        {
            Vector3 newPosition = targetHex.Visual.transform.position;
            newPosition.y = 1f;
            movingUnit.Visual.transform.position = newPosition;
        }
        
        Hex currentHex = movingUnit.CurrentHex;
        currentHex.unit = null;
        targetHex.unit = movingUnit;
        movingUnit.CurrentHex = targetHex;
        movingUnit.HasMoved = true;
        
        if (targetHex.Owner == null || targetHex.Owner.OwnerId != movingUnit.OwnerId)
        {
            provincialManager.ChangeProvinceOwnership(targetHex, movingUnit.OwnerId);
        }
    }
    
    public Unit GetUnitAtHex(Hex hex)
    {
        if (hex != null && hex.unit != null)
        {
            return hex.unit;
        }
        return null;
    }
    
    private Hex FindEmptyHexForUnit(List<Hex> territory)
    {
        var emptyHexes = territory.Where(hex => hex.unit == null && !hex.HasBuilding).ToList();
        if (emptyHexes.Count == 0)
        {
            return null;
        }
        
        return emptyHexes[Random.Range(0, emptyHexes.Count)];
    }
    
}
