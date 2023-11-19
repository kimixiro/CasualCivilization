using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {
    public GameObject hexPrefab;
    public int mapRadius;
    public Material hexMaterial;
    public bool addColliders = true;
    public float hexRadius;

    private Dictionary<CubeIndex, Hex> hexMap = new Dictionary<CubeIndex, Hex>();

    public void Initialize() {
        GenerateHexagonalGrid();
    }

    void GenerateHexagonalGrid() {
        for (int q = -mapRadius; q <= mapRadius; q++) {
            int r1 = Mathf.Max(-mapRadius, -q - mapRadius);
            int r2 = Mathf.Min(mapRadius, -q + mapRadius);
            for (int r = r1; r <= r2; r++) {
                int s = -q - r;
                CreateHex(q, r, s);
            }
        }
    }

    void CreateHex(int q, int r, int s) {
        CubeIndex index = new CubeIndex(q, r, s);
        Vector3 position = HexCoordToPosition(index);

        GameObject hexGO = Instantiate(hexPrefab, position, Quaternion.identity, transform);
        hexGO.name = $"Hex_{q}_{r}_{s}";
        hexGO.GetComponent<Renderer>().material = hexMaterial;

        if (addColliders) {
            hexGO.AddComponent<MeshCollider>();
        }

        Hex hex = new Hex(hexGO, index);
        hexMap.Add(index, hex);
    }

    Vector3 HexCoordToPosition(CubeIndex cubeIndex) {
        float horizontalSpacing = Mathf.Sqrt(3) * hexRadius;
        float verticalSpacing = 1.5f * hexRadius;

        float x = horizontalSpacing * (cubeIndex.x + cubeIndex.z / 2f);
        float z = verticalSpacing * cubeIndex.z;

        return new Vector3(x, 0, z);
    }
    
    public Hex GetHex(CubeIndex index) {
        Hex hex;
        if (hexMap.TryGetValue(index, out hex)) {
            return hex;
        }
        return null;
    }
    
    public Vector3 GetHexPosition(CubeIndex index) {
        return HexCoordToPosition(index);
    }
    
    public Dictionary<CubeIndex, Hex> GetAllHexes() {
        return new Dictionary<CubeIndex, Hex>(hexMap);
    }

    public Dictionary<CubeIndex, Hex> GetHexesInRange(CubeIndex center, int radius) {
        Dictionary<CubeIndex, Hex> hexesInRange = new Dictionary<CubeIndex, Hex>();

        if (hexMap.ContainsKey(center)) {
            Queue<CubeIndex> queue = new Queue<CubeIndex>();
            HashSet<CubeIndex> visited = new HashSet<CubeIndex>();
            queue.Enqueue(center);
            visited.Add(center);

            while (queue.Count > 0) {
                CubeIndex current = queue.Dequeue();
                hexesInRange[current] = hexMap[current];

                foreach (CubeIndex neighbor in GetNeighbors(current)) {
                    if (!visited.Contains(neighbor) && !queue.Contains(neighbor)
                                                    && Vector3.Distance(GetHexPosition(center), GetHexPosition(neighbor)) <= radius) {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }
        }

        return hexesInRange;
    }
    
    public IEnumerable<CubeIndex> GetNeighbors(CubeIndex index) {
        List<CubeIndex> neighbors = new List<CubeIndex>();

        Vector3Int[] directions = {
            new Vector3Int(1, -1, 0), new Vector3Int(1, 0, -1), new Vector3Int(0, 1, -1),
            new Vector3Int(-1, 1, 0), new Vector3Int(-1, 0, 1), new Vector3Int(0, -1, 1)
        };

        foreach (var direction in directions) {
            CubeIndex neighborIndex = new CubeIndex(
                index.x + direction.x,
                index.y + direction.y,
                index.z + direction.z
            );

            if (hexMap.ContainsKey(neighborIndex)) {
                neighbors.Add(neighborIndex);
            }
        }

        return neighbors;
    }
}