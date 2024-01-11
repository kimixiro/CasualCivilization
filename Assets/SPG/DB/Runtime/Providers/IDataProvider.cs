using System;

namespace SPG.DB.Runtime.Providers
{
    public interface IDataProvider
    {
        Type TableType { get; }
    }
}