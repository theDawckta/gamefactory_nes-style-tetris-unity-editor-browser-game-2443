using UnityEngine;
using UnityEngine.UIElements;

public class GameOverScreen : BaseScreen
{
    public VisualElement InitialsRegion { get; private set; }
    public VisualElement ReturnPromptRegion { get; private set; }
    public bool ReturnPromptVisible { get; private set; }

    private VisualElement _finalScoreRegion;

    private void Start()
    {
        var root = Document.rootVisualElement;
        if (root == null) return;
        _finalScoreRegion = root.Q("finalScoreRegion");
        InitialsRegion = root.Q("initialsRegion");
        ReturnPromptRegion = root.Q("returnPromptRegion");
    }

    public void ShowWithScore(int score)
    {
        Show();

        if (_finalScoreRegion != null)
        {
            var scoreLabel = _finalScoreRegion.Q<Label>("finalScoreLabel");
            if (scoreLabel != null)
                scoreLabel.text = $"SCORE: {score}";
        }

        bool isTopFive = LeaderboardService.Instance == null || LeaderboardService.Instance.IsTopFive(score);

        if (InitialsRegion != null)
            InitialsRegion.style.display = isTopFive ? DisplayStyle.Flex : DisplayStyle.None;
        if (ReturnPromptRegion != null)
            ReturnPromptRegion.style.display = isTopFive ? DisplayStyle.None : DisplayStyle.Flex;
    }

    public void ShowReturnPrompt()
    {
        ReturnPromptVisible = true;
        if (InitialsRegion != null)
            InitialsRegion.style.display = DisplayStyle.None;
        if (ReturnPromptRegion != null)
            ReturnPromptRegion.style.display = DisplayStyle.Flex;
    }
}
