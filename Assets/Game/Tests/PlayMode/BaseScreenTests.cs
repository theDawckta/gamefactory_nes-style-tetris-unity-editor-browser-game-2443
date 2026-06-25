using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class TestableBaseScreen : BaseScreen
{
    public UIDocument GetDocument() => Document;
    public bool OnShowCalled;
    public bool OnHideCalled;

    protected override void OnShow() { OnShowCalled = true; }
    protected override void OnHide() { OnHideCalled = true; }
}

public class BaseScreenTests
{
    private GameObject _go;
    private PanelSettings _panelSettings;

    [SetUp]
    public void SetUp()
    {
        _panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        _go = new GameObject();
        var doc = _go.AddComponent<UIDocument>();
        doc.panelSettings = _panelSettings;
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_go);
        Object.Destroy(_panelSettings);
    }

    [UnityTest]
    public IEnumerator BaseScreen_StartsHidden()
    {
        _go.AddComponent<TestableBaseScreen>();
        yield return null;
        Assert.IsFalse(_go.GetComponent<TestableBaseScreen>().IsVisible);
    }

    [UnityTest]
    public IEnumerator BaseScreen_ShowMakesVisible()
    {
        var screen = _go.AddComponent<TestableBaseScreen>();
        yield return null;
        screen.Show();
        Assert.IsTrue(screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator BaseScreen_HideMakesNotVisible()
    {
        var screen = _go.AddComponent<TestableBaseScreen>();
        yield return null;
        screen.Show();
        screen.Hide();
        Assert.IsFalse(screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator BaseScreen_DocumentPropertyReturnsUIDocument()
    {
        var doc = _go.GetComponent<UIDocument>();
        var screen = _go.AddComponent<TestableBaseScreen>();
        yield return null;
        Assert.AreEqual(doc, screen.GetDocument());
    }

    [UnityTest]
    public IEnumerator BaseScreen_OnShowCalledOnShow()
    {
        var screen = _go.AddComponent<TestableBaseScreen>();
        yield return null;
        screen.Show();
        Assert.IsTrue(screen.OnShowCalled);
    }

    [UnityTest]
    public IEnumerator BaseScreen_OnHideCalledOnHide()
    {
        var screen = _go.AddComponent<TestableBaseScreen>();
        yield return null;
        screen.Show();
        screen.OnHideCalled = false;
        screen.Hide();
        Assert.IsTrue(screen.OnHideCalled);
    }
}
