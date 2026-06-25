using UnityEngine;
using UnityEngine.UIElements;

public class ScoreWidget : MonoBehaviour
{
    [SerializeField] private GameScreen _gameScreen;

    private ScoringSystem _scoringSystem;
    private Label _valueLabel;

    public void SetScoringSystem(ScoringSystem scoringSystem)
    {
        if (_scoringSystem != null)
            _scoringSystem.OnStatsChanged -= HandleStatsChanged;
        _scoringSystem = scoringSystem;
        if (_scoringSystem != null && gameObject.activeInHierarchy)
            _scoringSystem.OnStatsChanged += HandleStatsChanged;
    }

    public void InitializeWithRegion(VisualElement scoreRegion)
    {
        if (scoreRegion == null)
            return;
        var header = new Label("SCORE");
        header.AddToClassList("score-header");
        _valueLabel = new Label("0000000");
        _valueLabel.AddToClassList("score-value");
        scoreRegion.Add(header);
        scoreRegion.Add(_valueLabel);
    }

    private void Start()
    {
        if (_gameScreen != null)
            InitializeWithRegion(_gameScreen.ScoreRegion);
    }

    private void OnEnable()
    {
        if (_scoringSystem != null)
            _scoringSystem.OnStatsChanged += HandleStatsChanged;
    }

    private void OnDisable()
    {
        if (_scoringSystem != null)
            _scoringSystem.OnStatsChanged -= HandleStatsChanged;
    }

    private void HandleStatsChanged(int score, int level, int lines)
    {
        if (_valueLabel != null)
            _valueLabel.text = score.ToString("D7");
    }
}
