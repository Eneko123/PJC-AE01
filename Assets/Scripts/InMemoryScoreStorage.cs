// Implementación sin dependencias de plataforma: valor por defecto si nadie inyecta otra y útil para pruebas.
public class InMemoryScoreStorage : IScoreStorage
{
    private float bestTime;

    public float LoadBestTime() => bestTime;

    public void SaveBestTime(float time) => bestTime = time;
}