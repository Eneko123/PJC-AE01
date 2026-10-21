using UnityEngine;

// Muestra FPS y frame time frente al objetivo de la Fase 1 (60 FPS / 16,67 ms).
// Se crea solo (no hay que ponerlo en la escena) y únicamente en editor y builds de desarrollo.
public class DiagnosticsOverlay : MonoBehaviour
{
    private const float Interval = 0.5f;

    private float accumulated;
    private int frames;
    private string label = "";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Create()
    {
        if (!Debug.isDebugBuild) return;   // build normal: no existe
        var go = new GameObject("DiagnosticsOverlay");
        DontDestroyOnLoad(go);
        go.AddComponent<DiagnosticsOverlay>();
    }

    private void Update()
    {
        accumulated += Time.unscaledDeltaTime;   // unscaled: sigue midiendo con el juego en pausa
        frames++;
        if (accumulated < Interval) return;

        float ms = accumulated / frames * 1000f;
        label = $"{1000f / ms:0} FPS | {ms:0.00} ms (objetivo: 60 FPS / 16,67 ms)";
        accumulated = 0f;
        frames = 0;
    }

    private void OnGUI()
    {
        if (label.Length == 0) return;
        GUI.Label(new Rect(10f, Screen.height - 30f, 700f, 24f), label);
    }
}