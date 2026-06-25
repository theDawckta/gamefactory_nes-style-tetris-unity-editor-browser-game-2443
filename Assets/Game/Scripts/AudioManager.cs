using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource _musicSource;
    private AudioSource _sfxSource;

    private readonly Dictionary<string, AudioClip> _musicClips = new Dictionary<string, AudioClip>();
    private readonly Dictionary<string, AudioClip> _sfxClips = new Dictionary<string, AudioClip>();

    private int _prevLines;
    private int _prevLevel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = true;
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.loop = false;
    }

    private void Start()
    {
        LoadClips();
        WireEvents();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void LoadClips()
    {
        _musicClips["gameplay_theme"] = Resources.Load<AudioClip>("Music/gameplay_theme");
        _musicClips["start_screen_theme"] = Resources.Load<AudioClip>("Music/start_screen_theme");
        _sfxClips["piece_lock"] = Resources.Load<AudioClip>("SFX/piece_lock");
        _sfxClips["line_clear"] = Resources.Load<AudioClip>("SFX/line_clear");
        _sfxClips["tetris_clear"] = Resources.Load<AudioClip>("SFX/tetris_clear");
        _sfxClips["level_up"] = Resources.Load<AudioClip>("SFX/level_up");
        _sfxClips["game_over"] = Resources.Load<AudioClip>("SFX/game_over");
        _sfxClips["menu_move"] = Resources.Load<AudioClip>("SFX/menu_move");
        _sfxClips["menu_confirm"] = Resources.Load<AudioClip>("SFX/menu_confirm");
        _sfxClips["start_game"] = Resources.Load<AudioClip>("SFX/start_game");
    }

    private void WireEvents()
    {
        var startScreen = Object.FindAnyObjectByType<StartScreen>();
        if (startScreen != null)
        {
            startScreen.OnStartPressed += HandleStartPressed;
            startScreen.OnShown += HandleStartScreenShown;
        }

        var gameplay = Object.FindAnyObjectByType<GameplayController>();
        if (gameplay != null)
        {
            gameplay.OnGameOver += HandleGameOver;
            if (gameplay.Scoring != null)
            {
                _prevLines = gameplay.Scoring.TotalLines;
                _prevLevel = gameplay.Scoring.Level;
                gameplay.Scoring.OnStatsChanged += HandleStatsChanged;
            }
        }

        var piece = Object.FindAnyObjectByType<PieceController>();
        if (piece != null)
            piece.OnPieceLocked += HandlePieceLocked;

        var initials = Object.FindAnyObjectByType<InitialsEntryWidget>();
        if (initials != null)
        {
            initials.OnCharCycled += HandleMenuMove;
            initials.OnSlotConfirmed += HandleMenuConfirm;
        }
    }

    private void HandleStartPressed()
    {
        PlaySFX("start_game");
        PlayMusic("gameplay_theme");
    }

    private void HandleStartScreenShown()
    {
        PlayMusic("start_screen_theme");
    }

    private void HandleGameOver()
    {
        StopMusic();
        PlaySFX("game_over");
    }

    private void HandlePieceLocked()
    {
        PlaySFX("piece_lock");
    }

    private void HandleStatsChanged(int score, int level, int totalLines)
    {
        int linesAdded = totalLines - _prevLines;
        bool levelIncreased = level > _prevLevel;

        if (linesAdded == 4)
            PlaySFX("tetris_clear");
        else if (linesAdded > 0)
            PlaySFX("line_clear");

        if (levelIncreased)
            PlaySFX("level_up");

        _prevLines = totalLines;
        _prevLevel = level;
    }

    private void HandleMenuMove() => PlaySFX("menu_move");
    private void HandleMenuConfirm() => PlaySFX("menu_confirm");

    public void PlayMusic(string name)
    {
        if (_musicClips.TryGetValue(name, out var clip) && clip != null)
        {
            _musicSource.Stop();
            _musicSource.clip = clip;
            _musicSource.Play();
        }
    }

    public void StopMusic()
    {
        _musicSource.Stop();
    }

    public void PlaySFX(string name)
    {
        if (_sfxClips.TryGetValue(name, out var clip) && clip != null)
            _sfxSource.PlayOneShot(clip);
    }
}
