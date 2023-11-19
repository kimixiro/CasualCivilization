using System.Collections.Generic;
using UnityEngine;

public class Province
{
    public int OwnerId { get; set; }
    public CubeIndex Center { get; set; }
    public List<Hex> Territory { get; set; }
    public Color Color { get; set; }
    
    public int Treasury { get; set; }
    public int GoldPerTurn { get; set; }


    public Province(CubeIndex center, Color color)
    {
        Center = center;
        Color = color;
        Territory = new List<Hex>();
    }
}
