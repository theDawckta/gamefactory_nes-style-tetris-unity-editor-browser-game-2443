using NUnit.Framework;

public class ScoringSystemTests
{
    private ScoringSystem _scoring;

    [SetUp]
    public void SetUp()
    {
        _scoring = new ScoringSystem();
    }

    [Test]
    public void AddLines_Single_AtLevel0_Adds40Points()
    {
        _scoring.AddLines(1);
        Assert.AreEqual(40, _scoring.Score);
    }

    [Test]
    public void AddLines_Double_AtLevel0_Adds100Points()
    {
        _scoring.AddLines(2);
        Assert.AreEqual(100, _scoring.Score);
    }

    [Test]
    public void AddLines_Triple_AtLevel0_Adds300Points()
    {
        _scoring.AddLines(3);
        Assert.AreEqual(300, _scoring.Score);
    }

    [Test]
    public void AddLines_Tetris_AtLevel0_Adds1200Points()
    {
        _scoring.AddLines(4);
        Assert.AreEqual(1200, _scoring.Score);
    }

    [Test]
    public void AddLines_Tetris_AtLevel1_Adds2400Points()
    {
        // Advance to level 1 by clearing 10 lines first
        for (int i = 0; i < 10; i++)
            _scoring.AddLines(1);

        Assert.AreEqual(1, _scoring.Level);

        int scoreBefore = _scoring.Score;
        _scoring.AddLines(4);
        Assert.AreEqual(scoreBefore + 2400, _scoring.Score);
    }

    [Test]
    public void Level_StartsAtZero()
    {
        Assert.AreEqual(0, _scoring.Level);
    }

    [Test]
    public void Level_IncrementsTo1_After10LinesCleared()
    {
        for (int i = 0; i < 10; i++)
            _scoring.AddLines(1);

        Assert.AreEqual(1, _scoring.Level);
        Assert.AreEqual(10, _scoring.TotalLines);
    }

    [Test]
    public void Level_DoesNotIncrement_Before10Lines()
    {
        for (int i = 0; i < 9; i++)
            _scoring.AddLines(1);

        Assert.AreEqual(0, _scoring.Level);
    }

    [Test]
    public void TotalLines_TracksAllClearedLines()
    {
        _scoring.AddLines(2);
        _scoring.AddLines(3);
        Assert.AreEqual(5, _scoring.TotalLines);
    }

    [Test]
    public void OnStatsChanged_FiresAfterAddLines()
    {
        bool fired = false;
        int firedScore = -1, firedLevel = -1, firedLines = -1;

        _scoring.OnStatsChanged += (score, level, lines) =>
        {
            fired = true;
            firedScore = score;
            firedLevel = level;
            firedLines = lines;
        };

        _scoring.AddLines(1);

        Assert.IsTrue(fired);
        Assert.AreEqual(40, firedScore);
        Assert.AreEqual(0, firedLevel);
        Assert.AreEqual(1, firedLines);
    }

    [Test]
    public void OnStatsChanged_ReportsUpdatedLevel_WhenLevelAdvances()
    {
        int lastLevel = -1;
        _scoring.OnStatsChanged += (score, level, lines) => lastLevel = level;

        for (int i = 0; i < 10; i++)
            _scoring.AddLines(1);

        Assert.AreEqual(1, lastLevel);
    }

    [Test]
    public void Reset_SetsAllValuesToZero()
    {
        _scoring.AddLines(4);
        _scoring.AddLines(4);
        _scoring.Reset();

        Assert.AreEqual(0, _scoring.Score);
        Assert.AreEqual(0, _scoring.Level);
        Assert.AreEqual(0, _scoring.TotalLines);
    }

    [Test]
    public void AddLines_InvalidCount_DoesNotChangeScore()
    {
        _scoring.AddLines(0);
        _scoring.AddLines(5);
        Assert.AreEqual(0, _scoring.Score);
        Assert.AreEqual(0, _scoring.TotalLines);
    }
}
