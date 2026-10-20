using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text versionText;

    [Header("Inicio")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    [Header("Pausa")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TMP_Text pauseMessageText;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button returnButton;

    [Header("Fin")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text endText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button returnEndButton;

    private int lastSeconds = -1;   // solo reescribimos el texto cuando cambia el segundo
    private Button focusButton;     // botón que debe tener el foco en el panel visible

    private void Start()
    {
        var gm = GameManager.Instance;
        gm.StateChanged += OnStateChanged;
        gm.ScoreChanged += RefreshScore;

        playButton.onClick.AddListener(gm.StartGame);
        quitButton.onClick.AddListener(gm.Quit);
        resumeButton.onClick.AddListener(() => gm.SetPaused(false));
        restartButton.onClick.AddListener(gm.Restart);
        returnButton.onClick.AddListener(gm.ReturnMenu);
        returnEndButton.onClick.AddListener(gm.ReturnMenu);

        versionText.text = $"{Application.productName} v{Application.version} " +
                           (Debug.isDebugBuild ? "[DEV]" : "[RELEASE]");
        RefreshScore();
        OnStateChanged(gm.State);   // estado inicial (normalmente Menu)
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.StateChanged -= OnStateChanged;
        GameManager.Instance.ScoreChanged -= RefreshScore;
    }

    private void Update()
    {
        if (focusButton != null && focusButton.gameObject.activeInHierarchy &&
            EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(focusButton.gameObject);
        }

        int s = Mathf.CeilToInt(GameManager.Instance.TimeLeft);
        if (s == lastSeconds) return;
        lastSeconds = s;
        timerText.SetText("{0}", s);
    }

    private void RefreshScore() =>
        scoreText.SetText("{0}/{1}", GameManager.Instance.Score, GameManager.Instance.TotalBalls);

    private void OnStateChanged(GameState state)
    {
        var gm = GameManager.Instance;
        startPanel.SetActive(state == GameState.Menu);
        pausePanel.SetActive(state == GameState.Paused);
        endPanel.SetActive(state == GameState.Finished);

        switch (state)
        {
            case GameState.Menu:
                Focus(playButton);
                break;
            case GameState.Paused:
                pauseMessageText.text = "Menu de pausa";
                Focus(resumeButton);
                break;
            case GameState.Finished:
                endText.text = (gm.Won ? "¡Victoria!" : "Se acabó el tiempo") +
                               $"\nRécord: {gm.BestTime:0.0}s restantes";
                Focus(restartButton);
                break;
            default:
                focusButton = null;
                break;
        }
    }

    // Foco inicial para navegar con el D-pad/stick sin ratón
    private void Focus(Button b)
    {
        focusButton = b;
        EventSystem.current.SetSelectedGameObject(null);   // fuerza a re-resaltar aunque ya estuviera seleccionado
        EventSystem.current.SetSelectedGameObject(b.gameObject);
    }
}