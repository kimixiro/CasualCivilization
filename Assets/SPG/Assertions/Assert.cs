using System;
using System.Diagnostics;
using SPG.Logging;

namespace SPG.Assertions
{
    public static class Assert
    {
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNull<T>
        (
            T value, 
            UnityEngine.Object contextObject
        )
            where T : class
        {
            try
            {
                UnityEngine.Assertions.Assert.IsNull(value);
            }
            catch (Exception e)
            {
                Logger.LogException(e, contextObject);

                throw;
            }
        }
        
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsNotNull<T>
        (
            T value, 
            UnityEngine.Object contextObject
        )
            where T : class
        {
            try
            {
                UnityEngine.Assertions.Assert.IsNotNull(value);
            }
            catch (Exception e)
            {
                Logger.LogException(e, contextObject);

                throw;
            }
        }
    }
}