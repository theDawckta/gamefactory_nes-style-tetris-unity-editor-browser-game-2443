using UnityEngine;
using UnityEngine.UIElements;

public class FinalScoreWidget : MonoBehaviour
{
    [SerializeField] private GameOverScreen _gameOverScreen;

    private Label _valueLabel;

    public void InitializeWithRegion(VisualElement finalScoreRegion)
    {
        if (finalScoreRegion == null) return;

        var header = new Label("SCORE");
        header.AddToClassList("final-score-header");
        _valueLabel = new Label("0000000");
        _valueLabel.AddToClassList("final-score-value");
        finalScoreRegion.Add(header);
        finalScoreRegion.Add(_valueLabel);
    }

    public void SetScore(int score)
    {
        if (_valueLabel != null)
            _valueLabel.text = score.ToString("D7");
    }

    private void Start()
    {
        if (_gameOverScreen != null)
            InitializeWithRegion(_gameOverScreen.FinalScoreRegion);
    }
}
