using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameplayControllerTests
{
    private GameObject _controllerGo;
    private GameObject _playfieldGo;
    private GameObject _pieceGo;
    private GameplayController _gc;
    private PlayfieldController _playfield;
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
    }

    [TearDown]
    public void TearDown()
    {
        if (_controllerGo != null) Object.Destroy(_controllerGo);
        if (_playfieldGo != null) Object.Destroy(_playfieldGo);
        if (_pieceGo != null) Object.Destroy(_pieceGo);
    }

    [UnityTest]
    public IEnumerator GameplayController_HasGameplayControllerComponent()
    {
        yield return null;
        Assert.IsNotNull(_controllerGo.GetComponent<GameplayController>());
    }

    [UnityTest]
    public IEnumerator GameplayController_ScoringIsNotNull()
    {
        yield return null;
        Assert.IsNotNull(_gc.Scoring);
    }

    [UnityTest]
    public IEnumerator GameplayController_StartGame_ResetsScoreToZero()
    {
        yield return null;
        _gc.StartGame();
        Assert.AreEqual(0, _gc.Scoring.Score);
    }

    [UnityTest]
    public IEnumerator GameplayController_StartGame_SpawnsPieceInPieceController()
    {
        yield return null;
        _gc.StartGame();
        Assert.IsNotNull(_piece.CurrentData);
    }

    [UnityTest]
    public IEnumerator GameplayController_StartGame_FiresOnNextPieceChanged()
    {
        yield return null;
        bool fired = false;
        _gc.OnNextPieceChanged += _ => fired = true;
        _gc.StartGame();
        Assert.IsTrue(fired);
    }

    [UnityTest]
    public IEnumerator GameplayController_StartGame_OnNextPieceChangedPayloadIsNotNull()
    {
        yield return null;
        TetrominoData received = null;
        _gc.OnNextPieceChanged += d => received = d;
        _gc.StartGame();
        Assert.IsNotNull(received);
    }

    [UnityTest]
    public IEnumerator GameplayController_StopGame_FiresOnGameOver()
    {
        yield return null;
        _gc.StartGame();
        bool fired = false;
        _gc.OnGameOver += () => fired = true;
        _gc.StopGame();
        Assert.IsTrue(fired);
    }

    [UnityTest]
    public IEnumerator GameplayController_PieceControllerLevelMatchesScoringLevelAfterTick()
    {
        yield return null;
        _gc.StartGame();
        yield return null;
        Assert.AreEqual(_gc.Scoring.Level, _piece.Level);
    }
}
