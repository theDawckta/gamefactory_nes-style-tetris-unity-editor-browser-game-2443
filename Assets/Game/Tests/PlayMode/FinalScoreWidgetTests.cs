using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class FinalScoreWidgetTests
{
    private GameObject _go;
    private FinalScoreWidget _widget;
    private VisualElement _region;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("FinalScoreWidget");
        _widget = _go.AddComponent<FinalScoreWidget>();
        _region = new VisualElement();
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null)
            Object.Destroy(_go);
    }

    [UnityTest]
    public IEnumerator FinalScoreWidget_HeaderLabelIsSCORE()
    {
        yield return null;
        _widget.InitializeWithRegion(_region);
        var header = _region.Q<Label>(null, "final-score-header");
        Assert.IsNotNull(header);
        Assert.AreEqual("SCORE", header.text);
    }

    [UnityTest]
    public IEnumerator FinalScoreWidget_InitialValueIsAllZeros()
    {
        yield return null;
        _widget.InitializeWithRegion(_region);
        var label = _region.Q<Label>(null, "final-score-value");
        Assert.IsNotNull(label);
        Assert.AreEqual("0000000", label.text);
    }

    [UnityTest]
    public IEnumerator FinalScoreWidget_SetScore_FormatsAs7DigitZeroPadded()
    {
        yield return null;
        _widget.InitializeWithRegion(_region);
        _widget.SetScore(12345);
        var label = _region.Q<Label>(null, "final-score-value");
        Assert.AreEqual("0012345", label.text);
    }

    [UnityTest]
    public IEnumerator FinalScoreWidget_SetScore_ZeroDisplaysAllZeros()
    {
        yield return null;
        _widget.InitializeWithRegion(_region);
        _widget.SetScore(0);
        var label = _region.Q<Label>(null, "final-score-value");
        Assert.AreEqual("0000000", label.text);
    }

    [UnityTest]
    public IEnumerator FinalScoreWidget_SetScore_BeforeInitializeDoesNotThrow()
    {
        yield return null;
        Assert.DoesNotThrow(() => _widget.SetScore(999));
    }
}
