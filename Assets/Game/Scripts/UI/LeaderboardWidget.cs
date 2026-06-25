using UnityEngine;
using UnityEngine.UIElements;

public class LeaderboardWidget : MonoBehaviour
{
    [SerializeField] private StartScreen _startScreen;

    private Label _loadingLabel;
    private VisualElement[] _rows;
    private Label[] _rankLabels;
    private Label[] _initialsLabels;
    private Label[] _scoreLabels;

    private void Start()
    {
        if (_startScreen == null || _startScreen.LeaderboardRegion == null)
            return;
        InitializeWithRegion(_startScreen.LeaderboardRegion);
    }

    public void InitializeWithRegion(VisualElement region)
    {
        if (region == null) return;
        region.Clear();

        var header = new Label("TOP 5");
        header.AddToClassList("leaderboard-header");
        region.Add(header);

        _loadingLabel = new Label("LOADING...");
        _loadingLabel.AddToClassList("leaderboard-loading");
        region.Add(_loadingLabel);

        _rows = new VisualElement[5];
        _rankLabels = new Label[5];
        _initialsLabels = new Label[5];
        _scoreLabels = new Label[5];

        for (int i = 0; i < 5; i++)
        {
            var row = new VisualElement();
            row.AddToClassList("leaderboard-row");

            var rank = new Label((i + 1).ToString());
            rank.AddToClassList("leaderboard-rank");

            var initials = new Label("---");
            initials.AddToClassList("leaderboard-initials");

            var score = new Label("0");
            score.AddToClassList("leaderboard-score");

            row.Add(rank);
            row.Add(initials);
            row.Add(score);
            region.Add(row);

            _rows[i] = row;
            _rankLabels[i] = rank;
            _initialsLabels[i] = initials;
            _scoreLabels[i] = score;
        }

        SetLoading();
    }

    public void SetLoading()
    {
        if (_loadingLabel == null) return;
        _loadingLabel.style.display = DisplayStyle.Flex;
        SetRowsVisible(false);
    }

    public void SetScores(LeaderboardEntry[] entries)
    {
        if (_rows == null) return;
        _loadingLabel.style.display = DisplayStyle.None;
        SetRowsVisible(true);

        for (int i = 0; i < 5; i++)
        {
            string initials = "---";
            string score = "0";
            if (entries != null && i < entries.Length)
            {
                initials = string.IsNullOrEmpty(entries[i].Initials) ? "---" : entries[i].Initials;
                score = entries[i].Score.ToString();
            }
            _rankLabels[i].text = (i + 1).ToString();
            _initialsLabels[i].text = initials;
            _scoreLabels[i].text = score;
        }
    }

    private void SetRowsVisible(bool visible)
    {
        if (_rows == null) return;
        var display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        foreach (var row in _rows)
            row.style.display = display;
    }
}
