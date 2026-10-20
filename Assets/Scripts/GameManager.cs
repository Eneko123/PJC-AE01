using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Menu, Playing, Paused, Finished }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private float roundTime = 60f;
    [SerializeField] private bool startInMenu = true;

    private IScoreStorage storage;   // depende del contrato, no de PlayerPrefs

    public GameState State { get; private set; }
    public float TimeLeft { get; private set; }
    public int Score { get; private set; }
    public int TotalBalls { get; private set; }
    public float BestTime { get; private set; }
    public bool Won { get; private set; }
    public bool IsPlaying => State == GameState.Playing;

    public event Action<GameState> StateChanged;
    public event Action ScoreChanged;

    public void SetStorage(IScoreStorage s) => storage = s;

    private void Awake()
    {
        Instance = this;
        storage ??= new PlayerPrefsScoreStorage();
        TotalBalls = FindObjectsByType<Ball>(FindObjectsSortMode.None).Length;
        TimeLeft = roundTime;
        BestTime = storage.LoadBestTime();

        bool menu = startInMenu;
        State = menu ? GameState.Menu : GameState.Playing;
        Time.timeScale = menu ? 0f : 1f;

        Cursor.visible = false;                       
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (State != GameState.Playing) return;
        TimeLeft -= Time.deltaTime;
        if (TimeLeft <= 0f) { TimeLeft = 0f; Finish(false); }
    }

    public void StartGame()
    {
        if (State != GameState.Menu) return;
        State = GameState.Playing;
        Time.timeScale = 1f;
        StateChanged?.Invoke(State);
    }

    public void ReturnMenu()
    {
        State = GameState.Menu;
        Time.timeScale = 0;
        StateChanged?.Invoke(State);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && State == GameState.Playing)
            SetPaused(true, "Juego en pausa: la aplicación perdió el foco");
    }

    public void AddScore()
    {
        if (State != GameState.Playing) return;
        Score++;
        ScoreChanged?.Invoke();
        if (Score >= TotalBalls) Finish(true);
    }

    public void SetPaused(bool paused, string message = "")
    {
        if (State == GameState.Finished || State == GameState.Menu) return;
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

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}