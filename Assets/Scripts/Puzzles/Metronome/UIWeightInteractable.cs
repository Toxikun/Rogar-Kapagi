using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Puzzles.MetronomeUI
{
    /// <summary>
    /// Attached to each weight square (UI Image).
    /// Click to pick up, click on a pan area to place.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class UIWeightInteractable : MonoBehaviour, IPointerClickHandler
    {
        public float weightValue = 1f;
        
        [HideInInspector]
        public UIScalePan CurrentPan;

        private RectTransform _rect;
        private Vector2 _originalAnchoredPos;
        private Transform _originalParent;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        /// <summary>
        /// Save position so we can return if cancelled.
        /// </summary>
        public void SavePosition()
        {
            _originalAnchoredPos = _rect.anchoredPosition;
            _originalParent = transform.parent;
        }

        /// <summary>
        /// Return to saved position (cancel pickup).
        /// </summary>
        public void ReturnToSavedPosition()
        {
            transform.SetParent(_originalParent, false);
            _rect.anchoredPosition = _originalAnchoredPos;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var manager = MetronomeUIPuzzleManager.Instance;
            if (manager == null || manager.isSolved) return;

            if (manager.HeldWeight == this)
            {
                // Cancel: put it back
                ReturnToSavedPosition();
                manager.ClearHeldWeight();
            }
            else if (manager.HeldWeight == null)
            {
                // Pick up this weight
                SavePosition();
                if (CurrentPan != null)
                {
                    CurrentPan.RemoveWeight(this);
                }
                manager.HoldWeight(this);
            }
            // If holding a different weight and clicking another weight, ignore
        }
    }
}
