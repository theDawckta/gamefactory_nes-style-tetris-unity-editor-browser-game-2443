using UnityEngine;

[System.Serializable]
public class RotationState
{
    public Vector2Int[] cells = new Vector2Int[4];
}

[CreateAssetMenu(fileName = "TetrominoData", menuName = "Tetris/TetrominoData")]
public class TetrominoData : ScriptableObject
{
    public int colorIndex;
    public RotationState[] rotationStates = new RotationState[4];
}
