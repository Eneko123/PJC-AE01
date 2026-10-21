using UnityEngine;

// Raíz de composición: único lugar donde se elige la implementación concreta de IScoreStorage.
// Para otra plataforma (consola) bastaría cambiar la línea marcada, sin tocar el gameplay.
// Se ejecuta antes que GameManager.Awake (orden -100).
[DefaultExecutionOrder(-100)]
public class Bootstrap : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private void Awake()
    {
        if (gameManager == null) gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("Bootstrap: no hay GameManager en la escena.");
            return;
        }

        gameManager.SetStorage(new PlayerPrefsScoreStorage());   // <- cambiar aquí por la implementación de la plataforma
    }
}