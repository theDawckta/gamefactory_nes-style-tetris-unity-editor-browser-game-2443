using UnityEngine;

[CreateAssetMenu(fileName = "TetrominoData", menuName = "Tetris/TetrominoData")]
public class TetrominoData : ScriptableObject
{
    public int colorIndex;
    public RotationState[] rotationStates;
}

[System.Serializable]
public class RotationState
{
    public Vector2Int[] cells;
}
