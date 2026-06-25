using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PieceControllerTests
{
    private GameObject _pieceGo;
    private GameObject _playfieldGo;
    private PieceController _piece;
    private PlayfieldController _playfield;

    [SetUp]
    public void SetUp()
    {
        _playfieldGo = new GameObject("Playfield");
        _playfield = _playfieldGo.AddComponent<PlayfieldController>();

        _pieceGo = new GameObject("PieceController");
        _piece = _pieceGo.AddComponent<PieceController>();
        _piece.Playfield = _playfield;
    }

    [TearDown]
    public void TearDown()
    {
        if (_pieceGo != null) Object.Destroy(_pieceGo);
        if (_playfieldGo != null) Object.Destroy(_playfieldGo);
    }

    private TetrominoData CreateSingleCellData()
    {
        var data = ScriptableObject.CreateInstance<TetrominoData>();
        data.colorIndex = 1;
        data.rotationStates = new RotationState[]
        {
            new RotationState { cells = new Vector2Int[] { Vector2Int.zero } }
        };
        return data;
    }

    [UnityTest]
    public IEnumerator PieceController_HasPieceControllerComponent()
    {
        yield return null;
        Assert.IsNotNull(_pieceGo.GetComponent<PieceController>());
    }

    [UnityTest]
    public IEnumerator PieceController_SpawnPiece_SetsPivotToColumn4Row20()
    {
        yield return null;
        var data = CreateSingleCellData();
        _piece.SpawnPiece(data);
        Assert.AreEqual(new Vector2Int(4, 20), _piece.CurrentPivot);
    }

    [UnityTest]
    public IEnumerator PieceController_SpawnPiece_SetsRotationToZero()
    {
        yield return null;
        var data = CreateSingleCellData();
        _piece.SpawnPiece(data);
        Assert.AreEqual(0, _piece.CurrentRotation);
    }

    [UnityTest]
    public IEnumerator PieceController_IsLocked_FalseAfterSpawn()
    {
        yield return null;
        var data = CreateSingleCellData();
        _piece.SpawnPiece(data);
        Assert.IsFalse(_piece.IsLocked);
    }

    [UnityTest]
    public IEnumerator PieceController_Tick_GravityCausesPieceToFall()
    {
        yield return null;
        var data = CreateSingleCellData();
        _piece.SpawnPiece(data);
        Assert.AreEqual(20, _piece.CurrentPivot.y);

        // Level 0 gravity interval is 0.800s; Tick with 0.85s triggers one fall step.
        _piece.Tick(0.85f);
        Assert.AreEqual(19, _piece.CurrentPivot.y);
    }

    [UnityTest]
    public IEnumerator PieceController_Tick_LockDelayFiresOnPieceLocked()
    {
        yield return null;
        var data = CreateSingleCellData();

        // Place a locked cell at row 19 col 4 so the piece spawning at (4,20) is grounded immediately.
        _playfield.SetCell(19, 4, 1);
        _piece.SpawnPiece(data);

        bool locked = false;
        _piece.OnPieceLocked += () => locked = true;

        // Level 0 gravity is 0.800s so no gravity step occurs at 0.55s.
        // Lock delay is 0.500s so it expires and fires OnPieceLocked.
        _piece.Tick(0.55f);

        Assert.IsTrue(locked);
        Assert.IsTrue(_piece.IsLocked);
    }
}
