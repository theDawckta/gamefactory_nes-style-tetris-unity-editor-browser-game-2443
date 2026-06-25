using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PieceController : MonoBehaviour
{
    private static readonly float[] GravityIntervals = {
        0.800f, 0.717f, 0.633f, 0.550f, 0.467f,
        0.383f, 0.300f, 0.217f, 0.133f, 0.100f
    };

    private const float DasDelay = 16f / 60f;
    private const float DasRepeat = 6f / 60f;
    private const float LockDelayDuration = 0.500f;

    public PlayfieldController Playfield;
    public int Level;

    private TetrominoData _data;
    private int _rotation;
    private Vector2Int _pivot;

    private float _gravityTimer;
    private float _dasTimer;
    private float _dasRepeatTimer;
    private bool _dasActive;
    private int _dasDirection;

    private float _lockTimer;
    private bool _grounded;
    private bool _locked;

    public event Action OnPieceLocked;

    public bool IsLocked => _locked;
    public Vector2Int CurrentPivot => _pivot;
    public int CurrentRotation => _rotation;

    public void SpawnPiece(TetrominoData data)
    {
        _data = data;
        _rotation = 0;
        _pivot = new Vector2Int(4, 20);
        _gravityTimer = 0f;
        _dasTimer = 0f;
        _dasRepeatTimer = 0f;
        _dasActive = false;
        _dasDirection = 0;
        _lockTimer = 0f;
        _grounded = false;
        _locked = false;
    }

    public void Tick(float deltaTime)
    {
        if (_data == null || _locked)
            return;

        HandleRotation();
        HandleDas(deltaTime);
        HandleGravity(deltaTime);
        HandleLockDelay(deltaTime);
    }

    private void HandleRotation()
    {
        var kb = Keyboard.current;
        if (kb == null || !kb.upArrowKey.wasPressedThisFrame)
            return;

        int stateCount = _data.rotationStates.Length;
        if (stateCount == 0)
            return;

        int newRot = (_rotation + 1) % stateCount;
        if (Playfield.IsValidPosition(_data, newRot, _pivot))
        {
            _rotation = newRot;
            if (_grounded)
                _lockTimer = 0f;
        }
    }

    private void HandleDas(float deltaTime)
    {
        var kb = Keyboard.current;
        bool leftDown = kb != null && kb.leftArrowKey.isPressed;
        bool rightDown = kb != null && kb.rightArrowKey.isPressed;
        bool leftPressed = kb != null && kb.leftArrowKey.wasPressedThisFrame;
        bool rightPressed = kb != null && kb.rightArrowKey.wasPressedThisFrame;

        int direction = 0;
        if (leftDown && !rightDown) direction = -1;
        else if (rightDown && !leftDown) direction = 1;

        if (direction == 0)
        {
            _dasTimer = 0f;
            _dasRepeatTimer = 0f;
            _dasActive = false;
            _dasDirection = 0;
            return;
        }

        bool justPressed = (direction == -1 && leftPressed) || (direction == 1 && rightPressed);
        if (justPressed || direction != _dasDirection)
        {
            _dasDirection = direction;
            _dasActive = false;
            _dasTimer = 0f;
            _dasRepeatTimer = 0f;
            TryMove(direction);
            return;
        }

        if (!_dasActive)
        {
            _dasTimer += deltaTime;
            if (_dasTimer >= DasDelay)
            {
                _dasActive = true;
                _dasRepeatTimer = 0f;
                TryMove(direction);
            }
        }
        else
        {
            _dasRepeatTimer += deltaTime;
            while (_dasRepeatTimer >= DasRepeat)
            {
                _dasRepeatTimer -= DasRepeat;
                TryMove(direction);
            }
        }
    }

    private void TryMove(int direction)
    {
        Vector2Int newPivot = new Vector2Int(_pivot.x + direction, _pivot.y);
        if (Playfield.IsValidPosition(_data, _rotation, newPivot))
        {
            _pivot = newPivot;
            if (_grounded)
                _lockTimer = 0f;
        }
    }

    private void HandleGravity(float deltaTime)
    {
        var kb = Keyboard.current;
        if (kb != null && kb.downArrowKey.isPressed)
        {
            TryFall();
            return;
        }

        int levelIndex = Mathf.Clamp(Level, 0, GravityIntervals.Length - 1);
        float interval = GravityIntervals[levelIndex];
        _gravityTimer += deltaTime;
        while (_gravityTimer >= interval)
        {
            _gravityTimer -= interval;
            TryFall();
        }
    }

    private void TryFall()
    {
        Vector2Int below = new Vector2Int(_pivot.x, _pivot.y - 1);
        if (Playfield.IsValidPosition(_data, _rotation, below))
        {
            _pivot = below;
            _grounded = false;
            _lockTimer = 0f;
        }
        else
        {
            _grounded = true;
        }
    }

    private void HandleLockDelay(float deltaTime)
    {
        if (!_grounded)
            return;

        // Re-check groundedness — a lateral move may have placed the piece over open space.
        Vector2Int below = new Vector2Int(_pivot.x, _pivot.y - 1);
        if (Playfield.IsValidPosition(_data, _rotation, below))
        {
            _grounded = false;
            _lockTimer = 0f;
            return;
        }

        _lockTimer += deltaTime;
        if (_lockTimer >= LockDelayDuration)
        {
            _locked = true;
            Playfield.LockPiece(_data, _rotation, _pivot);
            OnPieceLocked?.Invoke();
        }
    }
}
