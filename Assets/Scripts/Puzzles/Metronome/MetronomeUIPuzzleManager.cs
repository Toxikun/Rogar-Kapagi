using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Puzzles.MetronomeUI
{
    /// <summary>
    /// Manages the entire metronome/scale balancing puzzle.
    /// Two-step interaction:
    ///   Step 1: Player sees a rectangle (the "box"). Clicking it opens the inner puzzle view.
    ///   Step 2: Inside the puzzle view, player moves weights between two pan arms.
    /// </summary>
    public class MetronomeUIPuzzleManager : MonoBehaviour
    {
        public static MetronomeUIPuzzleManager Instance { get; private set; }

        [Header("Two-Step Views")]
        [Tooltip("The outer view: shows the closed box/rectangle")]
        public GameObject outerView;
        [Tooltip("The inner view: shows the scale with pans and weights")]
        public GameObject innerView;

        [Header("Scale Pans")]
        public UIScalePan leftPan;
        public UIScalePan rightPan;

        [Header("Win UI")]
        public TextMeshProUGUI winText;

        [Header("Buttons")]
        public Button closeButton; // close entire puzzle panel
        public Button backButton;  // go from inner view back to outer view
        public Button innerCloseButton;
        public Button boxButton;

        [Header("State")]
        public bool isSolved = false;

        [Header("Metronom")]
        public GameObject metronomTrue;
        public GameObject metronomFalse;
        public UIWeightInteractable HeldWeight { get; private set; }

        private Canvas _parentCanvas;
        private RectTransform _canvasRect;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            // Reset views: show outer, hide inner
            Instance = this;
            if (outerView != null) outerView.SetActive(true);
            if (innerView != null) innerView.SetActive(false);
            if (winText != null) winText.gameObject.SetActive(false);
            HeldWeight = null;
        }

        private void Start()
        {
            // Find the parent canvas for coordinate conversion
            _parentCanvas = GetComponentInParent<Canvas>();
            if (_parentCanvas != null)
                _canvasRect = _parentCanvas.GetComponent<RectTransform>();

            // Wire up buttons following the project pattern (Start binding)
            if (closeButton != null)
                closeButton.onClick.AddListener(ClosePuzzle);
            if (backButton != null)
                backButton.onClick.AddListener(GoBackToOuterView);
            if (innerCloseButton != null)
                innerCloseButton.onClick.AddListener(ClosePuzzle);
            if (boxButton != null)
                boxButton.onClick.AddListener(EnterInnerView);
        }

        private void Update()
        {
            // Make held weight follow mouse
            if (HeldWeight != null && _canvasRect != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvasRect, Input.mousePosition, _parentCanvas.worldCamera, out Vector2 localPoint
                );
                // Parent the held weight to the canvas root so it moves freely
                HeldWeight.GetComponent<RectTransform>().anchoredPosition = localPoint;
            }
        }

        // === TWO-STEP NAVIGATION ===

        /// <summary>Called when the outer box/rectangle is clicked.</summary>
        public void EnterInnerView()
        {
            if (outerView != null) outerView.SetActive(false);
            if (innerView != null) innerView.SetActive(true);
        }

        /// <summary>Called by back button to return to outer view.</summary>
        public void GoBackToOuterView()
        {
            // Cancel any held weight
            if (HeldWeight != null)
            {
                HeldWeight.ReturnToSavedPosition();
                ClearHeldWeight();
            }
            if (innerView != null) innerView.SetActive(false);
            if (outerView != null) outerView.SetActive(true);
        }

        /// <summary>Close the entire puzzle panel.</summary>
        public void ClosePuzzle()
        {
            if (HeldWeight != null)
            {
                HeldWeight.ReturnToSavedPosition();
                ClearHeldWeight();
            }
            gameObject.SetActive(false);
        }

        // === WEIGHT MANAGEMENT ===

        public void HoldWeight(UIWeightInteractable weight)
        {
            HeldWeight = weight;
            // Re-parent to the canvas root so it renders on top and moves freely
            if (_canvasRect != null)
                weight.transform.SetParent(_canvasRect, true);
            weight.transform.SetAsLastSibling();
        }

        public void ClearHeldWeight()
        {
            HeldWeight = null;
        }

        public void RecalculateBalance()
        {
            if (isSolved) return;

            float wLeft = leftPan != null ? leftPan.TotalWeight : 0;
            float wRight = rightPan != null ? rightPan.TotalWeight : 0;

            Debug.Log($"Scale Balance: Left={wLeft}kg, Right={wRight}kg");

            if (wLeft > 0 && wRight > 0 && Mathf.Approximately(wLeft, wRight))
            {
                PuzzleSolved();
            }
        }

        private void PuzzleSolved()
        {
            isSolved = true;
            Debug.Log("Puzzle Solved! Weights are balanced.");

            if (winText != null)
            {
                winText.text = "Senkronize ettin!\n(Şifre: 1...)";
                winText.gameObject.SetActive(true);
            }

            if (GameManager.Instance != null && GameManager.Instance.dialogBox != null)
            {
                GameManager.Instance.dialogBox.Show("Senkronize ettin! (Şifre: 1...)", 4f);
            }

            // Switch metronome visuals: desync → sync
            if (metronomFalse != null) metronomFalse.SetActive(false);
            if (metronomTrue != null)
            {
                metronomTrue.SetActive(true);
                // Set the Animator parameter so MetronomSallanma plays
                Animator trueAnim = metronomTrue.GetComponentInChildren<Animator>();
                if (trueAnim != null)
                    trueAnim.SetBool("metronomDone", true);
            }

            StartCoroutine(CloseAfterDelay(3f));
        }

        private IEnumerator CloseAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            ClosePuzzle();
        }
    }
}
