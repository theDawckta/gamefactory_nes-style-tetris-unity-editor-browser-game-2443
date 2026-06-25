using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class LeaderboardWidgetTests
{
    private GameObject _go;
    private LeaderboardWidget _widget;
    private VisualElement _region;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("LeaderboardWidget");
        _widget = _go.AddComponent<LeaderboardWidget>();
        _region = new VisualElement();
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null)
            Object.Destroy(_go);
    }

    [UnityTest]
    public IEnumerator LeaderboardWidget_SetLoadingShowsLoadingLabel()
    {
        yield return null;
        _widget.InitializeWithRegion(_region);
        _widget.SetLoading();
        var loadingLabel = _region.Q<Label>(null, "leaderboard-loading");
        Assert.IsNotNull(loadingLabel);
        Assert.AreEqual(DisplayStyle.Flex, loadingLabel.style.display.value);
    }

    [UnityTest]
    public IEnumerator LeaderboardWidget_SetLoadingHidesRows()
    {
        yield return null;
        _widget.InitializeWithRegion(_region);
        _widget.SetLoading();
        var rows = _region.Query(null, "leaderboard-row").ToList();
        Assert.AreEqual(5, rows.Count);
        foreach (var row in rows)
            Assert.AreEqual(DisplayStyle.None, row.style.display.value);
    }

    [UnityTest]
    public IEnumerator LeaderboardWidget_SetScoresWithFiveEntriesShowsAllRows()
    {
        yield return null;
        _widget.InitializeWithRegion(_region);
        var entries = new LeaderboardEntry[]
        {
            new LeaderboardEntry { Initials = "AAA", Score = 9000 },
            new LeaderboardEntry { Initials = "BBB", Score = 8000 },
            new LeaderboardEntry { Initials = "CCC", Score = 7000 },
            new LeaderboardEntry { Initials = "DDD", Score = 6000 },
            new LeaderboardEntry { Initials = "EEE", Score = 5000 },
        };
        _widget.SetScores(entries);
        var rows = _region.Query(null, "leaderboard-row").ToList();
        Assert.AreEqual(5, rows.Count);
        foreach (var row in rows)
            Assert.AreEqual(DisplayStyle.Flex, row.style.display.value);
    }

    [UnityTest]
    public IEnumerator LeaderboardWidget_SetScoresPopulatesCorrectData()
    {
        yield return null;
        _widget.InitializeWithRegion(_region);
        var entries = new LeaderboardEntry[]
        {
            new LeaderboardEntry { Initials = "ZZZ", Score = 1234567 },
            new LeaderboardEntry { Initials = "AAA", Score = 42 },
            new LeaderboardEntry { Initials = "---", Score = 0 },
            new LeaderboardEntry { Initials = "XYZ", Score = 999 },
            new LeaderboardEntry { Initials = "QRS", Score = 100 },
        };
        _widget.SetScores(entries);

        var rows = _region.Query(null, "leaderboard-row").ToList();
        var rank1Initials = rows[0].Q<Label>(null, "leaderboard-initials");
        var rank1Score = rows[0].Q<Label>(null, "leaderboard-score");
        Assert.AreEqual("ZZZ", rank1Initials.text);
        Assert.AreEqual("1234567", rank1Score.text);

        var rank2Rank = rows[1].Q<Label>(null, "leaderboard-rank");
        Assert.AreEqual("2", rank2Rank.text);
    }

    [UnityTest]
    public IEnumerator LeaderboardWidget_SetScoresWithFewerThanFiveFillsPlaceholders()
    {
        yield return null;
        _widget.InitializeWithRegion(_region);
        var entries = new LeaderboardEntry[]
        {
            new LeaderboardEntry { Initials = "AAA", Score = 500 },
        };
        _widget.SetScores(entries);

        var rows = _region.Query(null, "leaderboard-row").ToList();
        var rank2Initials = rows[1].Q<Label>(null, "leaderboard-initials");
        var rank2Score = rows[1].Q<Label>(null, "leaderboard-score");
        Assert.AreEqual("---", rank2Initials.text);
        Assert.AreEqual("0", rank2Score.text);
    }

    [UnityTest]
    public IEnumerator LeaderboardWidget_SetScoresHidesLoadingLabel()
    {
        yield return null;
        _widget.InitializeWithRegion(_region);
        _widget.SetScores(null);
        var loadingLabel = _region.Q<Label>(null, "leaderboard-loading");
        Assert.IsNotNull(loadingLabel);
        Assert.AreEqual(DisplayStyle.None, loadingLabel.style.display.value);
    }

    [UnityTest]
    public IEnumerator LeaderboardWidget_HeaderLabelIsTopFive()
    {
        yield return null;
        _widget.InitializeWithRegion(_region);
        var header = _region.Q<Label>(null, "leaderboard-header");
        Assert.IsNotNull(header);
        Assert.AreEqual("TOP 5", header.text);
    }
}
