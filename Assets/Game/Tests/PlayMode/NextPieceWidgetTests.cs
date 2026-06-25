using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NextPieceWidgetTests
{
    private GameObject _widgetGo;
    private NextPieceWidget _widget;
    private GameObject _controllerGo;
    private GameplayController _gc;
    private GameObject _playfieldGo;
    private PlayfieldController _playfield;
    private GameObject _pieceGo;
    private PieceController _piece;

    [SetUp]
    public void SetUp()
    {
        _playfieldGo = new GameObject("Playfield");
        _playfield = _playfieldGo.AddComponent<PlayfieldController>();

        _pieceGo = new GameObject("PieceController");
        _piece = _pieceGo.AddComponent<PieceController>();
        _piece.Playfield = _playfield;

        _controllerGo = new GameObject("GameplayController");
        _gc = _controllerGo.AddComponent<GameplayController>();
        _gc.PlayfieldController = _playfield;
        _gc.PieceController = _piece;

        _widgetGo = new GameObject("NextPieceWidget");
        _widgetGo.SetActive(false);
        _widget = _widgetGo.AddComponent<NextPieceWidget>();
        _widget.GameplayController = _gc;
        _widgetGo.SetActive(true);
    }

    [TearDown]
    public void TearDown()
    {
        if (_widgetGo != null) Object.Destroy(_widgetGo);
        if (_controllerGo != null) Object.Destroy(_controllerGo);
        if (_playfieldGo != null) Object.Destroy(_playfieldGo);
        if (_pieceGo != null) Object.Destroy(_pieceGo);
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_HasNextPieceWidgetComponent()
    {
        yield return null;
        Assert.IsNotNull(_widgetGo.GetComponent<NextPieceWidget>());
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_Start_Creates16PreviewCells()
    {
        yield return null;
        int count = 0;
        foreach (Transform child in _widgetGo.transform)
        {
            if (child.name.StartsWith("Preview_")) count++;
        }
        Assert.AreEqual(16, count);
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_AllCellsInvisibleBeforeGameStart()
    {
        yield return null;
        foreach (Transform child in _widgetGo.transform)
        {
            var sr = child.GetComponent<SpriteRenderer>();
            if (sr != null)
                Assert.AreEqual(0f, sr.color.a, 0.001f, $"{child.name} should be invisible before game starts");
        }
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_OnNextPieceChanged_ShowsExactly4OpaqueCells()
    {
        yield return null;
        _gc.StartGame();

        int opaque = 0;
        foreach (Transform child in _widgetGo.transform)
        {
            var sr = child.GetComponent<SpriteRenderer>();
            if (sr != null && sr.color.a > 0f) opaque++;
        }
        Assert.AreEqual(4, opaque, "Preview should show exactly 4 opaque cells for any tetromino");
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_OnNextPieceChanged_UnusedCellsAreInvisible()
    {
        yield return null;
        _gc.StartGame();

        int invisible = 0;
        foreach (Transform child in _widgetGo.transform)
        {
            var sr = child.GetComponent<SpriteRenderer>();
            if (sr != null && Mathf.Approximately(sr.color.a, 0f)) invisible++;
        }
        Assert.AreEqual(12, invisible, "12 of 16 preview cells should be invisible for any tetromino");
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_OnNextPieceChanged_AllOpaqueCellsMatchPieceColor()
    {
        yield return null;
        TetrominoData receivedPiece = null;
        _gc.OnNextPieceChanged += d => receivedPiece = d;
        _gc.StartGame();

        Assert.IsNotNull(receivedPiece, "OnNextPieceChanged should have fired");

        Color[] colorMap = new Color[]
        {
            new Color(0f, 0f, 0f, 0f),
            new Color(0f, 240f/255f, 240f/255f, 1f),
            new Color(240f/255f, 240f/255f, 0f, 1f),
            new Color(160f/255f, 0f, 240f/255f, 1f),
            new Color(0f, 240f/255f, 0f, 1f),
            new Color(240f/255f, 0f, 0f, 1f),
            new Color(0f, 0f, 240f/255f, 1f),
            new Color(240f/255f, 160f/255f, 0f, 1f),
        };
        Color expected = colorMap[receivedPiece.colorIndex];

        foreach (Transform child in _widgetGo.transform)
        {
            var sr = child.GetComponent<SpriteRenderer>();
            if (sr == null || !(sr.color.a > 0f)) continue;
            Assert.AreEqual(expected.r, sr.color.r, 0.01f, $"{child.name} red channel");
            Assert.AreEqual(expected.g, sr.color.g, 0.01f, $"{child.name} green channel");
            Assert.AreEqual(expected.b, sr.color.b, 0.01f, $"{child.name} blue channel");
        }
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_UnsubscribesOnDisable()
    {
        yield return null;
        // Disable widget before any StartGame so cells remain all-invisible
        _widgetGo.SetActive(false);

        // StartGame fires OnNextPieceChanged, but widget is unsubscribed
        _gc.StartGame();

        // Cells should still all be invisible (handler was not called)
        foreach (Transform child in _widgetGo.transform)
        {
            var sr = child.GetComponent<SpriteRenderer>();
            if (sr != null)
                Assert.AreEqual(0f, sr.color.a, 0.001f, $"{child.name} should remain invisible while widget is disabled");
        }
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_ResubscribesOnReEnable()
    {
        yield return null;
        _widgetGo.SetActive(false);
        _widgetGo.SetActive(true);
        yield return null;

        _gc.StartGame();

        int opaque = 0;
        foreach (Transform child in _widgetGo.transform)
        {
            var sr = child.GetComponent<SpriteRenderer>();
            if (sr != null && sr.color.a > 0f) opaque++;
        }
        Assert.AreEqual(4, opaque, "Widget should respond to events after re-enable");
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_NullGameScreen_LateUpdateDoesNotThrow()
    {
        // GameScreen is null (not wired) -- LateUpdate must not throw
        _widget.GameScreen = null;
        yield return null; // triggers LateUpdate
        // If we reach here, no exception was thrown
        Assert.IsNull(_widget.GameScreen);
    }

    [UnityTest]
    public IEnumerator NextPieceWidget_HasGameScreenField()
    {
        yield return null;
        // Verify the field exists and is accessible (set it to null and read it back)
        _widget.GameScreen = null;
        Assert.IsNull(_widget.GameScreen);
    }
}
