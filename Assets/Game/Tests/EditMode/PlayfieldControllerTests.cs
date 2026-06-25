using NUnit.Framework;
using UnityEngine;

public class PlayfieldControllerTests
{
    private GameObject _go;
    private PlayfieldController _controller;

    private TetrominoData MakePiece(int colorIndex, Vector2Int[] cells)
    {
        var data = ScriptableObject.CreateInstance<TetrominoData>();
        data.colorIndex = colorIndex;
        data.rotationStates = new RotationState[]
        {
            new RotationState { cells = cells }
        };
        return data;
    }

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject();
        _controller = _go.AddComponent<PlayfieldController>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_go);
    }

    [Test]
    public void GetCell_InitiallyZero()
    {
        Assert.AreEqual(0, _controller.GetCell(0, 0));
        Assert.AreEqual(0, _controller.GetCell(10, 5));
        Assert.AreEqual(0, _controller.GetCell(21, 9));
    }

    [Test]
    public void SetCell_StoresAndRetrievesValue()
    {
        _controller.SetCell(3, 4, 5);
        Assert.AreEqual(5, _controller.GetCell(3, 4));
    }

    [Test]
    public void ClearAll_ResetsAllCells()
    {
        _controller.SetCell(0, 0, 1);
        _controller.SetCell(10, 5, 3);
        _controller.SetCell(21, 9, 7);
        _controller.ClearAll();
        Assert.AreEqual(0, _controller.GetCell(0, 0));
        Assert.AreEqual(0, _controller.GetCell(10, 5));
        Assert.AreEqual(0, _controller.GetCell(21, 9));
    }

    [Test]
    public void IsValidPosition_ReturnsTrueForEmptyGrid()
    {
        var piece = MakePiece(1, new[] { Vector2Int.zero, Vector2Int.up, Vector2Int.right, new Vector2Int(1, 1) });
        var pivot = new Vector2Int(3, 3);
        Assert.IsTrue(_controller.IsValidPosition(piece, 0, pivot));
    }

    [Test]
    public void IsValidPosition_ReturnsFalseWhenOutOfBoundsLeft()
    {
        var piece = MakePiece(1, new[] { new Vector2Int(-1, 0), Vector2Int.zero, Vector2Int.up, Vector2Int.right });
        var pivot = new Vector2Int(0, 3);
        Assert.IsFalse(_controller.IsValidPosition(piece, 0, pivot));
    }

    [Test]
    public void IsValidPosition_ReturnsFalseWhenOutOfBoundsRight()
    {
        var piece = MakePiece(1, new[] { new Vector2Int(1, 0), Vector2Int.zero, Vector2Int.up, Vector2Int.down });
        var pivot = new Vector2Int(9, 3);
        Assert.IsFalse(_controller.IsValidPosition(piece, 0, pivot));
    }

    [Test]
    public void IsValidPosition_ReturnsFalseWhenOutOfBoundsBottom()
    {
        var piece = MakePiece(1, new[] { new Vector2Int(0, -1), Vector2Int.zero, Vector2Int.up, Vector2Int.right });
        var pivot = new Vector2Int(3, 0);
        Assert.IsFalse(_controller.IsValidPosition(piece, 0, pivot));
    }

    [Test]
    public void IsValidPosition_ReturnsFalseWhenOverlapsLockedCell()
    {
        _controller.SetCell(3, 3, 2);
        var piece = MakePiece(1, new[] { Vector2Int.zero, Vector2Int.up, Vector2Int.right, new Vector2Int(1, 1) });
        var pivot = new Vector2Int(3, 3);
        Assert.IsFalse(_controller.IsValidPosition(piece, 0, pivot));
    }

    [Test]
    public void LockPiece_WritesCellsToGrid()
    {
        var cells = new[] { Vector2Int.zero, Vector2Int.up, Vector2Int.right, new Vector2Int(1, 1) };
        var piece = MakePiece(3, cells);
        var pivot = new Vector2Int(2, 2);
        _controller.LockPiece(piece, 0, pivot);
        Assert.AreEqual(3, _controller.GetCell(2, 2));
        Assert.AreEqual(3, _controller.GetCell(3, 2));
        Assert.AreEqual(3, _controller.GetCell(2, 3));
        Assert.AreEqual(3, _controller.GetCell(3, 3));
    }

    [Test]
    public void ClearLines_ReturnsEmptyArrayWhenNoFullRows()
    {
        _controller.SetCell(0, 0, 1);
        int[] cleared = _controller.ClearLines();
        Assert.AreEqual(0, cleared.Length);
    }

    [Test]
    public void ClearLines_ClearsOneFullRow()
    {
        for (int col = 0; col < 10; col++)
            _controller.SetCell(0, col, 1);
        _controller.SetCell(1, 0, 2);

        int[] cleared = _controller.ClearLines();

        Assert.AreEqual(1, cleared.Length);
        Assert.AreEqual(0, cleared[0]);
        // Row 1 (with marker at col 0) shifts down to row 0 after clear
        Assert.AreEqual(2, _controller.GetCell(0, 0));
        for (int col = 1; col < 10; col++)
            Assert.AreEqual(0, _controller.GetCell(0, col));
    }

    [Test]
    public void ClearLines_ShiftsRowsDownCorrectly()
    {
        // Fill row 0 and row 2 fully; put marker in row 1
        for (int col = 0; col < 10; col++)
            _controller.SetCell(0, col, 1);
        _controller.SetCell(1, 5, 7);
        for (int col = 0; col < 10; col++)
            _controller.SetCell(2, col, 1);

        int[] cleared = _controller.ClearLines();

        Assert.AreEqual(2, cleared.Length);
        // After clearing rows 0 and 2, old row 1 (with marker at col 5) shifts to row 0
        Assert.AreEqual(7, _controller.GetCell(0, 5));
        // Everything above should be empty
        Assert.AreEqual(0, _controller.GetCell(1, 5));
    }

    [Test]
    public void ClearLines_ClearsFourFullRows()
    {
        for (int row = 0; row < 4; row++)
            for (int col = 0; col < 10; col++)
                _controller.SetCell(row, col, row + 1);

        _controller.SetCell(4, 0, 5);

        int[] cleared = _controller.ClearLines();

        Assert.AreEqual(4, cleared.Length);
        Assert.AreEqual(5, _controller.GetCell(0, 0));
        Assert.AreEqual(0, _controller.GetCell(1, 0));
    }

    [Test]
    public void IsValidPosition_ReturnsTrueAtTopBufferRows()
    {
        var piece = MakePiece(1, new[] { Vector2Int.zero, Vector2Int.up, Vector2Int.right, new Vector2Int(1, 1) });
        var pivot = new Vector2Int(3, 20);
        Assert.IsTrue(_controller.IsValidPosition(piece, 0, pivot));
    }
}
