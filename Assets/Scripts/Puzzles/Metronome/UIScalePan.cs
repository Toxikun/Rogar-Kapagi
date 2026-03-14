using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Puzzles.MetronomeUI
{
    /// <summary>
    /// Attached to each scale pan area (left or right).
    /// When clicked while holding a weight, the weight is placed here.
    /// Weights are laid out horizontally side-by-side on top of the pan bar.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class UIScalePan : MonoBehaviour, IPointerClickHandler
    {
        public bool isLeftPan;

        [Header("Layout")]
        [Tooltip("Horizontal spacing between weight squares")]
        public float weightSpacing = 10f;
        [Tooltip("Pixel offset above the pan bar where weights sit")]
        public float weightYOffset = 10f;

        private List<UIWeightInteractable> _weights = new List<UIWeightInteractable>();
        private RectTransform _rect;

        public float TotalWeight
        {
            get
            {
                float total = 0f;
                foreach (var w in _weights) total += w.weightValue;
                return total;
            }
        }

        public int WeightCount => _weights.Count;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var manager = MetronomeUIPuzzleManager.Instance;
            if (manager == null || manager.isSolved) return;

            if (manager.HeldWeight != null)
            {
                PlaceWeight(manager.HeldWeight);
                manager.ClearHeldWeight();
                manager.RecalculateBalance();
            }
        }

        /// <summary>
        /// Add a weight to this pan and re-layout all weights.
        /// </summary>
        public void PlaceWeight(UIWeightInteractable weight)
        {
            if (_weights.Contains(weight)) return;

            _weights.Add(weight);
            weight.CurrentPan = this;

            // Parent the weight under this pan
            weight.transform.SetParent(this.transform, false);
            
            LayoutWeights();
        }

        /// <summary>
        /// Remove a weight from this pan but don't re-parent it (caller handles that).
        /// </summary>
        public void RemoveWeight(UIWeightInteractable weight)
        {
            if (_weights.Remove(weight))
            {
                weight.CurrentPan = null;
                LayoutWeights();
            }
        }

        /// <summary>
        /// Position all weights side-by-side horizontally, centered on the pan, sitting on top.
        /// </summary>
        private void LayoutWeights()
        {
            if (_weights.Count == 0) return;

            float panHeight = _rect.rect.height;

            // Calculate total width of all weights + spacing
            float totalWidth = 0f;
            for (int i = 0; i < _weights.Count; i++)
            {
                var wRect = _weights[i].GetComponent<RectTransform>();
                totalWidth += wRect.rect.width;
                if (i < _weights.Count - 1) totalWidth += weightSpacing;
            }

            // Start position (left edge, centered)
            float startX = -totalWidth / 2f;
            float currentX = startX;

            for (int i = 0; i < _weights.Count; i++)
            {
                var wRect = _weights[i].GetComponent<RectTransform>();
                float halfW = wRect.rect.width / 2f;
                float halfH = wRect.rect.height / 2f;

                // Position: centered horizontally, sitting on top of the pan bar
                wRect.anchorMin = new Vector2(0.5f, 1f); // top-center anchor
                wRect.anchorMax = new Vector2(0.5f, 1f);
                wRect.anchoredPosition = new Vector2(
                    currentX + halfW,
                    weightYOffset + halfH
                );

                currentX += wRect.rect.width + weightSpacing;
            }
        }
    }
}
