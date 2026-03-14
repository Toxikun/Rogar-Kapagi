using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Puzzles.MetronomeUI
{
    public class UIWeightInteractable : MonoBehaviour, IPointerDownHandler
    {
        public float weightValue = 1f;
        public UIScalePan CurrentPan { get; set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            MetronomeUIPuzzleManager manager = MetronomeUIPuzzleManager.Instance;
            if (manager == null) return;

            // Pick up the weight if it's not already held
            if (manager.HeldWeight == this)
            {
                // Drop it back (cancel pick up)
                manager.ClearHeldWeight();
            }
            else if (manager.HeldWeight == null)
            {
                // Remove from pan logic happens on pickup visually
                if (CurrentPan != null)
                {
                    CurrentPan.RemoveWeight(this);
                    manager.RecalculateBalance();
                }
                manager.HoldWeight(this);
            }
        }
    }
}
