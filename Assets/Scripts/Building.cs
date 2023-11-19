using UnityEngine;

public class Building
{
    public GameObject Visual { get; private set; }
    public int Strength { get; private set; }
    public BuildingType Type { get; private set; }
    public int OwnerId { get; set; }

    public Building(GameObject visual, int strength, BuildingType type, int ownerId)
    {
        Visual = visual;
        Strength = strength;
        Type = type;
        OwnerId = ownerId;
    }
}


public enum BuildingType
{
    Castle,
    Tower
}