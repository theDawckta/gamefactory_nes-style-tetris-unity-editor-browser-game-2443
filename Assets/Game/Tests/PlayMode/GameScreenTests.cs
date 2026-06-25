using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class GameScreenTests
{
    private GameObject _go;
    private GameScreen _screen;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("GameScreen");
        var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        var uiDoc = _go.AddComponent<UIDocument>();
        uiDoc.panelSettings = panelSettings;
        _screen = _go.AddComponent<GameScreen>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null)
            Object.Destroy(_go);
    }

    [UnityTest]
    public IEnumerator GameScreen_HasUIDocumentComponent()
    {
        yield return null;
        Assert.IsNotNull(_go.GetComponent<UIDocument>());
    }

    [UnityTest]
    public IEnumerator GameScreen_StartsHidden()
    {
        yield return null;
        Assert.IsFalse(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator GameScreen_ShowSetsIsVisibleTrue()
    {
        yield return null;
        _screen.Show();
        Assert.IsTrue(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator GameScreen_HideSetsIsVisibleFalse()
    {
        yield return null;
        _screen.Show();
        _screen.Hide();
        Assert.IsFalse(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator GameScreen_RegionPropertiesNullWithoutUxml()
    {
        yield return null;
        Assert.IsNull(_screen.ScoreRegion);
        Assert.IsNull(_screen.LevelRegion);
        Assert.IsNull(_screen.LinesRegion);
        Assert.IsNull(_screen.NextPieceRegion);
    }
}
