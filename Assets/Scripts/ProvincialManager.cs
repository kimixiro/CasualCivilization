using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProvincialManager : MonoBehaviour
{
    private MapManager mapManager;
    private BuildingManager buildingManager;
    private UnitManager unitManager;
    public Color[] provinceColors;
    public int minimumProvinceDistance;
    public int provinceRadius;
    public Color neutralColor;
    public Dictionary<CubeIndex, Province> provinces = new Dictionary<CubeIndex, Province>();

    public void Initialize(int totalPlayers, MapManager mapManager, BuildingManager buildingManager,UnitManager unitManager)
    {
        this.mapManager = mapManager;
        this.buildingManager = buildingManager;
        this.unitManager = unitManager;
        InitializeProvinces(totalPlayers);
    }

    void InitializeProvinces(int numberOfProvinces)
    {
        for (int i = 0; i < numberOfProvinces; i++)
        {
            CreateProvince(i);
        }
    }

    void CreateProvince(int provinceId)
    {
        CubeIndex? center = FindSuitableProvinceCenter();
        if (center.HasValue)
        {
            Color provinceColor = provinceColors[provinceId % provinceColors.Length];
            Province newProvince = new Province(center.Value, provinceColor);
            newProvince.OwnerId = provinceId;
            List<Hex> territory = AssignTerritory(center.Value, provinceRadius, newProvince);
            newProvince.Territory = territory;
            provinces[center.Value] = newProvince;
            ColorProvinceArea(newProvince);
            
            Hex centerHex = mapManager.GetHex(center.Value);
        if (centerHex != null && !centerHex.HasBuilding)
        {
            buildingManager.BuildCastle(centerHex);
            centerHex.Owner = newProvince;
        }
        }
    }

    List<Hex> AssignTerritory(CubeIndex center, int radius, Province province)
    {
        int targetArea = Mathf.CeilToInt(Mathf.PI * radius * radius);
        var territory = new List<Hex>();
        var candidateHexes = new Queue<CubeIndex>();
        var visited = new HashSet<CubeIndex>();

        if (mapManager.GetHex(center)?.Owner == null)
        {
            candidateHexes.Enqueue(center);
            visited.Add(center);
        }

        while (territory.Count < targetArea && candidateHexes.Count > 0)
        {
            CubeIndex currentHexIndex = candidateHexes.Dequeue();
            Hex currentHex = mapManager.GetHex(currentHexIndex);

            if (currentHex != null && currentHex.Owner == null)
            {
                currentHex.Owner = province;
                territory.Add(currentHex);
            }

            foreach (CubeIndex neighborIndex in mapManager.GetNeighbors(currentHexIndex))
            {
                Hex neighborHex = mapManager.GetHex(neighborIndex);
                if (!visited.Contains(neighborIndex) && neighborHex != null && neighborHex.Owner == null)
                {
                    visited.Add(neighborIndex);
                    candidateHexes.Enqueue(neighborIndex);
                }
            }
        }

        return territory;
    }

    CubeIndex? FindSuitableProvinceCenter()
    {
        List<CubeIndex> potentialCenters = new List<CubeIndex>(mapManager.GetAllHexes().Keys);
        Shuffle(potentialCenters);

        foreach (var center in potentialCenters)
        {
            if (IsValidProvinceCenter(center))
            {
                return center;
            }
        }
        return null;
    }

    bool IsValidProvinceCenter(CubeIndex candidateCenter)
    {
        Hex hex = mapManager.GetHex(candidateCenter);
        if (hex != null && hex.Owner != null)
        {
            return false;
        }

        foreach (var province in provinces)
        {
            if (Vector3.Distance(mapManager.GetHexPosition(candidateCenter), mapManager.GetHexPosition(province.Value.Center)) < minimumProvinceDistance)
            {
                return false;
            }
        }

        int edgeBuffer = provinceRadius;
        if (candidateCenter.x < -mapManager.mapRadius + edgeBuffer || candidateCenter.x > mapManager.mapRadius - edgeBuffer ||
            candidateCenter.y < -mapManager.mapRadius + edgeBuffer || candidateCenter.y > mapManager.mapRadius - edgeBuffer ||
            candidateCenter.z < -mapManager.mapRadius + edgeBuffer || candidateCenter.z > mapManager.mapRadius - edgeBuffer)
        {
            return false;
        }

        return true;
    }

    void ColorProvinceArea(Province province)
    {
        foreach (var hex in province.Territory)
        {
            hex.UpdateColor(province.Color);
        }

        Hex centerHex = mapManager.GetHex(province.Center);
        if (centerHex != null)
        {
            centerHex.UpdateColor(province.Color);
        }
    }

    void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    public void ChangeProvinceOwnership(Hex hex, int newOwnerId)
    {
        Province oldProvince = provinces.Values.FirstOrDefault(p => p.Territory.Contains(hex));

        if (oldProvince != null && oldProvince.Center == hex.Coordinates)
        {
            foreach (var territoryHex in oldProvince.Territory)
            {
                territoryHex.Owner = null;
                territoryHex.UpdateColor(neutralColor);
                if (hex.HasBuilding)
                {
                    hex.DestroyBuilding(); 
                }
            }
            provinces.Remove(oldProvince.Center);
            return;
        }

        if (oldProvince != null)
        {
            oldProvince.Territory.Remove(hex);
        }

        Province newProvince = GetProvinceById(newOwnerId);
        hex.Owner = newProvince;

        if (newProvince != null && !provinces.ContainsKey(hex.Coordinates))
        {
            newProvince.Territory.Add(hex);
            hex.UpdateColor(newProvince.Color);
        }
        else
        {
            hex.UpdateColor(neutralColor);
        }

        if (oldProvince != null)
        {
            CheckProvinceConnections(oldProvince);
        }

        if (newProvince != null)
        {
            CheckProvinceConnections(newProvince);
        }
    }

    public void CheckProvinceConnections(Province province)
    {
        HashSet<Hex> connectedTerritory = new HashSet<Hex>();
        Queue<Hex> toExplore = new Queue<Hex>();

        Hex centerHex = mapManager.GetHex(province.Center);
        if (centerHex != null && centerHex.Owner == province)
        {
            toExplore.Enqueue(centerHex);
            connectedTerritory.Add(centerHex);
        }

        while (toExplore.Count > 0)
        {
            Hex currentHex = toExplore.Dequeue();
            foreach (CubeIndex neighborIndex in mapManager.GetNeighbors(currentHex.Coordinates))
            {
                Hex neighborHex = mapManager.GetHex(neighborIndex);
                if (neighborHex != null && neighborHex.Owner == currentHex.Owner && !connectedTerritory.Contains(neighborHex))
                {
                    toExplore.Enqueue(neighborHex);
                    connectedTerritory.Add(neighborHex);
                }
            }
        }
        
        foreach (Hex hex in province.Territory.ToList())
        {
            if (!connectedTerritory.Contains(hex))
            {
                hex.Owner = null;
                hex.UpdateColor(neutralColor);
                province.Territory.Remove(hex);
                
                if (hex.unit != null)
                {
                    DestroyUnitInHex(hex.unit);
                }
            }
        }
    }

    public Province GetPlayerProvince()
    {
        return provinces.Values.FirstOrDefault();
    }

    public Province GetProvinceById(int provinceId)
    {
        foreach (var province in provinces.Values)
        {
            if (province.OwnerId == provinceId)
            {
                return province;
            }
        }
        return null;
    }
    
    private void DestroyUnitInHex(Unit unit)
    {
        if (unit.Visual != null)
        {
            GameObject.Destroy(unit.Visual);
        }
        unit.CurrentHex.unit = null;

        unitManager.RemoveUnit(unit, unit.CurrentHex.Coordinates);
    }
}