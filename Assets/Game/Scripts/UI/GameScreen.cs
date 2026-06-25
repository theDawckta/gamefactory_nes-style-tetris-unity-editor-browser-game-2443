using UnityEngine;
using UnityEngine.UIElements;

public class GameScreen : BaseScreen
{
    public VisualElement ScoreRegion { get; private set; }
    public VisualElement LevelRegion { get; private set; }
    public VisualElement LinesRegion { get; private set; }
    public VisualElement NextPieceRegion { get; private set; }

    private void Start()
    {
        var root = Document.rootVisualElement;
        if (root == null)
            return;
        ScoreRegion = root.Q("scoreRegion");
        LevelRegion = root.Q("levelRegion");
        LinesRegion = root.Q("linesRegion");
        NextPieceRegion = root.Q("nextPieceRegion");
    }
}
