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

    private GameManager gm;          // cacheado: evita acceder a Instance en cada Update
    private int lastSeconds = -1;    // solo reescribimos el texto cuando cambia el segundo
    private Button focusButton;      // botón que debe tener el foco en el panel visible

    private void Start()
    {
        gm = GameManager.Instance;
        gm.StateChanged += OnStateChanged;
        gm.ScoreChanged += RefreshScore;

        playButton.onClick.AddListener(gm.StartGame);
        quitButton.onClick.AddListener(gm.Quit);
        resumeButton.onClick.AddListener(() => gm.SetPaused(false));
        restartButton.onClick.AddListener(gm.Restart);
        returnButton.onClick.AddListener(gm.ReturnMenu);
        returnEndButton.onClick.AddListener(gm.ReturnMenu);

        // En consola las aplicaciones no se cierran desde el propio juego.
        quitButton.gameObject.SetActive(!Application.isConsolePlatform);

        // Identificación de la build: nombre, versión, configuración y autor (Company Name).
        versionText.text = $"{Application.productName} v{Application.version} " +
                           (Debug.isDebugBuild ? "[DEV]" : "[RELEASE]") +
                           $" - {Application.companyName}";
        RefreshScore();
        OnStateChanged(gm.State);   // estado inicial (normalmente Menu)
    }

    private void OnDestroy()
    {
        if (gm == null) return;
        gm.StateChanged -= OnStateChanged;
        gm.ScoreChanged -= RefreshScore;
    }

    private void Update()
    {
        if (focusButton != null && focusButton.gameObject.activeInHierarchy &&
            EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(focusButton.gameObject);
        }

        int s = Mathf.CeilToInt(gm.TimeLeft);
        if (s == lastSeconds) return;
        lastSeconds = s;
        timerText.SetText("{0}", s);
    }

    private void RefreshScore() =>
        scoreText.SetText("{0}/{1}", gm.Score, gm.TotalBalls);

    private void OnStateChanged(GameState state)
    {
        startPanel.SetActive(state == GameState.Menu);
        pausePanel.SetActive(state == GameState.Paused);
        endPanel.SetActive(state == GameState.Finished);

        switch (state)
        {
            case GameState.Menu:
                Focus(playButton);
                break;
            case GameState.Paused:
                pauseMessageText.text = gm.PauseMessage;   // "Menú de pausa", mando desconectado, foco perdido...
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