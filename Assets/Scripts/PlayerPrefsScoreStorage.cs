using UnityEngine;

public class PlayerPrefsScoreStorage : IScoreStorage
{
    private const string Key = "best_time_left";

    public float LoadBestTime() => PlayerPrefs.GetFloat(Key, 0f);

    public void SaveBestTime(float time)
    {
        PlayerPrefs.SetFloat(Key, time);
        PlayerPrefs.Save();
    }
}