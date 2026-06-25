using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class ScoreWidgetTests
{
    private GameObject _go;
    private ScoreWidget _widget;
    private VisualElement _scoreRegion;
    private ScoringSystem _scoringSystem;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("ScoreWidget");
        _widget = _go.AddComponent<ScoreWidget>();
        _scoreRegion = new VisualElement();
        _scoringSystem = new ScoringSystem();
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null)
            Object.Destroy(_go);
    }

    [UnityTest]
    public IEnumerator ScoreWidget_InitialValueIsZeroPadded()
    {
        yield return null;
        _widget.InitializeWithRegion(_scoreRegion);
        var label = _scoreRegion.Q<Label>(null, "score-value");
        Assert.IsNotNull(label);
        Assert.AreEqual("0000000", label.text);
    }

    [UnityTest]
    public IEnumerator ScoreWidget_HeaderLabelIsScore()
    {
        yield return null;
        _widget.InitializeWithRegion(_scoreRegion);
        var header = _scoreRegion.Q<Label>(null, "score-header");
        Assert.IsNotNull(header);
        Assert.AreEqual("SCORE", header.text);
    }

    [UnityTest]
    public IEnumerator ScoreWidget_UpdatesOnStatsChanged()
    {
        yield return null;
        _widget.InitializeWithRegion(_scoreRegion);
        _widget.SetScoringSystem(_scoringSystem);
        _scoringSystem.AddLines(4); // 1200 * (0+1) = 1200
        var label = _scoreRegion.Q<Label>(null, "score-value");
        Assert.AreEqual("0001200", label.text);
    }

    [UnityTest]
    public IEnumerator ScoreWidget_ScoreFormattedAs7DigitZeroPadded()
    {
        yield return null;
        _widget.InitializeWithRegion(_scoreRegion);
        _widget.SetScoringSystem(_scoringSystem);
        _scoringSystem.AddLines(1); // 40 * 1 = 40
        var label = _scoreRegion.Q<Label>(null, "score-value");
        Assert.AreEqual("0000040", label.text);
    }

    [UnityTest]
    public IEnumerator ScoreWidget_UnsubscribesOnDisable()
    {
        yield return null;
        _widget.InitializeWithRegion(_scoreRegion);
        _widget.SetScoringSystem(_scoringSystem);
        _go.SetActive(false);
        _scoringSystem.AddLines(4);
        var label = _scoreRegion.Q<Label>(null, "score-value");
        Assert.AreEqual("0000000", label.text);
    }
}
