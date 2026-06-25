using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

public class TetrominoDefinitionsTests
{
    [UnityTest]
    public IEnumerator TetrominoDefinitions_All_ReturnsSeven()
    {
        yield return null;
        var all = TetrominoDefinitions.All;
        Assert.IsNotNull(all, "TetrominoDefinitions.All is null");
        Assert.AreEqual(7, all.Length, "Expected 7 tetromino definitions");
    }

    [UnityTest]
    public IEnumerator TetrominoDefinitions_Random_ReturnsNonNull()
    {
        yield return null;
        var result = TetrominoDefinitions.Random();
        Assert.IsNotNull(result, "TetrominoDefinitions.Random() returned null");
    }

    [UnityTest]
    public IEnumerator TetrominoDefinitions_Random_ReturnsItemFromAll()
    {
        yield return null;
        var all = TetrominoDefinitions.All;
        var result = TetrominoDefinitions.Random();
        bool found = false;
        foreach (var item in all)
        {
            if (item == result) { found = true; break; }
        }
        Assert.IsTrue(found, "Random() returned an item not in All");
    }

    [UnityTest]
    public IEnumerator TetrominoDefinitions_All_AllHaveFourRotationStates()
    {
        yield return null;
        foreach (var data in TetrominoDefinitions.All)
        {
            Assert.IsNotNull(data);
            Assert.AreEqual(4, data.rotationStates.Length,
                data.name + " should have 4 rotation states");
        }
    }
}
