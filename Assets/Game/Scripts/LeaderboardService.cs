using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("GameTests.EditMode")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("GameTests.PlayMode")]

[Serializable]
public struct LeaderboardEntry
{
    public string Initials;
    public int Score;
}

public class LeaderboardService : MonoBehaviour
{
    private const string BaseUrl = "http://localhost:3000";

    public static LeaderboardService Instance { get; private set; }

    private LeaderboardEntry[] _cachedScores;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void FetchScores(Action<LeaderboardEntry[]> onComplete)
    {
        StartCoroutine(FetchScoresCoroutine(onComplete));
    }

    public void SubmitScore(string initials, int score, Action<LeaderboardEntry[]> onComplete)
    {
        StartCoroutine(SubmitScoreCoroutine(initials, score, onComplete));
    }

    public bool IsTopFive(int score)
    {
        if (_cachedScores == null || _cachedScores.Length == 0)
            return true;

        foreach (var entry in _cachedScores)
        {
            if (entry.Initials == "---")
                return true;
        }

        int lowestScore = int.MaxValue;
        foreach (var entry in _cachedScores)
        {
            if (entry.Score < lowestScore)
                lowestScore = entry.Score;
        }

        return score > lowestScore;
    }

    internal void SetCachedScores(LeaderboardEntry[] entries)
    {
        _cachedScores = entries;
    }

    private IEnumerator FetchScoresCoroutine(Action<LeaderboardEntry[]> onComplete)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "/leaderboard"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                _cachedScores = ParseScores(request.downloadHandler.text);
                onComplete?.Invoke(_cachedScores);
            }
            else
            {
                onComplete?.Invoke(_cachedScores);
            }
        }
    }

    private IEnumerator SubmitScoreCoroutine(string initials, int score, Action<LeaderboardEntry[]> onComplete)
    {
        string json = JsonUtility.ToJson(new ScorePayload { initials = initials, score = score });
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(BaseUrl + "/leaderboard", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                _cachedScores = ParseScores(request.downloadHandler.text);
                onComplete?.Invoke(_cachedScores);
            }
            else
            {
                onComplete?.Invoke(_cachedScores);
            }
        }
    }

    private LeaderboardEntry[] ParseScores(string json)
    {
        LeaderboardWrapper wrapper = JsonUtility.FromJson<LeaderboardWrapper>("{\"entries\":" + json + "}");
        if (wrapper?.entries == null)
            return new LeaderboardEntry[0];

        var result = new LeaderboardEntry[wrapper.entries.Length];
        for (int i = 0; i < wrapper.entries.Length; i++)
        {
            result[i] = new LeaderboardEntry
            {
                Initials = wrapper.entries[i].initials,
                Score = wrapper.entries[i].score
            };
        }
        return result;
    }

    [Serializable]
    private class ScorePayload
    {
        public string initials;
        public int score;
    }

    [Serializable]
    private struct LeaderboardEntryJson
    {
        public string initials;
        public int score;
    }

    [Serializable]
    private class LeaderboardWrapper
    {
        public LeaderboardEntryJson[] entries;
    }
}
