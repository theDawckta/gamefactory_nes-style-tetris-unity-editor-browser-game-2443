using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ReturnPromptWidgetTests
{
    private GameObject _go;
    private ReturnPromptWidget _widget;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("ReturnPromptWidget");
        _widget = _go.AddComponent<ReturnPromptWidget>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null)
            Object.Destroy(_go);
    }

    [UnityTest]
    public IEnumerator ReturnPromptWidget_AttachesToGameObject()
    {
        yield return null;
        Assert.IsNotNull(_widget);
    }

    [UnityTest]
    public IEnumerator ReturnPromptWidget_OnReturnPressedCanBeSubscribed()
    {
        yield return null;
        bool fired = false;
        _widget.OnReturnPressed += () => { fired = true; };
        Assert.IsFalse(fired);
    }

    [UnityTest]
    public IEnumerator ReturnPromptWidget_OnReturnPressedFiresWhenSimulated()
    {
        yield return null;
        bool fired = false;
        _widget.OnReturnPressed += () => { fired = true; };
        _widget.SimulateReturnPressed();
        Assert.IsTrue(fired);
    }

    [UnityTest]
    public IEnumerator ReturnPromptWidget_OnReturnPressedNotFiredWithoutScreen()
    {
        yield return null;
        bool fired = false;
        _widget.OnReturnPressed += () => { fired = true; };
        // No GameOverScreen set, so Update() returns early -- no fire
        yield return null;
        Assert.IsFalse(fired);
    }

    [UnityTest]
    public IEnumerator ReturnPromptWidget_PromptLabelNotVisibleWithoutScreen()
    {
        yield return null;
        // Without a screen/region reference, no label is created
        Assert.IsFalse(_widget.IsPromptLabelVisible);
    }
}
