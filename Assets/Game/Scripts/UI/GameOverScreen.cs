using UnityEngine;
using UnityEngine.UIElements;

public class GameOverScreen : BaseScreen
{
    public VisualElement FinalScoreRegion { get; private set; }
    public VisualElement InitialsRegion { get; private set; }
    public bool ReturnPromptVisible { get; private set; }

    [SerializeField] private FinalScoreWidget _finalScoreWidget;

    private VisualElement _returnPromptRegion;

    private void Start()
    {
        var root = Document.rootVisualElement;
        if (root == null) return;
        FinalScoreRegion = root.Q("finalScoreRegion");
        InitialsRegion = root.Q("initialsRegion");
        _returnPromptRegion = root.Q("returnPromptRegion");
    }

    public void ShowWithScore(int score)
    {
        Show();
        if (_finalScoreWidget != null)
            _finalScoreWidget.SetScore(score);
    }

    public void ShowReturnPrompt()
    {
        ReturnPromptVisible = true;
        if (_returnPromptRegion != null)
            _returnPromptRegion.style.display = DisplayStyle.Flex;
    }
}
