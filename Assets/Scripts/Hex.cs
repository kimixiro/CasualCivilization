using UnityEngine;

public class Hex
{
    public GameObject Visual { get; private set; }
    public CubeIndex Coordinates { get; private set; }
    public Province Owner { get; set; }
    public Unit unit { get; set; }
    public bool HasBuilding { get; set; }
    public Building Building { get; set; }

    public Hex(GameObject visual, CubeIndex coordinates)
    {
        Visual = visual;
        Coordinates = coordinates;
    }

    public void UpdateColor(Color newColor)
    {
        if (Visual != null)
        {
            Renderer renderer = Visual.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = newColor;
            }
        }
    }
    
    public void DestroyBuilding()
    {
        if (Building != null && Building.Visual != null)
        {
            GameObject.Destroy(Building.Visual);
            Building = null;
            HasBuilding = false;
        }
    }
}