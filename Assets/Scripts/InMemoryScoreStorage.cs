// Implementacion sin dependencias de plataforma
public class InMemoryScoreStorage : IScoreStorage
{
    private float bestTime;

    public float LoadBestTime() => bestTime;

    public void SaveBestTime(float time) => bestTime = time;
}