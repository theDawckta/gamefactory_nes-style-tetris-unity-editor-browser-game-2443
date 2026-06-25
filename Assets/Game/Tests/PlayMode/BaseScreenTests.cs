using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class BaseScreenTests
{
    private class TestableBaseScreen : BaseScreen
    {
        public UIDocument ExposedDocument => Document;
        public bool OnShowCalled;
        public bool OnHideCalled;

        protected override void OnShow() { OnShowCalled = true; }
        protected override void OnHide() { OnHideCalled = true; }
    }

    private GameObject _go;
    private TestableBaseScreen _screen;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("TestScreen");
        var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        var uiDoc = _go.AddComponent<UIDocument>();
        uiDoc.panelSettings = panelSettings;
        _screen = _go.AddComponent<TestableBaseScreen>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null)
            Object.Destroy(_go);
    }

    [UnityTest]
    public IEnumerator BaseScreen_StartsHidden()
    {
        yield return null;
        Assert.IsFalse(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator BaseScreen_ShowMakesVisible()
    {
        yield return null;
        _screen.Show();
        Assert.IsTrue(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator BaseScreen_HideMakesNotVisible()
    {
        yield return null;
        _screen.Show();
        _screen.Hide();
        Assert.IsFalse(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator BaseScreen_DocumentPropertyReturnsUIDocument()
    {
        yield return null;
        Assert.IsNotNull(_screen.ExposedDocument);
        Assert.AreEqual(_go.GetComponent<UIDocument>(), _screen.ExposedDocument);
    }

    [UnityTest]
    public IEnumerator BaseScreen_OnShowCalledOnShow()
    {
        yield return null;
        _screen.OnShowCalled = false;
        _screen.Show();
        Assert.IsTrue(_screen.OnShowCalled);
    }

    [UnityTest]
    public IEnumerator BaseScreen_OnHideCalledOnHide()
    {
        yield return null;
        _screen.Show();
        _screen.OnHideCalled = false;
        _screen.Hide();
        Assert.IsTrue(_screen.OnHideCalled);
    }
}
