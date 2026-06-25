using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class GameOverScreenTests
{
    private GameObject _go;
    private GameOverScreen _screen;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("GameOverScreen");
        var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        var uiDoc = _go.AddComponent<UIDocument>();
        uiDoc.panelSettings = panelSettings;
        _screen = _go.AddComponent<GameOverScreen>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null)
            Object.Destroy(_go);
    }

    [UnityTest]
    public IEnumerator GameOverScreen_HasUIDocumentComponent()
    {
        yield return null;
        Assert.IsNotNull(_go.GetComponent<UIDocument>());
    }

    [UnityTest]
    public IEnumerator GameOverScreen_StartsHidden()
    {
        yield return null;
        Assert.IsFalse(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator GameOverScreen_ShowSetsIsVisibleTrue()
    {
        yield return null;
        _screen.Show();
        Assert.IsTrue(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator GameOverScreen_HideSetsIsVisibleFalse()
    {
        yield return null;
        _screen.Show();
        _screen.Hide();
        Assert.IsFalse(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator GameOverScreen_RegionPropertiesNullWithoutUxml()
    {
        yield return null;
        Assert.IsNull(_screen.InitialsRegion);
        Assert.IsNull(_screen.ReturnPromptRegion);
    }

    [UnityTest]
    public IEnumerator GameOverScreen_ShowWithScore_SetsIsVisible()
    {
        yield return null;
        _screen.ShowWithScore(5000);
        Assert.IsTrue(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator GameOverScreen_ShowReturnPrompt_DoesNotThrowWithoutUxml()
    {
        yield return null;
        Assert.DoesNotThrow(() => _screen.ShowReturnPrompt());
    }
}
