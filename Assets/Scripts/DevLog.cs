using System.Diagnostics;

// Log de diagnóstico
public static class DevLog
{
    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void Log(string message) => UnityEngine.Debug.Log(message);
}