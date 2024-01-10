using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HexMap", menuName = "Maps/HexMap", order = 1)]
public class HexMapData : ScriptableObject
{
    [SerializeField] private List<SerializableCubeIndex> keys = new List<SerializableCubeIndex>();
    [SerializeField] private List<SerializableCellConfig> values = new List<SerializableCellConfig>();
    
    public List<SerializableCubeIndex> Keys { get { return keys; } }
    public List<SerializableCellConfig> Values { get { return values; } }
    
    public void AddCellConfig(CubeIndex index, CellConfig config)
    {
        SerializableCubeIndex serializableIndex = new SerializableCubeIndex(index.x, index.y, index.z);
        SerializableCellConfig serializableConfig = new SerializableCellConfig(config.Type);

        keys.Add(serializableIndex);
        values.Add(serializableConfig);
    }

    public bool TryGetCellConfig(CubeIndex index, out CellConfig config)
    {
        SerializableCubeIndex serializableIndex = new SerializableCubeIndex(index.x, index.y, index.z);
        int indexPosition = keys.IndexOf(serializableIndex);
        
        if (indexPosition != -1)
        {
            config = new CellConfig { Type = values[indexPosition].Type };
            return true;
        }

        config = new CellConfig();
        return false;
    }
    
    public IEnumerable<KeyValuePair<CubeIndex, CellConfig>> GetMapData()
    {
        for (int i = 0; i < keys.Count; i++)
        {
            yield return new KeyValuePair<CubeIndex, CellConfig>(
                new CubeIndex(keys[i].x, keys[i].y, keys[i].z),
                new CellConfig { Type = values[i].Type });
        }
    }

}