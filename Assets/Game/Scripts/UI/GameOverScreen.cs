using UnityEngine;
using UnityEngine.UIElements;

public class GameOverScreen : BaseScreen
{
    public VisualElement InitialsRegion { get; private set; }
    public VisualElement ReturnPromptRegion { get; private set; }
    public bool ReturnPromptVisible { get; private set; }

    private void Start()
    {
        var root = Document.rootVisualElement;
        if (root == null) return;
        InitialsRegion = root.Q("initialsRegion");
        ReturnPromptRegion = root.Q("returnPromptRegion");
    }

    public void ShowReturnPrompt()
    {
        ReturnPromptVisible = true;
        if (ReturnPromptRegion != null)
            ReturnPromptRegion.style.display = DisplayStyle.Flex;
    }
}
