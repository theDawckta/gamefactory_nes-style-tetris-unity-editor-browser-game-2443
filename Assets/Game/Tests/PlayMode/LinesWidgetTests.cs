using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class LinesWidgetTests
{
    private GameObject _go;
    private LinesWidget _widget;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("LinesWidget");
        _widget = _go.AddComponent<LinesWidget>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null)
            Object.Destroy(_go);
    }

    [UnityTest]
    public IEnumerator LinesWidget_LinesTextDefaultsToZero()
    {
        yield return null;
        Assert.AreEqual("0", _widget.LinesText);
    }

    [UnityTest]
    public IEnumerator LinesWidget_SetRegion_CreatesHeaderAndValueLabels()
    {
        yield return null;
        var container = new VisualElement();
        _widget.SetRegion(container);

        Assert.AreEqual(2, container.childCount);

        var header = container.ElementAt(0) as Label;
        Assert.IsNotNull(header);
        Assert.AreEqual("LINES", header.text);

        var valueLabel = container.ElementAt(1) as Label;
        Assert.IsNotNull(valueLabel);
        Assert.AreEqual("0", valueLabel.text);
    }

    [UnityTest]
    public IEnumerator LinesWidget_Initialize_UpdatesLinesTextWhenStatsChange()
    {
        yield return null;
        var container = new VisualElement();
        _widget.SetRegion(container);

        var scoring = new ScoringSystem();
        _widget.Initialize(scoring);
        scoring.AddLines(4);

        Assert.AreEqual("4", _widget.LinesText);
    }

    [UnityTest]
    public IEnumerator LinesWidget_OnDisable_UnsubscribesFromScoringSystem()
    {
        yield return null;
        var container = new VisualElement();
        _widget.SetRegion(container);

        var scoring = new ScoringSystem();
        _widget.Initialize(scoring);

        _go.SetActive(false);
        scoring.AddLines(2);

        Assert.AreEqual("0", _widget.LinesText);
    }

    [UnityTest]
    public IEnumerator LinesWidget_HandlesNullGameScreenGracefully()
    {
        yield return null;
        Assert.IsNotNull(_widget);
        Assert.AreEqual("0", _widget.LinesText);
    }

    [UnityTest]
    public IEnumerator LinesWidget_HeaderLabelHasHudHeaderClass()
    {
        yield return null;
        var region = new VisualElement();
        _widget.SetRegion(region);
        var header = region.Q<Label>(null, "hud-header");
        Assert.IsNotNull(header);
        Assert.AreEqual("LINES", header.text);
    }

    [UnityTest]
    public IEnumerator LinesWidget_ValueLabelHasHudValueClass()
    {
        yield return null;
        var region = new VisualElement();
        _widget.SetRegion(region);
        var valueLabel = region.Q<Label>(null, "hud-value");
        Assert.IsNotNull(valueLabel);
    }
}
