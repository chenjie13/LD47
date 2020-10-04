using UnityEngine;

namespace CGDK.Core
{
    public static class LogUtil
    {
        public static bool EnableInfo = true;
        public static bool EnableLog = true;
        public static bool EnableWarning = true;
        public static bool EnableError = true;
        
        public static void Info(string content)
        {
            if (EnableInfo) Debug.Log(content);
        }
        
        public static void Log(string content)
        {
            if (EnableLog) Debug.Log(content);
        }

        public static void Warning(string content)
        {
            if (EnableWarning) Debug.LogWarning(content);
        }

        public static void Error(string content)
        {
            if (EnableError) Debug.LogError(content);
        }
    }
}