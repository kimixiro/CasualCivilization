public struct CubeIndex {
    public int x, y, z;

    public CubeIndex(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static CubeIndex operator +(CubeIndex a, CubeIndex b) {
        return new CubeIndex(a.x + b.x, a.y + b.y, a.z + b.z);
    }
    
    public static bool operator ==(CubeIndex a, CubeIndex b)
    {
        return a.x == b.x && a.y == b.y && a.z == b.z;
    }

    public static bool operator !=(CubeIndex a, CubeIndex b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        if (obj is CubeIndex)
        {
            CubeIndex c = (CubeIndex)obj;
            return x == c.x && y == c.y && z == c.z;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return x ^ y ^ z; 
    }
}