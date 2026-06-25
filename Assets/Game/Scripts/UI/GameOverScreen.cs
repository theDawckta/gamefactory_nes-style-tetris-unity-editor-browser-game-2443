using UnityEngine;
using UnityEngine.UIElements;

public class GameOverScreen : BaseScreen
{
    [SerializeField] private FinalScoreWidget _finalScoreWidget;

    public VisualElement FinalScoreRegion { get; private set; }
    public VisualElement InitialsRegion { get; private set; }
    public VisualElement ReturnPromptRegion { get; private set; }
    public bool ReturnPromptVisible { get; private set; }

    private void Start()
    {
        var root = Document.rootVisualElement;
        if (root == null) return;
        FinalScoreRegion = root.Q("finalScoreRegion");
        InitialsRegion = root.Q("initialsRegion");
        ReturnPromptRegion = root.Q("returnPromptRegion");
    }

    public void ShowWithScore(int score)
    {
        Show();
        _finalScoreWidget?.SetScore(score);

        bool isTopFive = LeaderboardService.Instance != null
            ? LeaderboardService.Instance.IsTopFive(score)
            : true;

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
