using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class StartScreenTests
{
    private GameObject _go;
    private StartScreen _screen;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("StartScreen");
        var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        var uiDoc = _go.AddComponent<UIDocument>();
        uiDoc.panelSettings = panelSettings;
        _screen = _go.AddComponent<StartScreen>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null)
            Object.Destroy(_go);
        var leftoverAll = Object.FindObjectsByType<LeaderboardService>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var leftover = leftoverAll.Length > 0 ? leftoverAll[0] : null;
        if (leftover != null)
            Object.Destroy(leftover.gameObject);
    }

    [UnityTest]
    public IEnumerator StartScreen_HasUIDocumentComponent()
    {
        yield return null;
        Assert.IsNotNull(_go.GetComponent<UIDocument>());
    }

    [UnityTest]
    public IEnumerator StartScreen_StartsHidden()
    {
        yield return null;
        Assert.IsFalse(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator StartScreen_ShowSetsIsVisibleTrue()
    {
        yield return null;
        _screen.Show();
        Assert.IsTrue(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator StartScreen_HideSetsIsVisibleFalse()
    {
        yield return null;
        _screen.Show();
        _screen.Hide();
        Assert.IsFalse(_screen.IsVisible);
    }

    [UnityTest]
    public IEnumerator StartScreen_RegionPropertiesNullWithoutUxml()
    {
        yield return null;
        Assert.IsNull(_screen.LeaderboardRegion);
        Assert.IsNull(_screen.PromptRegion);
    }

    [UnityTest]
    public IEnumerator StartScreen_OnStartPressedCanBeSubscribed()
    {
        yield return null;
        bool fired = false;
        _screen.OnStartPressed += () => fired = true;
        _screen.OnStartPressed -= () => fired = true;
        Assert.IsFalse(fired);
    }

    [UnityTest]
    public IEnumerator LeaderboardOffline_DoesNotBlockReturnToStart()
    {
        // Clear any lingering singleton from a previous test
        var existingAll = Object.FindObjectsByType<LeaderboardService>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var existing = existingAll.Length > 0 ? existingAll[0] : null;
        if (existing != null)
            Object.Destroy(existing.gameObject);
        yield return null;

        // Create a LeaderboardService and wait for Awake to set Instance
        var serviceGo = new GameObject("LeaderboardService");
        serviceGo.AddComponent<LeaderboardService>();
        yield return null;

        Assert.IsNotNull(LeaderboardService.Instance);

        // Deactivate it -- Instance remains non-null but isActiveAndEnabled is false
        serviceGo.SetActive(false);
        Assert.IsFalse(LeaderboardService.Instance.isActiveAndEnabled);

        // Show() must not throw a coroutine error when the service is inactive
        _screen.Show();
        yield return null;

        Assert.IsTrue(_screen.IsVisible);
    }
}
