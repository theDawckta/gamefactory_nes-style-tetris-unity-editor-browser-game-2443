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
    private TetrominoData _data;

    [SetUp]
    public void SetUp()
    {
        _playfieldGo = new GameObject("Playfield");
        _playfield = _playfieldGo.AddComponent<PlayfieldController>();

        _pieceGo = new GameObject("PieceController");
        _piece = _pieceGo.AddComponent<PieceController>();
        _piece.Playfield = _playfield;

        _data = ScriptableObject.CreateInstance<TetrominoData>();
        _data.colorIndex = 1;
        _data.rotationStates = new RotationState[]
        {
            new RotationState { cells = new Vector2Int[] { Vector2Int.zero } }
        };
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_pieceGo);
        Object.Destroy(_playfieldGo);
        Object.Destroy(_data);
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
        _piece.SpawnPiece(_data);
        Assert.AreEqual(new Vector2Int(4, 20), _piece.CurrentPivot);
    }

    [UnityTest]
    public IEnumerator PieceController_SpawnPiece_SetsRotationToZero()
    {
        yield return null;
        _piece.SpawnPiece(_data);
        Assert.AreEqual(0, _piece.CurrentRotation);
    }

    [UnityTest]
    public IEnumerator PieceController_IsLocked_FalseAfterSpawn()
    {
        yield return null;
        _piece.SpawnPiece(_data);
        Assert.IsFalse(_piece.IsLocked);
    }

    [UnityTest]
    public IEnumerator PieceController_Tick_GravityCausesPieceToFall()
    {
        yield return null;
        _piece.SpawnPiece(_data);
        // Level 0 gravity interval: 0.800s. A 0.9s tick fires gravity once.
        _piece.Tick(0.9f);
        Assert.AreEqual(new Vector2Int(4, 19), _piece.CurrentPivot);
    }

    [UnityTest]
    public IEnumerator PieceController_Tick_LockDelayFiresOnPieceLocked()
    {
        yield return null;
        _piece.SpawnPiece(_data);
        // Block the cell directly below spawn so the piece is immediately grounded.
        _playfield.SetCell(19, 4, 1);

        bool lockFired = false;
        _piece.OnPieceLocked += () => lockFired = true;

        // Tick enough to trigger gravity (>0.8s) and accumulate full lock delay (0.5s).
        _piece.Tick(2.0f);

        Assert.IsTrue(_piece.IsLocked);
        Assert.IsTrue(lockFired);
    }
}
