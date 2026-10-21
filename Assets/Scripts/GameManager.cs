using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Menu, Playing, Paused, Finished }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public const string DefaultPauseMessage = "Menú de pausa";

    [SerializeField] private float roundTime = 60f;
    [SerializeField] private bool startInMenu = true;
    [SerializeField] private int targetFrameRate = 60;   // Fase 1: 60 FPS = 16,67 ms. Escalable a 30.

    // Sobrevive a la recarga de escena: "Reiniciar" entra directo a jugar; "Volver al menú" no.
    private static bool skipMenuOnLoad;

    private IScoreStorage storage;   // depende del contrato, no de una implementación concreta

    public GameState State { get; private set; }
    public float TimeLeft { get; private set; }
    public int Score { get; private set; }
    public int TotalBalls { get; private set; }
    public float BestTime { get; private set; }
    public bool Won { get; private set; }
    public string PauseMessage { get; private set; } = DefaultPauseMessage;
    public bool IsPlaying => State == GameState.Playing;

    public event Action<GameState> StateChanged;
    public event Action ScoreChanged;

    // Lo llama Bootstrap (ejecuta antes que este Awake).
    public void SetStorage(IScoreStorage s) => storage = s;

    private void Awake()
    {
        Instance = this;

        Application.targetFrameRate = targetFrameRate;

        if (storage == null)
        {
            DevLog.Log("GameManager: no se inyectó IScoreStorage; se usa almacenamiento en memoria.");
            storage = new InMemoryScoreStorage();
        }

        TotalBalls = FindObjectsByType<Ball>(FindObjectsSortMode.None).Length;
        TimeLeft = roundTime;
        BestTime = storage.LoadBestTime();

        bool menu = startInMenu && !skipMenuOnLoad;
        skipMenuOnLoad = false;
        State = menu ? GameState.Menu : GameState.Playing;
        Time.timeScale = menu ? 0f : 1f;

        LockCursor();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        if (State != GameState.Playing) return;
        TimeLeft -= Time.deltaTime;
        if (TimeLeft <= 0f) { TimeLeft = 0f; Finish(false); }
    }

    // perdida de foco / suspension 

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) LockCursor();
        else PauseIfPlaying("Pausa: la aplicación perdió el foco");
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) PauseIfPlaying("Pausa: la aplicación fue suspendida");
    }

    private void PauseIfPlaying(string message)
    {
        if (State == GameState.Playing) SetPaused(true, message);
    }

    // Pausa con mensaje (p. ej. mando desconectado). Si ya estaba en pausa, solo actualiza el mensaje.
    public void PauseWithMessage(string message)
    {
        if (State == GameState.Playing) SetPaused(true, message);
        else SetPauseMessage(message);
    }

    // Cambia el texto de la pausa sin reanudar (p. ej. mando reconectado).
    public void SetPauseMessage(string message)
    {
        if (State != GameState.Paused) return;
        PauseMessage = message;
        StateChanged?.Invoke(State);
    }

    // Flujo de juego 

    public void StartGame()
    {
        if (State != GameState.Menu) return;
        State = GameState.Playing;
        Time.timeScale = 1f;
        StateChanged?.Invoke(State);
    }

    // Recarga la escena: tiempo, puntuacion y bolas vuelven a su estado inicial.
    public void ReturnMenu()
    {
        skipMenuOnLoad = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Restart()
    {
        skipMenuOnLoad = true;   // entra directamente a jugar
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void AddScore()
    {
        if (State != GameState.Playing) return;
        Score++;
        ScoreChanged?.Invoke();
        if (Score >= TotalBalls) Finish(true);
    }

    public void SetPaused(bool paused, string message = DefaultPauseMessage)
    {
        if (State == GameState.Finished || State == GameState.Menu) return;
        PauseMessage = paused ? message : DefaultPauseMessage;
        State = paused ? GameState.Paused : GameState.Playing;
        Time.timeScale = paused ? 0f : 1f;
        StateChanged?.Invoke(State);
    }

    public void TogglePause() => SetPaused(State == GameState.Playing);

    private void Finish(bool won)
    {
        Won = won;
        State = GameState.Finished;
        if (won && TimeLeft > BestTime) { BestTime = TimeLeft; storage.SaveBestTime(BestTime); }
        Time.timeScale = 0f;
        StateChanged?.Invoke(State);
    }

    private static void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}