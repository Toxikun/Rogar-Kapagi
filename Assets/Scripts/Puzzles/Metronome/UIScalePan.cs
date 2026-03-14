using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Puzzles.MetronomeUI
{
    public class UIScalePan : MonoBehaviour, IPointerDownHandler
    {
        public bool isLeftPan;
        public Transform weightPlacementPoint;
        private List<UIWeightInteractable> localWeights = new List<UIWeightInteractable>();

        public float TotalWeight
        {
            get
            {
                float total = 0f;
                foreach (var w in localWeights) total += w.weightValue;
                return total;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            MetronomeUIPuzzleManager manager = MetronomeUIPuzzleManager.Instance;
            if (manager != null && manager.HeldWeight != null)
            {
                // Place the currently held weight into this pan
                PlaceWeight(manager.HeldWeight);
            }
        }

        public void PlaceWeight(UIWeightInteractable weight)
        {
            // Remove from previous pan if it was in one
            if (weight.CurrentPan != null)
            {
                weight.CurrentPan.RemoveWeight(weight);
            }

            localWeights.Add(weight);
            weight.CurrentPan = this;
            
            // visually parent it to the pan's layout/placement point
            weight.transform.SetParent(weightPlacementPoint != null ? weightPlacementPoint : this.transform, false);
            
            MetronomeUIPuzzleManager.Instance.ClearHeldWeight();
            MetronomeUIPuzzleManager.Instance.RecalculateBalance();
        }

        public void RemoveWeight(UIWeightInteractable weight)
        {
            if (localWeights.Contains(weight))
            {
                localWeights.Remove(weight);
                weight.CurrentPan = null;
                // Move it back up higher in hierarchy so it doesn't get masked by the pan
                weight.transform.SetParent(MetronomeUIPuzzleManager.Instance.transform, false); 
            }
        }
    }
}
