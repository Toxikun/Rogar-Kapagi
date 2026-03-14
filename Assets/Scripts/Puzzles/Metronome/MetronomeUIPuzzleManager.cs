using UnityEngine;
using TMPro;
using System.Collections;

namespace Puzzles.MetronomeUI
{
    public class MetronomeUIPuzzleManager : MonoBehaviour
    {
        public static MetronomeUIPuzzleManager Instance { get; private set; }

        [Header("Components")]
        public UIScalePan leftPan;
        public UIScalePan rightPan;
        public TextMeshProUGUI winText;

        [Header("Puzzle State")]
        public bool isSolved = false;
        public float targetWeight = 4f;

        public UIWeightInteractable HeldWeight { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (winText != null) winText.gameObject.SetActive(false);
            RecalculateBalance();
        }

        private void Update()
        {
            // If dragging, make it follow mouse within Canvas
            if (HeldWeight != null)
            {
                HeldWeight.transform.position = Input.mousePosition;
            }
        }

        public void HoldWeight(UIWeightInteractable weight)
        {
            HeldWeight = weight;
            // Bring to front 
            weight.transform.SetAsLastSibling();
        }

        public void ClearHeldWeight()
        {
            HeldWeight = null;
        }

        public void RecalculateBalance()
        {
            if (isSolved) return;

            float wLeft = leftPan.TotalWeight;
            float wRight = rightPan.TotalWeight;

            // Check win condition
            if (Mathf.Approximately(wLeft, targetWeight) && Mathf.Approximately(wRight, targetWeight))
            {
                PuzzleSolved();
            }
        }

        private void PuzzleSolved()
        {
            isSolved = true;
            Debug.Log("UI Metronome Puzzle Solved!");
            
            if (winText != null)
            {
                winText.text = "Senkronize ettin!";
                winText.gameObject.SetActive(true);
            }
            
            // Wait 2 seconds and close panel, unlock door or give item
            StartCoroutine(CloseAfterDelay(2.5f));
        }

        private IEnumerator CloseAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            this.gameObject.SetActive(false);
        }
    }
}
