using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class TetrominoDataTests
{
    private const string BasePath = "Assets/Resources/Tetrominoes/";

    private TetrominoData Load(string name)
    {
        return AssetDatabase.LoadAssetAtPath<TetrominoData>(BasePath + name + ".asset");
    }

    [Test]
    public void TetrominoData_I_ExistsWithCorrectColorIndex()
    {
        var data = Load("I");
        Assert.IsNotNull(data, "I.asset not found");
        Assert.AreEqual(1, data.colorIndex);
    }

    [Test]
    public void TetrominoData_O_ExistsWithCorrectColorIndex()
    {
        var data = Load("O");
        Assert.IsNotNull(data, "O.asset not found");
        Assert.AreEqual(2, data.colorIndex);
    }

    [Test]
    public void TetrominoData_T_ExistsWithCorrectColorIndex()
    {
        var data = Load("T");
        Assert.IsNotNull(data, "T.asset not found");
        Assert.AreEqual(3, data.colorIndex);
    }

    [Test]
    public void TetrominoData_S_ExistsWithCorrectColorIndex()
    {
        var data = Load("S");
        Assert.IsNotNull(data, "S.asset not found");
        Assert.AreEqual(4, data.colorIndex);
    }

    [Test]
    public void TetrominoData_Z_ExistsWithCorrectColorIndex()
    {
        var data = Load("Z");
        Assert.IsNotNull(data, "Z.asset not found");
        Assert.AreEqual(5, data.colorIndex);
    }

    [Test]
    public void TetrominoData_J_ExistsWithCorrectColorIndex()
    {
        var data = Load("J");
        Assert.IsNotNull(data, "J.asset not found");
        Assert.AreEqual(6, data.colorIndex);
    }

    [Test]
    public void TetrominoData_L_ExistsWithCorrectColorIndex()
    {
        var data = Load("L");
        Assert.IsNotNull(data, "L.asset not found");
        Assert.AreEqual(7, data.colorIndex);
    }

    [Test]
    public void TetrominoData_AllPieces_HaveFourRotationStates()
    {
        foreach (string name in new[] { "I", "O", "T", "S", "Z", "J", "L" })
        {
            var data = Load(name);
            Assert.IsNotNull(data, name + ".asset not found");
            Assert.AreEqual(4, data.rotationStates.Length, name + " should have 4 rotation states");
        }
    }

    [Test]
    public void TetrominoData_AllPieces_EachRotationHasFourCells()
    {
        foreach (string name in new[] { "I", "O", "T", "S", "Z", "J", "L" })
        {
            var data = Load(name);
            Assert.IsNotNull(data, name + ".asset not found");
            for (int i = 0; i < 4; i++)
            {
                Assert.IsNotNull(data.rotationStates[i], name + " rotation " + i + " is null");
                Assert.AreEqual(4, data.rotationStates[i].cells.Length,
                    name + " rotation " + i + " should have 4 cells");
            }
        }
    }

    [Test]
    public void TetrominoData_I_Rot0_IsHorizontalLine()
    {
        var data = Load("I");
        Assert.IsNotNull(data);
        var cells = data.rotationStates[0].cells;
        // All cells on same row (y=0), spanning x=-1..2
        Assert.IsTrue(cells.All(c => c.y == 0), "I Rot0 cells should all be on y=0");
        var xs = cells.Select(c => c.x).OrderBy(x => x).ToArray();
        CollectionAssert.AreEqual(new[] { -1, 0, 1, 2 }, xs, "I Rot0 x positions should be -1,0,1,2");
    }

    [Test]
    public void TetrominoData_I_Rot1_IsVerticalLine()
    {
        var data = Load("I");
        Assert.IsNotNull(data);
        var cells = data.rotationStates[1].cells;
        // All cells on same column (x=0), spanning y=-2..1
        Assert.IsTrue(cells.All(c => c.x == 0), "I Rot1 cells should all be on x=0");
        var ys = cells.Select(c => c.y).OrderByDescending(y => y).ToArray();
        CollectionAssert.AreEqual(new[] { 1, 0, -1, -2 }, ys, "I Rot1 y positions should be 1,0,-1,-2");
    }

    [Test]
    public void TetrominoData_O_AllRotationsSameShape()
    {
        var data = Load("O");
        Assert.IsNotNull(data);
        var rot0 = data.rotationStates[0].cells.OrderBy(c => c.x).ThenBy(c => c.y).ToArray();
        for (int i = 1; i < 4; i++)
        {
            var rotN = data.rotationStates[i].cells.OrderBy(c => c.x).ThenBy(c => c.y).ToArray();
            CollectionAssert.AreEqual(rot0, rotN, "O piece rotation " + i + " should equal rotation 0");
        }
    }

    [Test]
    public void TetrominoData_T_Rot0_HasCorrectShape()
    {
        var data = Load("T");
        Assert.IsNotNull(data);
        var cells = data.rotationStates[0].cells;
        // T facing up: top center + 3 in a row below
        // Expected: (0,1), (-1,0), (0,0), (1,0)
        var expected = new[] { new Vector2Int(0,1), new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0) };
        var sorted = cells.OrderBy(c => c.y).ThenBy(c => c.x).ToArray();
        var expectedSorted = expected.OrderBy(c => c.y).ThenBy(c => c.x).ToArray();
        CollectionAssert.AreEqual(expectedSorted, sorted, "T Rot0 shape mismatch");
    }

    [Test]
    public void TetrominoData_AllPieces_NoDuplicateCellsInAnyRotation()
    {
        foreach (string name in new[] { "I", "O", "T", "S", "Z", "J", "L" })
        {
            var data = Load(name);
            Assert.IsNotNull(data, name + ".asset not found");
            for (int i = 0; i < 4; i++)
            {
                var cells = data.rotationStates[i].cells;
                var distinct = cells.Distinct().Count();
                Assert.AreEqual(4, distinct, name + " rotation " + i + " has duplicate cells");
            }
        }
    }
}
