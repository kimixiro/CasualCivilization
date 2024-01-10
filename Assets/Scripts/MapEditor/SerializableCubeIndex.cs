using System;

[Serializable]
public struct SerializableCubeIndex
{
    public int x, y, z;

    public SerializableCubeIndex(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}