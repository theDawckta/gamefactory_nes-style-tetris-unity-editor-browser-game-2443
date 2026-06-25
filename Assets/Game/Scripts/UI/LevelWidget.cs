using UnityEngine;
using UnityEngine.UIElements;

public class LevelWidget : MonoBehaviour
{
    [SerializeField] private GameScreen _gameScreen;

    private ScoringSystem _scoringSystem;
    private Label _valueLabel;
    private int _currentLevel;

    public string LevelText => (_currentLevel + 1).ToString();

    public void Initialize(GameScreen screen, ScoringSystem scoringSystem)
    {
        _gameScreen = screen;
        _scoringSystem = scoringSystem;
    }

    private void Start()
    {
        if (_gameScreen == null || _gameScreen.LevelRegion == null)
            return;
        InitializeWithRegion(_gameScreen.LevelRegion);
    }

    internal void InitializeWithRegion(VisualElement region)
    {
        if (region == null) return;

        var header = new Label("LEVEL");
        header.AddToClassList("hud-header");

        _valueLabel = new Label(LevelText);
        _valueLabel.AddToClassList("hud-value");

        region.Clear();
        region.Add(header);
        region.Add(_valueLabel);
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
        _currentLevel = level;
        if (_valueLabel != null)
            _valueLabel.text = LevelText;
    }
}
