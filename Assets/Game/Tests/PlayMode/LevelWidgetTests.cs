using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class LevelWidgetTests
{
    private GameObject _widgetGo;
    private LevelWidget _widget;
    private ScoringSystem _scoringSystem;

    [SetUp]
    public void SetUp()
    {
        _scoringSystem = new ScoringSystem();
        _widgetGo = new GameObject("LevelWidget");
        _widgetGo.SetActive(false);
        _widget = _widgetGo.AddComponent<LevelWidget>();
        _widget.Initialize(null, _scoringSystem);
        _widgetGo.SetActive(true);
    }

    [TearDown]
    public void TearDown()
    {
        if (_widgetGo != null)
            Object.Destroy(_widgetGo);
    }

    [UnityTest]
    public IEnumerator LevelWidget_DefaultLevelTextIsOne()
    {
        yield return null;
        Assert.AreEqual("1", _widget.LevelText);
    }

    [UnityTest]
    public IEnumerator LevelWidget_LevelTextUpdatesOnStatsChanged()
    {
        yield return null;
        // 3x AddLines(4) = 12 total lines, ScoringSystem.Level becomes 1
        _scoringSystem.AddLines(4);
        _scoringSystem.AddLines(4);
        _scoringSystem.AddLines(4);
        Assert.AreEqual("2", _widget.LevelText);
    }

    [UnityTest]
    public IEnumerator LevelWidget_UnsubscribesOnDisable()
    {
        yield return null;
        _widgetGo.SetActive(false);
        _scoringSystem.AddLines(4);
        _scoringSystem.AddLines(4);
        _scoringSystem.AddLines(4);
        Assert.AreEqual("1", _widget.LevelText);
    }

    [UnityTest]
    public IEnumerator LevelWidget_ResubscribesOnReEnable()
    {
        yield return null;
        _widgetGo.SetActive(false);
        _widgetGo.SetActive(true);
        _scoringSystem.AddLines(4);
        _scoringSystem.AddLines(4);
        _scoringSystem.AddLines(4);
        Assert.AreEqual("2", _widget.LevelText);
    }
}
