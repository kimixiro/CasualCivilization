#if UNITY_EDITOR
#define LOGGING
#define LOGGING_DEBUG
#define LOGGING_INFO
#define LOGGING_WARNING
#define LOGGING_ERROR
#endif

using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace ROZ.Logging
{
    public static class Logger
    {
        private const string LogFormatString  = "[{0}] [{1}.{2}] {3}";
        
        private const string DebugLogPrefix   = "DEBUG";
        private const string InfoLogPrefix    = "INFO";
        private const string WarningLogPrefix = "WARNING";
        private const string ErrorLogPrefix   = "ERROR";
        
        [Conditional("LOGGING")]
        [Conditional("LOGGING_INFO")]
        public static void LogInfo(string context, string group, string message, UnityEngine.Object contextObject = null)
        {
            if (contextObject != null)
            {
                Debug.Log(string.Format(LogFormatString, InfoLogPrefix, context, group, message), contextObject);
            }
            else
            {
                Debug.Log(string.Format(LogFormatString, InfoLogPrefix, context, group, message));
            }
        }
        
        [Conditional("LOGGING")]
        [Conditional("LOGGING_WARNING")]
        public static void LogWarning(string context, string group, string message, UnityEngine.Object contextObject = null)
        {
            if (contextObject != null)
            {
                Debug.LogWarning(string.Format(LogFormatString, WarningLogPrefix, context, group, message), contextObject);
            }
            else
            {
                Debug.LogWarning(string.Format(LogFormatString, WarningLogPrefix, context, group, message));
            }
        }
        
        [Conditional("LOGGING")]
        [Conditional("LOGGING_ERROR")]
        public static void LogError(string context, string group, string message, UnityEngine.Object contextObject = null)
        {
            if (contextObject != null)
            {
                Debug.LogError(string.Format(LogFormatString, ErrorLogPrefix, context, group, message), contextObject);
            }
            else
            {
                Debug.LogError(string.Format(LogFormatString, ErrorLogPrefix, context, group, message));
            }
        }
        
        [Conditional("LOGGING")]
        [Conditional("LOGGING_DEBUG")]
        public static void LogDebug(string context, string group, string message, UnityEngine.Object contextObject = null)
        {
            if (contextObject != null)
            {
                Debug.Log(string.Format(LogFormatString, DebugLogPrefix, context, group, message), contextObject);
            }
            else
            {
                Debug.Log(string.Format(LogFormatString, DebugLogPrefix, context, group, message));
            }
        }
        
        [Conditional("LOGGING")]
        [Conditional("LOGGING_ERROR")]
        public static void LogException(Exception exception, UnityEngine.Object contextObject = null)
        {
            if (contextObject != null)
            {
                Debug.LogException(exception, contextObject);
            }
            else
            {
                Debug.LogException(exception);
            }
        }
    }
}