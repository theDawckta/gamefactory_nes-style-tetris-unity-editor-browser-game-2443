using UnityEngine;
using UnityEngine.UIElements;

public class FinalScoreWidget : MonoBehaviour
{
    [SerializeField] private GameOverScreen _gameOverScreen;

    private Label _valueLabel;

    private void Start()
    {
        var region = _gameOverScreen != null ? _gameOverScreen.FinalScoreRegion : null;
        InitializeWithRegion(region);
    }

    internal void InitializeWithRegion(VisualElement region)
    {
        if (region == null) return;

        region.Clear();
        region.style.flexDirection = FlexDirection.Column;
        region.style.alignItems = Align.Center;
        region.style.justifyContent = Justify.Center;

        var headerLabel = new Label("SCORE");
        headerLabel.AddToClassList("final-score-header");
        region.Add(headerLabel);

        _valueLabel = new Label("0000000");
        _valueLabel.AddToClassList("final-score-value");
        region.Add(_valueLabel);
    }

    public void SetScore(int score)
    {
        if (_valueLabel == null) return;
        _valueLabel.text = score.ToString("D7");
    }
}
