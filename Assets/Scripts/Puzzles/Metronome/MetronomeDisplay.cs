using UnityEngine;
using UnityEngine.UI;
using Puzzles.MetronomeUI;

/// <summary>
/// Code-driven metronome sprite animation for UI.
/// Cycles through sprite frames to create a pendulum animation.
/// Uses desync pattern (asymmetric swing) before puzzle is solved,
/// and sync pattern (symmetric swing) after puzzle is solved.
/// </summary>
public class MetronomeDisplay : MonoBehaviour
{
    [Header("Sprites (ordered -6 to 6)")]
    [Tooltip("All metronome frame sprites in order: -6,-5,-4,-3,-2,-1,0,1,2,3,4,5,6")]
    public Sprite[] frames;

    [Header("Display")]
    public Image displayImage;

    [Header("Animation")]
    [Tooltip("Seconds per frame")]
    public float frameInterval = 0.083333f;

    private int _currentIndex = 0;
    private float _timer = 0f;
    private bool _wasSolved = false;

    // Desync: Derived from MetronomDesync.anim
    // Frame indices: 0=-6 ... 6=0 ... 12=6
    private static readonly int[] desyncSequence = new int[]
    {
        6, 7, 8, 7, 6, 5, 4, 3, 2, 1, 0, 1, 2, 3, 4, 5
    };

    // Sync: Derived from MetronomSallanma.anim
    private static readonly int[] syncSequence = new int[]
    {
        6, 7, 8, 9, 10, 11, 12, 11, 10, 9, 8, 7, 
        6, 5, 4, 3, 2, 1, 0, 1, 2, 3, 4, 5
    };

    private int[] _activeSequence;

    private void OnEnable()
    {
        _currentIndex = 0;
        _timer = 0f;
        UpdateSequence();
        UpdateFrame();
    }

    private void Update()
    {
        if (frames == null || frames.Length == 0 || displayImage == null) return;

        // Check if solved state changed
        bool solved = IsPuzzleSolved();
        if (solved != _wasSolved)
        {
            _wasSolved = solved;
            _currentIndex = 0;
            UpdateSequence();
        }

        _timer += Time.deltaTime;
        if (_timer >= frameInterval)
        {
            _timer -= frameInterval;
            _currentIndex = (_currentIndex + 1) % _activeSequence.Length;
            UpdateFrame();
        }
    }

    private void UpdateSequence()
    {
        _wasSolved = IsPuzzleSolved();
        _activeSequence = _wasSolved ? syncSequence : desyncSequence;
    }

    private void UpdateFrame()
    {
        if (_activeSequence == null || frames == null || displayImage == null) return;

        int frameIdx = _activeSequence[_currentIndex % _activeSequence.Length];
        if (frameIdx >= 0 && frameIdx < frames.Length)
        {
            displayImage.sprite = frames[frameIdx];
        }
    }

    private bool IsPuzzleSolved()
    {
        var mgr = MetronomeUIPuzzleManager.Instance;
        return mgr != null && mgr.isSolved;
    }
}
