using UnityEngine;
using UnityEngine.UIElements;

public class GameOverScreen : BaseScreen
{
    public VisualElement InitialsRegion { get; private set; }
    public bool ReturnPromptVisible { get; private set; }

    private VisualElement _returnPromptRegion;

    private void Start()
    {
        var root = Document.rootVisualElement;
        if (root == null) return;
        InitialsRegion = root.Q("initialsRegion");
        _returnPromptRegion = root.Q("returnPromptRegion");
    }

    public void ShowReturnPrompt()
    {
        ReturnPromptVisible = true;
        if (_returnPromptRegion != null)
            _returnPromptRegion.style.display = DisplayStyle.Flex;
    }
}
