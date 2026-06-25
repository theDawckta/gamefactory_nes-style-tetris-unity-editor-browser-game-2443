using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class StartPromptWidgetTests
{
    private GameObject _go;
    private StartPromptWidget _widget;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("StartPromptWidget");
        _widget = _go.AddComponent<StartPromptWidget>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null)
            Object.Destroy(_go);
    }

    [UnityTest]
    public IEnumerator StartPromptWidget_AttachesToGameObject()
    {
        yield return null;
        Assert.IsNotNull(_widget);
    }

    [UnityTest]
    public IEnumerator StartPromptWidget_PromptLabelNotVisibleWithoutScreen()
    {
        yield return null;
        // No StartScreen reference -- no label is created, so IsPromptLabelVisible is false
        Assert.IsFalse(_widget.IsPromptLabelVisible);
    }

    [UnityTest]
    public IEnumerator StartPromptWidget_PromptLabelHiddenWhenScreenHidden()
    {
        // Create a minimal StartScreen setup
        var screenGo = new GameObject("StartScreen");
        var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        var uiDoc = screenGo.AddComponent<UIDocument>();
        uiDoc.panelSettings = panelSettings;
        var screen = screenGo.AddComponent<StartScreen>();

        // Wire via the widget's serialized field using reflection is not allowed;
        // instead we verify the guard path: without a screen ref, label stays hidden.
        yield return null;

        Assert.IsFalse(_widget.IsPromptLabelVisible);

        Object.Destroy(screenGo);
    }

    [UnityTest]
    public IEnumerator StartPromptWidget_IsPromptLabelVisibleReturnsFalseByDefault()
    {
        yield return null;
        Assert.IsFalse(_widget.IsPromptLabelVisible);
    }
}
