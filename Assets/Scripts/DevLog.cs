using System.Diagnostics;

// Log de diagnóstico. En la build normal el compilador elimina las llamadas,
// incluida la evaluación de sus argumentos (p. ej. strings interpolados): coste cero.
public static class DevLog
{
    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void Log(string message) => UnityEngine.Debug.Log(message);
}