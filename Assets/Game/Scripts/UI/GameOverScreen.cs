using UnityEngine;
using UnityEngine.UIElements;

public class GameOverScreen : BaseScreen
{
    public VisualElement InitialsRegion { get; private set; }
    public VisualElement ReturnPromptRegion { get; private set; }

    private VisualElement _finalScoreRegion;

    private void Start()
    {
        var root = Document.rootVisualElement;
        if (root == null)
            return;
        _finalScoreRegion = root.Q("finalScoreRegion");
        InitialsRegion = root.Q("initialsRegion");
        ReturnPromptRegion = root.Q("returnPromptRegion");
    }

    public void ShowWithScore(int score)
    {
        Show();
        if (_finalScoreRegion != null)
        {
            _finalScoreRegion.Clear();
            var label = new Label(score.ToString());
            label.AddToClassList("final-score-value");
            _finalScoreRegion.Add(label);
        }

        bool isTopFive = LeaderboardService.Instance != null
            ? LeaderboardService.Instance.IsTopFive(score)
            : true;

        SetVisible(InitialsRegion, isTopFive);
        SetVisible(ReturnPromptRegion, !isTopFive);
    }

    public void ShowReturnPrompt()
    {
        SetVisible(InitialsRegion, false);
        SetVisible(ReturnPromptRegion, true);
    }

    private void SetVisible(VisualElement element, bool visible)
    {
        if (element == null)
            return;
        element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
