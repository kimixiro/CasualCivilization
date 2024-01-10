using System;

[Serializable]
public struct SerializableCellConfig
{
    public HexType Type;

    public SerializableCellConfig(HexType type)
    {
        this.Type = type;
    }
}