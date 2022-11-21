using System;
public class EmptyData : ICloneable
{
    public object Clone()
    {
        return MemberwiseClone();
    }
}
