using System;

namespace ROZ.DB.Runtime.Providers
{
    public interface IDataProvider
    {
        Type TableType { get; }
    }
}