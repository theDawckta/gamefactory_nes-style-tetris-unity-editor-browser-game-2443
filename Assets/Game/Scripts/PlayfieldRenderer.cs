using UnityEngine;

public class PlayfieldRenderer : MonoBehaviour
{
    private const int Cols = 10;
    private const int VisibleRows = 20;
    private const float CellSize = 24f;
    private const float CellScale = 0.9f;

    public PlayfieldController PlayfieldController;
    public PieceController PieceController;
    public Sprite BlockSprite;

    private static readonly Color[] ColorMap =
    {
        new Color(0f, 0f, 0f, 0f),                         // 0: empty
        new Color(0f, 240f/255f, 240f/255f, 1f),            // 1: I - cyan
        new Color(240f/255f, 240f/255f, 0f, 1f),            // 2: O - yellow
        new Color(160f/255f, 0f, 240f/255f, 1f),            // 3: T - purple
        new Color(0f, 240f/255f, 0f, 1f),                   // 4: S - green
        new Color(240f/255f, 0f, 0f, 1f),                   // 5: Z - red
        new Color(0f, 0f, 240f/255f, 1f),                   // 6: J - blue
        new Color(240f/255f, 160f/255f, 0f, 1f),            // 7: L - orange
    };

    private SpriteRenderer[] _gridRenderers;
    private SpriteRenderer[] _pieceRenderers;

    private void Start()
    {
        float scaledSize = CellSize * CellScale;

        _gridRenderers = new SpriteRenderer[Cols * VisibleRows];
        for (int row = 0; row < VisibleRows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                var go = new GameObject($"Cell_{col}_{row}");
                go.transform.SetParent(transform);
                go.transform.localPosition = GridToLocal(col, row);
                go.transform.localScale = Vector3.one * scaledSize;

                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = BlockSprite;
                sr.color = ColorMap[0];
                _gridRenderers[row * Cols + col] = sr;
            }
        }

        _pieceRenderers = new SpriteRenderer[4];
        for (int i = 0; i < 4; i++)
        {
            var go = new GameObject($"ActivePiece_{i}");
            go.transform.SetParent(transform);
            go.transform.localScale = Vector3.one * scaledSize;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = BlockSprite;
            sr.color = ColorMap[0];
            sr.sortingOrder = 1;
            _pieceRenderers[i] = sr;
        }

        CreateBorders();
    }

    private Vector3 GridToLocal(int col, int row)
    {
        float x = (col - (Cols - 1) * 0.5f) * CellSize;
        float y = (row - (VisibleRows - 1) * 0.5f) * CellSize;
        return new Vector3(x, y, 0f);
    }

    private void CreateBorders()
    {
        float halfW = Cols * CellSize * 0.5f;
        float halfH = VisibleRows * CellSize * 0.5f;
        const float b = 1f;

        CreateBorderQuad("Border_Left",
            new Vector3(-halfW - b * 0.5f, 0f, 0f),
            new Vector3(b, VisibleRows * CellSize + b * 2f, 1f));

        CreateBorderQuad("Border_Right",
            new Vector3(halfW + b * 0.5f, 0f, 0f),
            new Vector3(b, VisibleRows * CellSize + b * 2f, 1f));

        CreateBorderQuad("Border_Bottom",
            new Vector3(0f, -halfH - b * 0.5f, 0f),
            new Vector3(Cols * CellSize, b, 1f));

        CreateBorderQuad("Border_Top",
            new Vector3(0f, halfH + b * 0.5f, 0f),
            new Vector3(Cols * CellSize, b, 1f));
    }

    private void CreateBorderQuad(string name, Vector3 localPosition, Vector3 scale)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform);
        go.transform.localPosition = localPosition;
        go.transform.localScale = scale;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = BlockSprite;
        sr.color = Color.white;
    }

    private void LateUpdate()
    {
        RefreshGrid();
        RefreshActivePiece();
    }

    private void RefreshGrid()
    {
        if (PlayfieldController == null) return;

        for (int row = 0; row < VisibleRows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                int idx = PlayfieldController.GetCell(row, col);
                Color c = idx >= 0 && idx < ColorMap.Length ? ColorMap[idx] : ColorMap[0];
                _gridRenderers[row * Cols + col].color = c;
            }
        }
    }

    private void RefreshActivePiece()
    {
        if (PieceController == null || PieceController.CurrentData == null || PieceController.IsLocked)
        {
            HideActivePiece();
            return;
        }

        TetrominoData data = PieceController.CurrentData;
        int rotation = PieceController.CurrentRotation;
        Vector2Int pivot = PieceController.CurrentPivot;
        Color pieceColor = data.colorIndex >= 0 && data.colorIndex < ColorMap.Length
            ? ColorMap[data.colorIndex]
            : ColorMap[0];

        RotationState state = data.rotationStates[rotation];
        for (int i = 0; i < _pieceRenderers.Length; i++)
        {
            if (i < state.cells.Length)
            {
                int col = pivot.x + state.cells[i].x;
                int row = pivot.y + state.cells[i].y;
                _pieceRenderers[i].transform.localPosition = GridToLocal(col, row);
                _pieceRenderers[i].color = row >= 0 && row < VisibleRows ? pieceColor : ColorMap[0];
            }
            else
            {
                _pieceRenderers[i].color = ColorMap[0];
            }
        }
    }

    private void HideActivePiece()
    {
        for (int i = 0; i < _pieceRenderers.Length; i++)
            _pieceRenderers[i].color = ColorMap[0];
    }
}
