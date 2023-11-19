using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public GameObject Visual { get; set; }
    public Hex CurrentHex { get; set; }
    public bool HasMoved { get; set; }
    public int UpkeepCost { get; set; }
    public int Strength { get; set; }
    public int OwnerId { get; set; }
    
    public Unit(GameObject visual, int upkeepCost = 5, int strength = 0, int ownerId = 0)
    {
        Visual = visual;
        UpkeepCost = upkeepCost;
        HasMoved = false;
        Strength = strength;
        OwnerId = ownerId;
    }
}
