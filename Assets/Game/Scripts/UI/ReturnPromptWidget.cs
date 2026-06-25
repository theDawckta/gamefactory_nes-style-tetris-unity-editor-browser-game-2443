using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ReturnPromptWidget : MonoBehaviour
{
    [SerializeField] private GameOverScreen _gameOverScreen;

    public event Action OnReturnPressed;

    private Label _promptLabel;

    private void Start()
    {
        var region = _gameOverScreen != null ? _gameOverScreen.ReturnPromptRegion : null;
        if (region == null) return;

        region.Clear();
        region.style.flexDirection = FlexDirection.Column;
        region.style.alignItems = Align.Center;
        region.style.justifyContent = Justify.Center;

        _promptLabel = new Label("PRESS ENTER TO CONTINUE");
        _promptLabel.style.fontSize = 20;
        _promptLabel.style.color = new StyleColor(new Color(0.1f, 0.1f, 0.1f));
        _promptLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        region.Add(_promptLabel);

        StartCoroutine(BlinkCoroutine());
    }

    private void Update()
    {
        if (_gameOverScreen == null || !_gameOverScreen.ReturnPromptVisible) return;
        var kb = Keyboard.current;
        if (kb == null) return;
        if (kb.enterKey.wasPressedThisFrame)
            OnReturnPressed?.Invoke();
    }

    private IEnumerator BlinkCoroutine()
    {
        while (true)
        {
            bool visible = _gameOverScreen != null && _gameOverScreen.ReturnPromptVisible;
            if (!visible)
            {
                if (_promptLabel != null)
                    _promptLabel.style.display = DisplayStyle.None;
                yield return null;
                continue;
            }
            if (_promptLabel != null)
                _promptLabel.style.display = DisplayStyle.Flex;
            yield return new WaitForSeconds(0.6f);
            if (_promptLabel != null)
                _promptLabel.style.display = DisplayStyle.None;
            yield return new WaitForSeconds(0.4f);
        }
    }

    internal bool IsPromptLabelVisible =>
        _promptLabel != null && _promptLabel.style.display == DisplayStyle.Flex;

    internal void SimulateReturnPressed() => OnReturnPressed?.Invoke();
}
