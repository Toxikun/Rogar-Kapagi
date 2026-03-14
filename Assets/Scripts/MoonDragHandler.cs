using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Ay objesine eklenir. Sürükleme eventlerini MoonPuzzle'a iletir.
/// </summary>
public class MoonDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector]
    public MoonPuzzle moonPuzzle;

    private bool dragging = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (moonPuzzle == null) return;

        if (moonPuzzle.CanDragMoon())
        {
            dragging = true;
        }
        else
        {
            dragging = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging || moonPuzzle == null) return;
        moonPuzzle.OnMoonDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragging || moonPuzzle == null) return;
        moonPuzzle.OnMoonDragEnd();
        dragging = false;
    }
}
