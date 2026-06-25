using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TetrominoDefinitionsTests
{
    [UnityTest]
    public IEnumerator All_Returns7Entries()
    {
        yield return null;
        Assert.AreEqual(7, TetrominoDefinitions.All.Length);
    }

    [UnityTest]
    public IEnumerator Random_ReturnsNonNull()
    {
        yield return null;
        Assert.IsNotNull(TetrominoDefinitions.Random());
    }

    [UnityTest]
    public IEnumerator Random_ResultIsInAll()
    {
        yield return null;
        var result = TetrominoDefinitions.Random();
        bool found = false;
        foreach (var entry in TetrominoDefinitions.All)
            if (entry == result) { found = true; break; }
        Assert.IsTrue(found, "Random() result must be a member of All");
    }

    [UnityTest]
    public IEnumerator AllEntries_HaveFourRotationStates()
    {
        yield return null;
        foreach (var entry in TetrominoDefinitions.All)
            Assert.AreEqual(4, entry.rotationStates.Length);
    }
}
