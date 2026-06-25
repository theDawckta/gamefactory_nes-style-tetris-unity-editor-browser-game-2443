using UnityEngine;
using UnityEngine.UIElements;

public class StartPromptWidget : MonoBehaviour
{
    [SerializeField] private StartScreen _startScreen;

    private Label _promptLabel;
    private float _blinkTimer;

    private void Start()
    {
        var region = _startScreen != null ? _startScreen.PromptRegion : null;
        if (region == null) return;

        region.Clear();
        region.style.flexDirection = FlexDirection.Column;
        region.style.alignItems = Align.Center;
        region.style.justifyContent = Justify.Center;

        _promptLabel = new Label("PRESS ENTER TO START");
        _promptLabel.style.fontSize = 20;
        _promptLabel.style.color = new StyleColor(new Color(0.1f, 0.1f, 0.1f));
        _promptLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        _promptLabel.style.display = DisplayStyle.None;
        region.Add(_promptLabel);
    }

    private void Update()
    {
        if (_promptLabel == null) return;

        if (_startScreen == null || !_startScreen.IsVisible)
        {
            _promptLabel.style.display = DisplayStyle.None;
            _blinkTimer = 0f;
            return;
        }

        _blinkTimer += Time.deltaTime;
        if (_blinkTimer >= 1.0f)
            _blinkTimer -= 1.0f;

        _promptLabel.style.display = _blinkTimer < 0.6f ? DisplayStyle.Flex : DisplayStyle.None;
    }

    internal bool IsPromptLabelVisible =>
        _promptLabel != null && _promptLabel.style.display == DisplayStyle.Flex;
}
