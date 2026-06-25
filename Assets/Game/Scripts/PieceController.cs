using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PieceController : MonoBehaviour
{
    private static readonly float[] GravityIntervals =
    {
        0.800f, 0.717f, 0.633f, 0.550f, 0.467f, 0.383f, 0.300f, 0.217f, 0.133f, 0.100f
    };

    private const float DasInitialDelay = 16f / 60f;
    private const float DasRepeatInterval = 6f / 60f;
    private const float LockDelayDuration = 0.500f;

    public PlayfieldController Playfield;
    public int Level;

    private TetrominoData _data;
    private Vector2Int _pivot;
    private int _rotation;
    private bool _isLocked;
    private bool _hasPiece;
    private float _gravityTimer;
    private float _dasLeftTimer;
    private float _dasRightTimer;
    private bool _isGrounded;
    private float _lockTimer;

    public event Action OnPieceLocked;

    public bool IsLocked => _isLocked;
    public Vector2Int CurrentPivot => _pivot;
    public int CurrentRotation => _rotation;

    public void SpawnPiece(TetrominoData data)
    {
        _data = data;
        _pivot = new Vector2Int(4, 20);
        _rotation = 0;
        _isLocked = false;
        _hasPiece = true;
        _gravityTimer = 0f;
        _dasLeftTimer = -DasInitialDelay;
        _dasRightTimer = -DasInitialDelay;
        _isGrounded = false;
        _lockTimer = 0f;
    }

    public void Tick(float deltaTime)
    {
        if (!_hasPiece || _isLocked)
            return;

        HandleDas(deltaTime);
        HandleSoftDropAndGravity(deltaTime);
        HandleRotation();
        HandleLockDelay(deltaTime);
    }

    private float GetGravityInterval()
    {
        int idx = Mathf.Clamp(Level, 0, GravityIntervals.Length - 1);
        return GravityIntervals[idx];
    }

    private void HandleDas(float deltaTime)
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.leftArrowKey.wasPressedThisFrame)
        {
            TryMove(-1);
            _dasLeftTimer = -DasInitialDelay;
        }
        else if (keyboard.leftArrowKey.isPressed)
        {
            _dasLeftTimer += deltaTime;
            if (_dasLeftTimer >= 0f)
            {
                TryMove(-1);
                _dasLeftTimer = -DasRepeatInterval;
            }
        }
        else
        {
            _dasLeftTimer = -DasInitialDelay;
        }

        if (keyboard.rightArrowKey.wasPressedThisFrame)
        {
            TryMove(1);
            _dasRightTimer = -DasInitialDelay;
        }
        else if (keyboard.rightArrowKey.isPressed)
        {
            _dasRightTimer += deltaTime;
            if (_dasRightTimer >= 0f)
            {
                TryMove(1);
                _dasRightTimer = -DasRepeatInterval;
            }
        }
        else
        {
            _dasRightTimer = -DasInitialDelay;
        }
    }

    private void HandleSoftDropAndGravity(float deltaTime)
    {
        var keyboard = Keyboard.current;
        bool softDrop = keyboard != null && keyboard.downArrowKey.isPressed;

        if (softDrop)
        {
            TryFall();
            _gravityTimer = 0f;
        }
        else
        {
            _gravityTimer += deltaTime;
            if (_gravityTimer >= GetGravityInterval())
            {
                TryFall();
                _gravityTimer = 0f;
            }
        }
    }

    private void HandleRotation()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.upArrowKey.wasPressedThisFrame)
        {
            int nextRotation = (_rotation + 1) % _data.rotationStates.Length;
            if (Playfield.IsValidPosition(_data, nextRotation, _pivot))
            {
                _rotation = nextRotation;
                if (_isGrounded)
                    _lockTimer = 0f;
            }
        }
    }

    private void HandleLockDelay(float deltaTime)
    {
        // Re-check groundedness with current position so sliding off a block resumes falling.
        _isGrounded = !Playfield.IsValidPosition(_data, _rotation, new Vector2Int(_pivot.x, _pivot.y - 1));

        if (_isGrounded)
        {
            _lockTimer += deltaTime;
            if (_lockTimer >= LockDelayDuration)
                LockPiece();
        }
        else
        {
            _lockTimer = 0f;
        }
    }

    private bool TryMove(int direction)
    {
        var newPivot = new Vector2Int(_pivot.x + direction, _pivot.y);
        if (Playfield.IsValidPosition(_data, _rotation, newPivot))
        {
            _pivot = newPivot;
            if (_isGrounded)
                _lockTimer = 0f;
            return true;
        }
        return false;
    }

    private bool TryFall()
    {
        var newPivot = new Vector2Int(_pivot.x, _pivot.y - 1);
        if (Playfield.IsValidPosition(_data, _rotation, newPivot))
        {
            _pivot = newPivot;
            return true;
        }
        return false;
    }

    private void LockPiece()
    {
        Playfield.LockPiece(_data, _rotation, _pivot);
        _isLocked = true;
        _hasPiece = false;
        OnPieceLocked?.Invoke();
    }
}
