using System;
using OneTimeGames.CoreSystems;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    private const string StatePlaying = "Playing";
    private const string StateLineClear = "LineClear";
    private const string StateGameOver = "GameOver";
    private const float LineClearDelay = 0.5f;

    public PlayfieldController PlayfieldController;
    public PieceController PieceController;
    public PlayfieldRenderer PlayfieldRenderer;

    private GameStateMachine _stateMachine;
    private ScoringSystem _scoring;
    private TetrominoData _nextPiece;
    private float _lineClearTimer;

    public event Action OnGameOver;
    public event Action<TetrominoData> OnNextPieceChanged;

    public ScoringSystem Scoring => _scoring;

    private void Awake()
    {
        _scoring = new ScoringSystem();

        _stateMachine = new GameStateMachine();
        _stateMachine.RegisterState(StatePlaying, EnterPlaying, TickPlaying, ExitPlaying);
        _stateMachine.RegisterState(StateLineClear, EnterLineClear, TickLineClear, null);
        _stateMachine.RegisterState(StateGameOver, EnterGameOver, null, null);
    }

    private void Update()
    {
        _stateMachine.Tick();
    }

    public void StartGame()
    {
        PlayfieldController.ClearAll();
        _scoring.Reset();

        _nextPiece = TetrominoDefinitions.Random();
        SpawnNextPiece();

        _stateMachine.TransitionTo(StatePlaying);
    }

    public void StopGame()
    {
        _stateMachine.TransitionTo(StateGameOver);
    }

    private void SpawnNextPiece()
    {
        var toSpawn = _nextPiece;
        _nextPiece = TetrominoDefinitions.Random();
        OnNextPieceChanged?.Invoke(_nextPiece);
        PieceController.SpawnPiece(toSpawn);
    }

    private bool IsSpawnPositionValid()
    {
        return PlayfieldController.IsValidPosition(
            PieceController.CurrentData,
            PieceController.CurrentRotation,
            PieceController.CurrentPivot);
    }

    private void EnterPlaying()
    {
        PieceController.OnPieceLocked += HandlePieceLocked;
    }

    private void TickPlaying()
    {
        PieceController.Level = _scoring.Level;
        PieceController.Tick(Time.deltaTime);
    }

    private void ExitPlaying()
    {
        PieceController.OnPieceLocked -= HandlePieceLocked;
    }

    private void HandlePieceLocked()
    {
        int[] clearedRows = PlayfieldController.ClearLines();
        if (clearedRows.Length > 0)
        {
            _scoring.AddLines(clearedRows.Length);
            _stateMachine.TransitionTo(StateLineClear);
        }
        else
        {
            SpawnAndCheckGameOver();
        }
    }

    private void EnterLineClear()
    {
        _lineClearTimer = 0f;
    }

    private void TickLineClear()
    {
        _lineClearTimer += Time.deltaTime;
        if (_lineClearTimer >= LineClearDelay)
        {
            SpawnNextPiece();
            if (!IsSpawnPositionValid())
                _stateMachine.TransitionTo(StateGameOver);
            else
                _stateMachine.TransitionTo(StatePlaying);
        }
    }

    private void EnterGameOver()
    {
        OnGameOver?.Invoke();
    }

    private void SpawnAndCheckGameOver()
    {
        SpawnNextPiece();
        if (!IsSpawnPositionValid())
            _stateMachine.TransitionTo(StateGameOver);
    }
}
