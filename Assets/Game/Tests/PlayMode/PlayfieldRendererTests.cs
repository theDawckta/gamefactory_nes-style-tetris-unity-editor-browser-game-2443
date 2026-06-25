using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayfieldRendererTests
{
    private GameObject _rendererGo;
    private GameObject _playfieldGo;
    private PlayfieldRenderer _renderer;
    private PlayfieldController _playfield;

    [SetUp]
    public void SetUp()
    {
        _playfieldGo = new GameObject("Playfield");
        _playfield = _playfieldGo.AddComponent<PlayfieldController>();

        _rendererGo = new GameObject("PlayfieldRenderer");
        _renderer = _rendererGo.AddComponent<PlayfieldRenderer>();
        _renderer.PlayfieldController = _playfield;
    }

    [TearDown]
    public void TearDown()
    {
        if (_rendererGo != null) Object.Destroy(_rendererGo);
        if (_playfieldGo != null) Object.Destroy(_playfieldGo);
    }

    [UnityTest]
    public IEnumerator PlayfieldRenderer_HasPlayfieldRendererComponent()
    {
        yield return null;
        Assert.IsNotNull(_rendererGo.GetComponent<PlayfieldRenderer>());
    }

    [UnityTest]
    public IEnumerator PlayfieldRenderer_Start_Creates200GridCellRenderers()
    {
        yield return null;
        int count = 0;
        foreach (Transform child in _rendererGo.transform)
        {
            if (child.name.StartsWith("Cell_"))
                count++;
        }
        Assert.AreEqual(200, count);
    }

    [UnityTest]
    public IEnumerator PlayfieldRenderer_Start_Creates4ActivePieceRenderers()
    {
        yield return null;
        int count = 0;
        foreach (Transform child in _rendererGo.transform)
        {
            if (child.name.StartsWith("ActivePiece_"))
                count++;
        }
        Assert.AreEqual(4, count);
    }

    [UnityTest]
    public IEnumerator PlayfieldRenderer_Start_Creates4BorderQuads()
    {
        yield return null;
        int count = 0;
        foreach (Transform child in _rendererGo.transform)
        {
            if (child.name.StartsWith("Border_"))
                count++;
        }
        Assert.AreEqual(4, count);
    }

    [UnityTest]
    public IEnumerator PlayfieldRenderer_EmptyCells_AreInvisible()
    {
        yield return null;
        yield return null; // LateUpdate also runs
        foreach (Transform child in _rendererGo.transform)
        {
            if (!child.name.StartsWith("Cell_")) continue;
            var sr = child.GetComponent<SpriteRenderer>();
            Assert.AreEqual(0f, sr.color.a, 0.001f, $"{child.name} should be invisible");
        }
    }

    [UnityTest]
    public IEnumerator PlayfieldRenderer_LockedCell_ShowsCorrectColor()
    {
        yield return null; // Start

        _playfield.SetCell(0, 0, 1); // cyan at row=0, col=0

        yield return null; // LateUpdate

        Transform cell00 = _rendererGo.transform.Find("Cell_0_0");
        Assert.IsNotNull(cell00);
        var sr = cell00.GetComponent<SpriteRenderer>();
        Assert.AreEqual(1f, sr.color.a, 0.001f, "Locked cell should be opaque");
        Assert.AreEqual(0f, sr.color.r, 0.01f, "Cyan R");
        Assert.AreEqual(240f / 255f, sr.color.g, 0.01f, "Cyan G");
        Assert.AreEqual(240f / 255f, sr.color.b, 0.01f, "Cyan B");
    }

    [UnityTest]
    public IEnumerator PlayfieldRenderer_ActivePiece_IsVisibleWhenInVisibleRows()
    {
        var pieceGo = new GameObject("PieceController");
        var piece = pieceGo.AddComponent<PieceController>();
        piece.Playfield = _playfield;
        _renderer.PieceController = piece;

        // Cells extend downward so they land in visible rows (0-19) from pivot at (4,20)
        var data = ScriptableObject.CreateInstance<TetrominoData>();
        data.colorIndex = 6; // blue
        data.rotationStates = new RotationState[]
        {
            new RotationState
            {
                cells = new Vector2Int[]
                {
                    new Vector2Int(0, -1),
                    new Vector2Int(1, -1),
                    new Vector2Int(0, -2),
                    new Vector2Int(1, -2),
                }
            }
        };
        piece.SpawnPiece(data); // pivot = (4, 20); cells land at rows 19 and 18

        yield return null; // Start + LateUpdate

        int visibleCount = 0;
        foreach (Transform child in _rendererGo.transform)
        {
            if (!child.name.StartsWith("ActivePiece_")) continue;
            var sr = child.GetComponent<SpriteRenderer>();
            if (sr.color.a > 0f) visibleCount++;
        }
        Assert.AreEqual(4, visibleCount, "All 4 active piece cells should be visible");

        Object.Destroy(pieceGo);
        Object.Destroy(data);
    }
}
