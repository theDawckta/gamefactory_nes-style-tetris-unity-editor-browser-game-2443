using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class FinalScoreWidgetTests
{
    private GameObject _go;
    private FinalScoreWidget _widget;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("FinalScoreWidget");
        _widget = _go.AddComponent<FinalScoreWidget>();
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
        var region = new VisualElement();
        _widget.InitializeWithRegion(region);
        yield return null;
        var labels = region.Query<Label>().ToList();
        Assert.IsTrue(labels.Count >= 1);
        Assert.AreEqual("SCORE", labels[0].text);
    }

    [UnityTest]
    public IEnumerator FinalScoreWidget_InitialValueIsAllZeros()
    {
        var region = new VisualElement();
        _widget.InitializeWithRegion(region);
        yield return null;
        var labels = region.Query<Label>().ToList();
        Assert.AreEqual(2, labels.Count);
        Assert.AreEqual("0000000", labels[1].text);
    }

    [UnityTest]
    public IEnumerator FinalScoreWidget_SetScore_FormatsAs7DigitZeroPadded()
    {
        var region = new VisualElement();
        _widget.InitializeWithRegion(region);
        yield return null;
        _widget.SetScore(12345);
        var labels = region.Query<Label>().ToList();
        Assert.AreEqual("0012345", labels[1].text);
    }

    [UnityTest]
    public IEnumerator FinalScoreWidget_SetScore_ZeroDisplaysAllZeros()
    {
        var region = new VisualElement();
        _widget.InitializeWithRegion(region);
        yield return null;
        _widget.SetScore(0);
        var labels = region.Query<Label>().ToList();
        Assert.AreEqual("0000000", labels[1].text);
    }

    [UnityTest]
    public IEnumerator FinalScoreWidget_SetScore_BeforeInitializeDoesNotThrow()
    {
        yield return null;
        Assert.DoesNotThrow(() => _widget.SetScore(999));
    }
}
