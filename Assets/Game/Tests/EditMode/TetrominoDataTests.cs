using NUnit.Framework;
using UnityEngine;
using UnityEditor;

public class TetrominoDataTests
{
    static TetrominoData Load(string name) =>
        AssetDatabase.LoadAssetAtPath<TetrominoData>($"Assets/Resources/Tetrominoes/{name}.asset");

    [Test] public void IPiece_ColorIndexIs1() { var d = Load("I"); Assert.IsNotNull(d); Assert.AreEqual(1, d.colorIndex); }
    [Test] public void OPiece_ColorIndexIs2() { var d = Load("O"); Assert.IsNotNull(d); Assert.AreEqual(2, d.colorIndex); }
    [Test] public void TPiece_ColorIndexIs3() { var d = Load("T"); Assert.IsNotNull(d); Assert.AreEqual(3, d.colorIndex); }
    [Test] public void SPiece_ColorIndexIs4() { var d = Load("S"); Assert.IsNotNull(d); Assert.AreEqual(4, d.colorIndex); }
    [Test] public void ZPiece_ColorIndexIs5() { var d = Load("Z"); Assert.IsNotNull(d); Assert.AreEqual(5, d.colorIndex); }
    [Test] public void JPiece_ColorIndexIs6() { var d = Load("J"); Assert.IsNotNull(d); Assert.AreEqual(6, d.colorIndex); }
    [Test] public void LPiece_ColorIndexIs7() { var d = Load("L"); Assert.IsNotNull(d); Assert.AreEqual(7, d.colorIndex); }

    [Test]
    public void AllPieces_HaveFourRotationStates()
    {
        foreach (string name in new[] { "I", "O", "T", "S", "Z", "J", "L" })
        {
            var d = Load(name);
            Assert.IsNotNull(d, $"{name}.asset not found");
            Assert.AreEqual(4, d.rotationStates.Length, $"{name} should have 4 rotation states");
        }
    }

    [Test]
    public void AllPieces_HaveFourCellsPerRotationState()
    {
        foreach (string name in new[] { "I", "O", "T", "S", "Z", "J", "L" })
        {
            var d = Load(name);
            Assert.IsNotNull(d, $"{name}.asset not found");
            for (int r = 0; r < d.rotationStates.Length; r++)
                Assert.AreEqual(4, d.rotationStates[r].cells.Length,
                    $"{name} rotation {r} should have 4 cells");
        }
    }

    [Test]
    public void IPiece_Rot0_IsHorizontal()
    {
        var cells = Load("I").rotationStates[0].cells;
        int y = cells[0].y;
        foreach (var c in cells)
            Assert.AreEqual(y, c.y, "I Rot0 all cells should share the same Y (horizontal)");
    }

    [Test]
    public void IPiece_Rot1_IsVertical()
    {
        var cells = Load("I").rotationStates[1].cells;
        int x = cells[0].x;
        foreach (var c in cells)
            Assert.AreEqual(x, c.x, "I Rot1 all cells should share the same X (vertical)");
    }

    [Test]
    public void OPiece_AllRotationsIdentical()
    {
        var d = Load("O");
        var rot0 = d.rotationStates[0].cells;
        for (int r = 1; r < 4; r++)
        {
            var cells = d.rotationStates[r].cells;
            for (int i = 0; i < 4; i++)
                Assert.AreEqual(rot0[i], cells[i], $"O rotation {r} differs from Rot0 at cell {i}");
        }
    }

    [Test]
    public void TPiece_Rot0_HasTShape()
    {
        var cells = Load("T").rotationStates[0].cells;
        int rowCount = 0;
        int rowY = 0;
        for (int y = -2; y <= 2; y++)
        {
            int count = 0;
            foreach (var c in cells) if (c.y == y) count++;
            if (count == 3) { rowCount++; rowY = y; }
        }
        Assert.AreEqual(1, rowCount, "T Rot0 should have exactly one row of 3 cells");
        int aboveCount = 0;
        foreach (var c in cells) if (c.y == rowY + 1) aboveCount++;
        Assert.AreEqual(1, aboveCount, "T Rot0 should have 1 cell above the row of 3");
    }

    [Test]
    public void NoDuplicateCells_InAnyRotationState()
    {
        foreach (string name in new[] { "I", "O", "T", "S", "Z", "J", "L" })
        {
            var d = Load(name);
            for (int r = 0; r < d.rotationStates.Length; r++)
            {
                var cells = d.rotationStates[r].cells;
                for (int i = 0; i < cells.Length; i++)
                    for (int j = i + 1; j < cells.Length; j++)
                        Assert.AreNotEqual(cells[i], cells[j],
                            $"{name} rotation {r}: duplicate cells at index {i} and {j}");
            }
        }
    }
}
