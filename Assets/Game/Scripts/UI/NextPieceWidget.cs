using UnityEngine;

public class NextPieceWidget : MonoBehaviour
{
    private const float CellSize = 24f;
    private const float CellScale = 0.9f;
    private const int GridSize = 4;

    public GameplayController GameplayController;
    public GameScreen GameScreen;
    public Sprite BlockSprite;

    private static readonly Color[] ColorMap =
    {
        new Color(0f, 0f, 0f, 0f),
        new Color(0f, 240f/255f, 240f/255f, 1f),
        new Color(240f/255f, 240f/255f, 0f, 1f),
        new Color(160f/255f, 0f, 240f/255f, 1f),
        new Color(0f, 240f/255f, 0f, 1f),
        new Color(240f/255f, 0f, 0f, 1f),
        new Color(0f, 0f, 240f/255f, 1f),
        new Color(240f/255f, 160f/255f, 0f, 1f),
    };

    private SpriteRenderer[] _cells;

    private void Start()
    {
        float scaledSize = CellSize * CellScale;
        _cells = new SpriteRenderer[GridSize * GridSize];

        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                var go = new GameObject($"Preview_{col}_{row}");
                go.transform.SetParent(transform);
                float lx = (col - (GridSize - 1) * 0.5f) * CellSize;
                float ly = (row - (GridSize - 1) * 0.5f) * CellSize;
                go.transform.localPosition = new Vector3(lx, ly, 0f);
                go.transform.localScale = Vector3.one * scaledSize;

                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = BlockSprite;
                sr.color = ColorMap[0];
                _cells[row * GridSize + col] = sr;
            }
        }
    }

    private void OnEnable()
    {
        if (GameplayController != null)
            GameplayController.OnNextPieceChanged += HandleNextPieceChanged;
    }

    private void OnDisable()
    {
        if (GameplayController != null)
            GameplayController.OnNextPieceChanged -= HandleNextPieceChanged;
    }

    private void LateUpdate()
    {
        AlignToNextPieceRegion();
    }

    private void AlignToNextPieceRegion()
    {
        if (GameScreen == null || GameScreen.NextPieceRegion == null || Camera.main == null)
            return;

        var regionBound = GameScreen.NextPieceRegion.worldBound;
        if (regionBound.width <= 0f || regionBound.height <= 0f) return;

        var panelRoot = GameScreen.NextPieceRegion.panel?.visualTree;
        if (panelRoot == null) return;
        var panelBound = panelRoot.worldBound;
        if (panelBound.width <= 0f || panelBound.height <= 0f) return;

        float normalizedX = regionBound.center.x / panelBound.width;
        float normalizedY = regionBound.center.y / panelBound.height;
        float screenX = normalizedX * Screen.width;
        float screenY = (1f - normalizedY) * Screen.height;

        float depth = -Camera.main.transform.position.z;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenX, screenY, depth));
        transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
    }

    private void HandleNextPieceChanged(TetrominoData data)
    {
        if (_cells == null) return;

        for (int i = 0; i < _cells.Length; i++)
            _cells[i].color = ColorMap[0];

        if (data == null || data.rotationStates == null || data.rotationStates.Length == 0)
            return;

        Color pieceColor = data.colorIndex >= 0 && data.colorIndex < ColorMap.Length
            ? ColorMap[data.colorIndex]
            : ColorMap[0];

        RotationState state = data.rotationStates[0];
        foreach (var cell in state.cells)
        {
            // +1 offset centers each piece's rotation-0 offsets within the 4x4 grid
            int gridCol = cell.x + 1;
            int gridRow = cell.y + 1;
            if (gridCol >= 0 && gridCol < GridSize && gridRow >= 0 && gridRow < GridSize)
                _cells[gridRow * GridSize + gridCol].color = pieceColor;
        }
    }
}
