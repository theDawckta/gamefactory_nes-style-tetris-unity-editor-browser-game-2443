using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InitialsEntryWidget : MonoBehaviour
{
    [SerializeField] private GameOverScreen _gameOverScreen;

    private static readonly char[] ValidChars = BuildCharset();

    private readonly int[] _slotIndices = new int[3];
    private int _cursorSlot;
    private int _score;
    private bool _active;
    private bool _awaitingConfirm;

    private Label[] _charLabels;
    private Label[] _cursorLabels;
    private Label _confirmLabel;

    public event Action OnCharCycled;
    public event Action OnSlotConfirmed;

    private void Start()
    {
        BuildUI();
    }

    private void BuildUI()
    {
        var region = _gameOverScreen != null ? _gameOverScreen.InitialsRegion : null;
        if (region == null) return;

        region.Clear();
        region.style.flexDirection = FlexDirection.Column;
        region.style.alignItems = Align.Center;
        region.style.justifyContent = Justify.Center;

        var slotsRow = new VisualElement();
        slotsRow.style.flexDirection = FlexDirection.Row;
        slotsRow.style.justifyContent = Justify.Center;
        slotsRow.style.marginBottom = 4;

        _charLabels = new Label[3];
        _cursorLabels = new Label[3];

        for (int i = 0; i < 3; i++)
        {
            var slotCol = new VisualElement();
            slotCol.style.flexDirection = FlexDirection.Column;
            slotCol.style.alignItems = Align.Center;
            slotCol.style.width = 48;
            slotCol.style.marginLeft = 4;
            slotCol.style.marginRight = 4;

            _charLabels[i] = new Label("A");
            _charLabels[i].style.fontSize = 32;
            _charLabels[i].style.color = new StyleColor(new Color(0.1f, 0.1f, 0.1f));
            _charLabels[i].style.unityTextAlign = TextAnchor.MiddleCenter;

            _cursorLabels[i] = new Label("_");
            _cursorLabels[i].style.fontSize = 20;
            _cursorLabels[i].style.color = new StyleColor(new Color(0.1f, 0.1f, 0.1f));
            _cursorLabels[i].style.unityTextAlign = TextAnchor.MiddleCenter;
            _cursorLabels[i].style.display = DisplayStyle.None;

            slotCol.Add(_charLabels[i]);
            slotCol.Add(_cursorLabels[i]);
            slotsRow.Add(slotCol);
        }

        region.Add(slotsRow);

        _confirmLabel = new Label("CONFIRM");
        _confirmLabel.style.fontSize = 20;
        _confirmLabel.style.color = new StyleColor(new Color(0.1f, 0.1f, 0.1f));
        _confirmLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        _confirmLabel.style.marginTop = 8;
        _confirmLabel.style.display = DisplayStyle.None;
        region.Add(_confirmLabel);
    }

    private void Update()
    {
        if (!_active) return;
        var kb = Keyboard.current;
        if (kb == null) return;

        if (_awaitingConfirm)
        {
            if (kb.downArrowKey.wasPressedThisFrame)
                SubmitInitials();
            else if (kb.upArrowKey.wasPressedThisFrame)
                CancelConfirm();
        }
        else
        {
            if (kb.leftArrowKey.wasPressedThisFrame) CycleChar(-1);
            else if (kb.rightArrowKey.wasPressedThisFrame) CycleChar(1);
            else if (kb.downArrowKey.wasPressedThisFrame) ConfirmSlot();
            else if (kb.upArrowKey.wasPressedThisFrame) GoBack();
        }
    }

    internal void CycleChar(int dir)
    {
        _slotIndices[_cursorSlot] = (_slotIndices[_cursorSlot] + dir + ValidChars.Length) % ValidChars.Length;
        RefreshSlotLabel(_cursorSlot);
        OnCharCycled?.Invoke();
    }

    internal void ConfirmSlot()
    {
        if (_cursorSlot < 2)
        {
            _cursorSlot++;
            RefreshCursor();
        }
        else
        {
            _awaitingConfirm = true;
            RefreshCursor();
            ShowConfirmPrompt();
        }
        OnSlotConfirmed?.Invoke();
    }

    internal void GoBack()
    {
        if (_cursorSlot > 0)
        {
            _cursorSlot--;
            RefreshCursor();
        }
    }

    private void CancelConfirm()
    {
        _awaitingConfirm = false;
        _cursorSlot = 2;
        HideConfirmPrompt();
        RefreshCursor();
    }

    private void SubmitInitials()
    {
        _active = false;
        var initials = new string(new[]
        {
            ValidChars[_slotIndices[0]],
            ValidChars[_slotIndices[1]],
            ValidChars[_slotIndices[2]]
        });

        if (LeaderboardService.Instance != null)
        {
            LeaderboardService.Instance.SubmitScore(initials, _score, _ =>
            {
                if (_gameOverScreen != null)
                    _gameOverScreen.ShowReturnPrompt();
            });
        }
        else if (_gameOverScreen != null)
        {
            _gameOverScreen.ShowReturnPrompt();
        }
    }

    private void RefreshSlotLabel(int slot)
    {
        if (_charLabels == null || slot < 0 || slot >= _charLabels.Length) return;
        if (_charLabels[slot] != null)
            _charLabels[slot].text = ValidChars[_slotIndices[slot]].ToString();
    }

    private void RefreshCursor()
    {
        if (_cursorLabels == null) return;
        for (int i = 0; i < 3; i++)
        {
            if (_cursorLabels[i] == null) continue;
            _cursorLabels[i].style.display = (!_awaitingConfirm && i == _cursorSlot)
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }
    }

    private void ShowConfirmPrompt()
    {
        if (_confirmLabel != null)
            _confirmLabel.style.display = DisplayStyle.Flex;
    }

    private void HideConfirmPrompt()
    {
        if (_confirmLabel != null)
            _confirmLabel.style.display = DisplayStyle.None;
    }

    public void Activate(int score)
    {
        _score = score;
        _slotIndices[0] = _slotIndices[1] = _slotIndices[2] = 0;
        _cursorSlot = 0;
        _awaitingConfirm = false;
        _active = true;

        for (int i = 0; i < 3; i++)
            RefreshSlotLabel(i);

        RefreshCursor();
        HideConfirmPrompt();
    }

    public void Deactivate()
    {
        _active = false;
    }

    internal char[] CurrentChars
    {
        get
        {
            var chars = new char[3];
            for (int i = 0; i < 3; i++)
                chars[i] = ValidChars[_slotIndices[i]];
            return chars;
        }
    }

    internal int CursorSlot => _cursorSlot;
    internal bool IsActive => _active;
    internal bool AwaitingConfirm => _awaitingConfirm;
    internal int Score => _score;

    private static char[] BuildCharset()
    {
        var chars = new char[36];
        for (int i = 0; i < 26; i++) chars[i] = (char)('A' + i);
        for (int i = 0; i < 10; i++) chars[26 + i] = (char)('0' + i);
        return chars;
    }
}
