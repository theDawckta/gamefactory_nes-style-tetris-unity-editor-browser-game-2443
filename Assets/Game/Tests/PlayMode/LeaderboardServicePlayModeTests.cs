using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class LeaderboardServicePlayModeTests
{
    private GameObject _serviceGo;

    [TearDown]
    public void TearDown()
    {
        if (_serviceGo != null)
            Object.Destroy(_serviceGo);
    }

    private LeaderboardService CreateService()
    {
        _serviceGo = new GameObject("LeaderboardService");
        return _serviceGo.AddComponent<LeaderboardService>();
    }

    [UnityTest]
    public IEnumerator Instance_IsNotNull_AfterAwake()
    {
        CreateService();
        yield return null;
        Assert.IsNotNull(LeaderboardService.Instance);
    }

    [UnityTest]
    public IEnumerator Instance_DuplicateIsDestroyed_WhenSecondAdded()
    {
        CreateService();
        yield return null;

        var secondGo = new GameObject("LeaderboardService2");
        secondGo.AddComponent<LeaderboardService>();
        yield return null;

        Assert.AreEqual(1, Object.FindObjectsByType<LeaderboardService>(FindObjectsSortMode.None).Length);
        Object.Destroy(secondGo);
    }

    [UnityTest]
    public IEnumerator IsTopFive_ReturnsTrue_WhenCacheIsEmpty()
    {
        var service = CreateService();
        yield return null;
        Assert.IsTrue(service.IsTopFive(0));
    }

    [UnityTest]
    public IEnumerator IsTopFive_ReturnsTrue_WhenPlaceholderEntriesExist()
    {
        var service = CreateService();
        yield return null;

        service.SetCachedScores(new LeaderboardEntry[]
        {
            new LeaderboardEntry { Initials = "AAA", Score = 5000 },
            new LeaderboardEntry { Initials = "BBB", Score = 3000 },
            new LeaderboardEntry { Initials = "---", Score = 0 },
            new LeaderboardEntry { Initials = "---", Score = 0 },
            new LeaderboardEntry { Initials = "---", Score = 0 }
        });

        Assert.IsTrue(service.IsTopFive(100));
    }

    [UnityTest]
    public IEnumerator IsTopFive_ReturnsTrue_WhenScoreBeatsLowest()
    {
        var service = CreateService();
        yield return null;

        service.SetCachedScores(new LeaderboardEntry[]
        {
            new LeaderboardEntry { Initials = "AAA", Score = 5000 },
            new LeaderboardEntry { Initials = "BBB", Score = 4000 },
            new LeaderboardEntry { Initials = "CCC", Score = 3000 },
            new LeaderboardEntry { Initials = "DDD", Score = 2000 },
            new LeaderboardEntry { Initials = "EEE", Score = 1000 }
        });

        Assert.IsTrue(service.IsTopFive(1500));
    }

    [UnityTest]
    public IEnumerator IsTopFive_ReturnsFalse_WhenScoreDoesNotBeatLowest()
    {
        var service = CreateService();
        yield return null;

        service.SetCachedScores(new LeaderboardEntry[]
        {
            new LeaderboardEntry { Initials = "AAA", Score = 5000 },
            new LeaderboardEntry { Initials = "BBB", Score = 4000 },
            new LeaderboardEntry { Initials = "CCC", Score = 3000 },
            new LeaderboardEntry { Initials = "DDD", Score = 2000 },
            new LeaderboardEntry { Initials = "EEE", Score = 1000 }
        });

        Assert.IsFalse(service.IsTopFive(1000));
    }

    [UnityTest]
    public IEnumerator FetchScores_CallsOnComplete_EvenOnNetworkFailure()
    {
        var service = CreateService();
        yield return null;

        bool called = false;
        service.FetchScores(entries => { called = true; });

        float elapsed = 0f;
        while (!called && elapsed < 10f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        Assert.IsTrue(called, "onComplete was not called within 10 seconds");
    }
}
