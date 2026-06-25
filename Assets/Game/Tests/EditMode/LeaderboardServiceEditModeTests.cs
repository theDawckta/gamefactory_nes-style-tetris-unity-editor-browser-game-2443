using NUnit.Framework;

public class LeaderboardServiceEditModeTests
{
    [Test]
    public void LeaderboardEntry_StoresInitials()
    {
        var entry = new LeaderboardEntry { Initials = "AAA", Score = 1000 };
        Assert.AreEqual("AAA", entry.Initials);
    }

    [Test]
    public void LeaderboardEntry_StoresScore()
    {
        var entry = new LeaderboardEntry { Initials = "ZZZ", Score = 9999 };
        Assert.AreEqual(9999, entry.Score);
    }

    [Test]
    public void LeaderboardEntry_DefaultInitialsIsNull()
    {
        var entry = new LeaderboardEntry();
        Assert.IsNull(entry.Initials);
    }

    [Test]
    public void LeaderboardEntry_DefaultScoreIsZero()
    {
        var entry = new LeaderboardEntry();
        Assert.AreEqual(0, entry.Score);
    }
}
