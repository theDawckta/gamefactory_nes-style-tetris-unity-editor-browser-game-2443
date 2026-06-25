using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class StartScreen : BaseScreen
{
    [SerializeField] private LeaderboardWidget _leaderboardWidget;

    public VisualElement LeaderboardRegion { get; private set; }
    public VisualElement PromptRegion { get; private set; }

    public event Action OnStartPressed;

    private bool _listening;

    private void Start()
    {
        var root = Document.rootVisualElement;
        if (root == null) return;
        LeaderboardRegion = root.Q("leaderboardRegion");
        PromptRegion = root.Q("promptRegion");
    }

    private void Update()
    {
        if (!_listening) return;
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
            OnStartPressed?.Invoke();
    }

    protected override void OnShow()
    {
        _listening = true;
        _leaderboardWidget?.SetLoading();
        if (LeaderboardService.Instance != null && LeaderboardService.Instance.isActiveAndEnabled)
            LeaderboardService.Instance.FetchScores(OnScoresFetched);
    }

    protected override void OnHide()
    {
        _listening = false;
    }

    private void OnScoresFetched(LeaderboardEntry[] entries)
    {
        _leaderboardWidget?.SetScores(entries);
    }
}
