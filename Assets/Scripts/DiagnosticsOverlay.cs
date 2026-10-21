using UnityEngine;

// Muestra FPS y frame time frente al objetivo (60 FPS / 16,67 ms).
public class DiagnosticsOverlay : MonoBehaviour
{
    private const float Interval = 0.5f;

    private float lastReport;
    private int frames;
    private string label = "";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Create()
    {
        if (!Debug.isDebugBuild) return;
        var go = new GameObject("DiagnosticsOverlay");
        DontDestroyOnLoad(go);
        go.AddComponent<DiagnosticsOverlay>();
    }

    private void Start()
    {
        lastReport = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        frames++;

        float now = Time.realtimeSinceStartup;
        float elapsed = now - lastReport;
        if (elapsed < Interval) return;

        float ms = elapsed / frames * 1000f;
        lastReport = now;
        frames = 0;
        if (float.IsNaN(ms) || ms <= 0f) return;   // nunca mostramos valores no validos

        label = $"{1000f / ms:0} FPS | {ms:0.00} ms (objetivo: 60 FPS / 16,67 ms)";
    }

    private void OnGUI()
    {
        if (label.Length == 0) return;
        GUI.Label(new Rect(10f, Screen.height - 30f, 700f, 24f), label);
    }
}