using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Puzzles.SoundColor
{
    /// <summary>
    /// Güney Duvarı – Kasa puzzle'ı.
    /// 4 kare var, her tıklamada Mavi→Kırmızı→Yeşil döngüsüyle renk değişir.
    /// Hepsi yeşil olunca ses değişir (sound_example → horn_sound).
    /// Panel kapanınca ses durur.
    /// </summary>
    public class SoundColorPuzzle : MonoBehaviour
    {
        [Header("Squares")]
        public Image[] squares = new Image[4];

        [Header("Audio")]
        public AudioClip backgroundClip;  // sound_example.mp3
        public AudioClip solvedClip;      // horn_sound.mp3

        [Header("UI")]
        public Button closeButton;
        public TextMeshProUGUI winText;

        [Header("State")]
        public bool isSolved = false;

        // Color cycle: Blue(0) → Red(1) → Green(2)
        private static readonly Color[] cycleColors = new Color[]
        {
            new Color(0.2f, 0.4f, 0.9f, 1f),   // Blue
            new Color(0.9f, 0.2f, 0.2f, 1f),   // Red
            new Color(0.2f, 0.8f, 0.3f, 1f),   // Green
        };

        // Index into cycleColors for each square
        private int[] colorIndices = new int[4];

        // Starting indices: Blue(0), Red(1), Red(1), Green(2)
        private static readonly int[] startIndices = { 0, 1, 1, 2 };

        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.loop = true;

            if (closeButton != null)
                closeButton.onClick.AddListener(ClosePuzzle);
        }

        private void OnEnable()
        {
            // Reset puzzle state every time the panel opens
            ResetPuzzle();
            PlayBackgroundAudio();
        }

        private void OnDisable()
        {
            StopAudio();
        }

        private void ResetPuzzle()
        {
            if (isSolved)
            {
                // Don't reset if already solved — keep horn playing on re-open
            }

            for (int i = 0; i < 4; i++)
            {
                colorIndices[i] = startIndices[i];
                if (squares[i] != null)
                    squares[i].color = cycleColors[colorIndices[i]];
            }

            if (winText != null)
                winText.gameObject.SetActive(false);

            isSolved = false;
        }

        private void PlayBackgroundAudio()
        {
            if (_audioSource == null) return;

            if (isSolved && solvedClip != null)
            {
                _audioSource.clip = solvedClip;
            }
            else if (backgroundClip != null)
            {
                _audioSource.clip = backgroundClip;
            }

            _audioSource.loop = true;
            _audioSource.Play();
        }

        private void StopAudio()
        {
            if (_audioSource != null && _audioSource.isPlaying)
                _audioSource.Stop();
        }

        /// <summary>
        /// Called when a square is clicked. Index 0-3.
        /// </summary>
        public void OnSquareClicked(int index)
        {
            if (isSolved) return;
            if (index < 0 || index >= 4) return;

            // Cycle: Blue(0) → Red(1) → Green(2) → Blue(0) → ...
            colorIndices[index] = (colorIndices[index] + 1) % 3;

            if (squares[index] != null)
                squares[index].color = cycleColors[colorIndices[index]];

            CheckWinCondition();
        }

        private void CheckWinCondition()
        {
            // Win when all 4 squares are Green (index 2)
            for (int i = 0; i < 4; i++)
            {
                if (colorIndices[i] != 2) return;
            }

            // All green!
            isSolved = true;
            Debug.Log("Sound Color Puzzle Solved!");

            if (winText != null)
            {
                winText.text = "Senkronize ettin!";
                winText.gameObject.SetActive(true);
            }

            // Switch audio to horn
            if (_audioSource != null && solvedClip != null)
            {
                _audioSource.Stop();
                _audioSource.clip = solvedClip;
                _audioSource.loop = true;
                _audioSource.Play();
            }
        }

        public void ClosePuzzle()
        {
            StopAudio();
            gameObject.SetActive(false);
        }
    }
}
