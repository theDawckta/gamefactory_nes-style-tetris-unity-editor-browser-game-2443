using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] private StartScreen _startScreen;
    [SerializeField] private GameScreen _gameScreen;
    [SerializeField] private GameOverScreen _gameOverScreen;
    [SerializeField] private GameplayController _gameplayController;
    [SerializeField] private InitialsEntryWidget _initialsEntryWidget;
    [SerializeField] private ReturnPromptWidget _returnPromptWidget;
    [SerializeField] private ScoreWidget _scoreWidget;
    [SerializeField] private LevelWidget _levelWidget;
    [SerializeField] private LinesWidget _linesWidget;

    private void Start()
    {
        WireWidgetScoringSystem();

        _startScreen.OnStartPressed += HandleStartPressed;
        _gameplayController.OnGameOver += HandleGameOver;
        _returnPromptWidget.OnReturnPressed += HandleReturnPressed;

        _gameOverScreen.Hide();
        _gameScreen.Hide();
        _startScreen.Show();
    }

    private void WireWidgetScoringSystem()
    {
        var scoring = _gameplayController.Scoring;
        _scoreWidget?.SetScoringSystem(scoring);
        _linesWidget?.Initialize(scoring);
        if (_levelWidget != null)
        {
            _levelWidget.Initialize(_gameScreen, scoring);
            // Re-trigger OnEnable so LevelWidget subscribes to the newly set scoring system.
            _levelWidget.enabled = false;
            _levelWidget.enabled = true;
        }
    }

    private void HandleStartPressed()
    {
        _startScreen.Hide();
        _gameplayController.StartGame();
        _gameScreen.Show();
    }

    private void HandleGameOver()
    {
        _gameScreen.Hide();
        int score = _gameplayController.Scoring.Score;
        _gameOverScreen.ShowWithScore(score);
        bool isTopFive = LeaderboardService.Instance != null
            ? LeaderboardService.Instance.IsTopFive(score)
            : true;
        if (isTopFive)
            _initialsEntryWidget?.Activate(score);
    }

    private void HandleReturnPressed()
    {
        _gameOverScreen.Hide();
        _startScreen.Show();
    }

    public void StartGame()
    {
        _startScreen.Hide();
        _gameplayController.StartGame();
        _gameScreen.Show();
    }

    public void GoToGameOver()
    {
        _gameplayController.StopGame();
    }

    public void GoToStart()
    {
        _gameScreen.Hide();
        _gameOverScreen.Hide();
        _startScreen.Show();
    }
}
