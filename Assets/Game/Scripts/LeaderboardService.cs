using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("GameTests.PlayMode")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("GameTests.EditMode")]

[Serializable]
public struct LeaderboardEntry
{
    public string Initials;
    public int Score;
}

public class LeaderboardService : MonoBehaviour
{
    private string _baseUrl = ""; // populated by ConfigService at runtime; empty = no requests

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

    private IEnumerator Start()
    {
        yield return ConfigService.Instance.EnsureLoaded();
        var url = ConfigService.Instance.Get("leaderboardUrl");
        if (!string.IsNullOrEmpty(url)) _baseUrl = url;
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

        int lowest = _cachedScores[_cachedScores.Length - 1].Score;
        return score > lowest;
    }

    internal void SetCachedScores(LeaderboardEntry[] scores)
    {
        _cachedScores = scores;
    }

    private IEnumerator FetchScoresCoroutine(Action<LeaderboardEntry[]> onComplete)
    {
        if (string.IsNullOrEmpty(_baseUrl)) { onComplete?.Invoke(_cachedScores); yield break; }
        using (var request = UnityWebRequest.Get(_baseUrl + "/leaderboard"))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onComplete?.Invoke(_cachedScores);
                yield break;
            }

            var entries = ParseEntries(request.downloadHandler.text);
            if (entries != null)
                _cachedScores = entries;

            onComplete?.Invoke(_cachedScores);
        }
    }

    private IEnumerator SubmitScoreCoroutine(string initials, int score, Action<LeaderboardEntry[]> onComplete)
    {
        if (string.IsNullOrEmpty(_baseUrl)) { onComplete?.Invoke(_cachedScores); yield break; }
        var body = "{\"initials\":\"" + initials + "\",\"score\":" + score + "}";
        var bodyBytes = System.Text.Encoding.UTF8.GetBytes(body);

        using (var request = new UnityWebRequest(_baseUrl + "/leaderboard", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onComplete?.Invoke(_cachedScores);
                yield break;
            }

            var entries = ParseEntries(request.downloadHandler.text);
            if (entries != null)
                _cachedScores = entries;

            onComplete?.Invoke(_cachedScores);
        }
    }

    private static LeaderboardEntry[] ParseEntries(string json)
    {
        var wrapper = JsonUtility.FromJson<LeaderboardResponseWrapper>(json);
        if (wrapper?.scores == null)
            return null;

        var result = new LeaderboardEntry[wrapper.scores.Length];
        for (int i = 0; i < wrapper.scores.Length; i++)
        {
            result[i] = new LeaderboardEntry
            {
                Initials = wrapper.scores[i].initials,
                Score = wrapper.scores[i].score
            };
        }
        return result;
    }

    [Serializable]
    private class LeaderboardResponseWrapper
    {
        public LeaderboardEntryJson[] scores;
    }

    [Serializable]
    private class LeaderboardEntryJson
    {
        public string initials;
        public int score;
    }
}
