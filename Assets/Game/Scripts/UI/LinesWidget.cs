using UnityEngine;
using UnityEngine.UIElements;

public class LinesWidget : MonoBehaviour
{
    [SerializeField] private GameScreen _gameScreen;

    private ScoringSystem _scoringSystem;
    private Label _valueLabel;

    public string LinesText => _valueLabel != null ? _valueLabel.text : "0";

    private void Start()
    {
        SetRegion(_gameScreen != null ? _gameScreen.LinesRegion : null);
    }

    public void SetRegion(VisualElement region)
    {
        if (region == null) return;
        region.Clear();

        var header = new Label("LINES");
        header.AddToClassList("hud-header");

        _valueLabel = new Label("0");
        _valueLabel.AddToClassList("hud-value");

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

    public void Initialize(ScoringSystem scoringSystem)
    {
        if (_scoringSystem != null)
            _scoringSystem.OnStatsChanged -= HandleStatsChanged;

        _scoringSystem = scoringSystem;

        if (isActiveAndEnabled && _scoringSystem != null)
            _scoringSystem.OnStatsChanged += HandleStatsChanged;
    }

    private void HandleStatsChanged(int score, int level, int totalLines)
    {
        if (_valueLabel != null)
            _valueLabel.text = totalLines.ToString();
    }
}
