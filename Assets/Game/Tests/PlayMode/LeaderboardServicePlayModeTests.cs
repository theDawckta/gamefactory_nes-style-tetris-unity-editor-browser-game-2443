using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class LeaderboardServicePlayModeTests
{
    private GameObject _go;

    [SetUp]
    public void SetUp()
    {
        // Clear any lingering singleton from a previous test
        var existing = Object.FindFirstObjectByType<LeaderboardService>();
        if (existing != null)
            Object.Destroy(existing.gameObject);
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null)
            Object.Destroy(_go);

        var leftover = Object.FindFirstObjectByType<LeaderboardService>();
        if (leftover != null)
            Object.Destroy(leftover.gameObject);
    }

    [UnityTest]
    public IEnumerator LeaderboardService_InstanceNonNullAfterAwake()
    {
        _go = new GameObject("LeaderboardService");
        _go.AddComponent<LeaderboardService>();
        yield return null;
        Assert.IsNotNull(LeaderboardService.Instance);
    }

    [UnityTest]
    public IEnumerator LeaderboardService_DestroysDuplicateInstance()
    {
        _go = new GameObject("LeaderboardService");
        _go.AddComponent<LeaderboardService>();
        yield return null;

        var duplicate = new GameObject("LeaderboardServiceDuplicate");
        duplicate.AddComponent<LeaderboardService>();
        yield return null;

        Assert.IsTrue(duplicate == null || !duplicate.activeInHierarchy || duplicate.GetComponent<LeaderboardService>() == null);
        Object.Destroy(duplicate);
    }

    [UnityTest]
    public IEnumerator IsTopFive_ReturnsTrueWhenCacheEmpty()
    {
        _go = new GameObject("LeaderboardService");
        var service = _go.AddComponent<LeaderboardService>();
        yield return null;

        Assert.IsTrue(service.IsTopFive(0));
    }

    [UnityTest]
    public IEnumerator IsTopFive_ReturnsTrueWhenPlaceholderExists()
    {
        _go = new GameObject("LeaderboardService");
        var service = _go.AddComponent<LeaderboardService>();
        yield return null;

        service.SetCachedScores(new LeaderboardEntry[]
        {
            new LeaderboardEntry { Initials = "AAA", Score = 500 },
            new LeaderboardEntry { Initials = "---", Score = 0 },
            new LeaderboardEntry { Initials = "---", Score = 0 },
        });

        Assert.IsTrue(service.IsTopFive(1));
    }

    [UnityTest]
    public IEnumerator IsTopFive_ReturnsTrueWhenScoreBeatsLowest()
    {
        _go = new GameObject("LeaderboardService");
        var service = _go.AddComponent<LeaderboardService>();
        yield return null;

        service.SetCachedScores(new LeaderboardEntry[]
        {
            new LeaderboardEntry { Initials = "AAA", Score = 1000 },
            new LeaderboardEntry { Initials = "BBB", Score = 800 },
            new LeaderboardEntry { Initials = "CCC", Score = 600 },
            new LeaderboardEntry { Initials = "DDD", Score = 400 },
            new LeaderboardEntry { Initials = "EEE", Score = 200 },
        });

        Assert.IsTrue(service.IsTopFive(201));
    }

    [UnityTest]
    public IEnumerator IsTopFive_ReturnsFalseWhenScoreDoesNotBeatLowest()
    {
        _go = new GameObject("LeaderboardService");
        var service = _go.AddComponent<LeaderboardService>();
        yield return null;

        service.SetCachedScores(new LeaderboardEntry[]
        {
            new LeaderboardEntry { Initials = "AAA", Score = 1000 },
            new LeaderboardEntry { Initials = "BBB", Score = 800 },
            new LeaderboardEntry { Initials = "CCC", Score = 600 },
            new LeaderboardEntry { Initials = "DDD", Score = 400 },
            new LeaderboardEntry { Initials = "EEE", Score = 200 },
        });

        Assert.IsFalse(service.IsTopFive(200));
    }

    [UnityTest]
    public IEnumerator FetchScores_CallsOnCompleteOnNetworkFailure()
    {
        _go = new GameObject("LeaderboardService");
        var service = _go.AddComponent<LeaderboardService>();
        yield return null;

        bool callbackInvoked = false;
        service.FetchScores(entries => { callbackInvoked = true; });

        float elapsed = 0f;
        while (!callbackInvoked && elapsed < 10f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        Assert.IsTrue(callbackInvoked, "FetchScores did not invoke onComplete within 10 seconds");
    }
}
