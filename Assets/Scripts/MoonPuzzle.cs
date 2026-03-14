using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

/// <summary>
/// Doğu duvarındaki pencere puzzle'ı.
/// Pencereye tıklayınca açılan panelde ay objesi sürüklenerek
/// çerçeve dışına çıkarılır. Olta envanterde seçili olmalıdır.
/// Ay çerçeve dışına çıkınca fade-out olur ve envantere düşer.
/// </summary>
public class MoonPuzzle : MonoBehaviour
{
    [Header("Panel")]
    public GameObject puzzlePanel;

    [Header("Moon")]
    public Image moonImage;
    public RectTransform moonRect;

    [Header("Window Frame")]
    public RectTransform windowFrame;

    [Header("Gradient Overlay")]
    public Image gradientOverlay;

    [Header("Olta Toggle (on East Wall)")]
    public Button toggleOnButton;
    public Button toggleOffButton;
    public TextMeshProUGUI toggleStatusText;

    [Header("Puzzle Buttons")]
    public Button backButton;

    [Header("State")]
    public bool hasOlta = false;
    public bool moonRemoved = false;

    private bool isDragging = false;
    private Vector2 moonStartPos;
    private Canvas parentCanvas;
    private RectTransform panelRect;

    private void Start()
    {
        // Olta ON/OFF
        if (toggleOnButton != null)
            toggleOnButton.onClick.AddListener(TurnOltaOn);
        if (toggleOffButton != null)
            toggleOffButton.onClick.AddListener(TurnOltaOff);

        // Back button
        if (backButton != null)
            backButton.onClick.AddListener(ClosePuzzle);

        // Puzzle panel starts hidden
        if (puzzlePanel != null)
            puzzlePanel.SetActive(false);

        // Cache canvas
        parentCanvas = GetComponentInParent<Canvas>();
        if (puzzlePanel != null)
            panelRect = puzzlePanel.GetComponent<RectTransform>();

        // Save moon start position
        if (moonRect != null)
            moonStartPos = moonRect.anchoredPosition;

        UpdateToggleUI();
    }

    // ===== PUZZLE AÇ/KAPAT =====
    public void OpenPuzzle()
    {
        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(true);

            // Reset moon if not already removed
            if (!moonRemoved && moonRect != null)
            {
                moonRect.anchoredPosition = moonStartPos;
                SetMoonAlpha(1f);
                SetGradientAlpha(1f);
            }
        }
    }

    public void ClosePuzzle()
    {
        if (puzzlePanel != null)
            puzzlePanel.SetActive(false);
    }

    // ===== OLTA ON/OFF =====
    private void TurnOltaOn()
    {
        hasOlta = true;
        GameManager gm = GameManager.Instance;
        if (gm != null && gm.inventory != null)
            gm.inventory.AddItem("Olta");
        UpdateToggleUI();
    }

    private void TurnOltaOff()
    {
        hasOlta = false;
        GameManager gm = GameManager.Instance;
        if (gm != null && gm.inventory != null)
            gm.inventory.RemoveItem("Olta");
        UpdateToggleUI();
    }

    private void UpdateToggleUI()
    {
        if (toggleStatusText != null)
            toggleStatusText.text = hasOlta ? "Olta: AÇIK" : "Olta: KAPALI";
    }

    // ===== MOON DRAG (called by MoonDragHandler) =====
    public bool CanDragMoon()
    {
        if (moonRemoved) return false;

        GameManager gm = GameManager.Instance;
        if (gm == null || gm.inventory == null) return false;

        string selected = gm.inventory.GetSelectedItem();
        if (selected != "Olta")
        {
            gm.dialogBox.Show("Ay'ı çekmek için oltanı seç!", 2f);
            return false;
        }
        return true;
    }

    public void OnMoonDrag(PointerEventData eventData)
    {
        if (moonRect == null || parentCanvas == null) return;

        // Move moon with pointer
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panelRect, eventData.position, parentCanvas.worldCamera, out localPoint);
        moonRect.anchoredPosition = localPoint;

        // Check if moon is outside window frame
        if (!IsMoonInsideFrame())
        {
            // Calculate how far outside (for gradual fade)
            float dist = GetDistanceOutsideFrame();
            float fadeStart = 20f;
            float fadeEnd = 150f;
            float t = Mathf.Clamp01((dist - fadeStart) / (fadeEnd - fadeStart));
            SetMoonAlpha(1f - t);
            SetGradientAlpha(1f - t * 0.8f);
        }
        else
        {
            SetMoonAlpha(1f);
            SetGradientAlpha(1f);
        }
    }

    public void OnMoonDragEnd()
    {
        if (moonRemoved) return;

        if (!IsMoonInsideFrame() && GetDistanceOutsideFrame() > 100f)
        {
            // Moon is outside frame enough → complete the puzzle
            StartCoroutine(MoonFadeOutAndCollect());
        }
        else
        {
            // Snap back
            if (moonRect != null)
                moonRect.anchoredPosition = moonStartPos;
            SetMoonAlpha(1f);
            SetGradientAlpha(1f);
        }
    }

    private IEnumerator MoonFadeOutAndCollect()
    {
        moonRemoved = true;
        float duration = 0.6f;
        float elapsed = 0f;

        float startMoonAlpha = moonImage != null ? moonImage.color.a : 1f;
        float startGradAlpha = gradientOverlay != null ? gradientOverlay.color.a : 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            SetMoonAlpha(Mathf.Lerp(startMoonAlpha, 0f, t));
            SetGradientAlpha(Mathf.Lerp(startGradAlpha, 0f, t));

            yield return null;
        }

        SetMoonAlpha(0f);
        SetGradientAlpha(0f);

        if (moonRect != null)
            moonRect.gameObject.SetActive(false);

        // Add moon to inventory
        GameManager gm = GameManager.Instance;
        if (gm != null && gm.inventory != null)
        {
            gm.inventory.AddItem("Ay");
            gm.dialogBox.Show("Ay'ı pencereden çektin! Envantere eklendi.", 3f);
        }
    }

    // ===== HELPERS =====
    private bool IsMoonInsideFrame()
    {
        if (moonRect == null || windowFrame == null) return true;

        Vector3[] frameCorners = new Vector3[4];
        windowFrame.GetWorldCorners(frameCorners);
        // corners: 0=bottom-left, 1=top-left, 2=top-right, 3=bottom-right

        Vector3 moonPos = moonRect.position;
        return moonPos.x >= frameCorners[0].x && moonPos.x <= frameCorners[2].x &&
               moonPos.y >= frameCorners[0].y && moonPos.y <= frameCorners[2].y;
    }

    private float GetDistanceOutsideFrame()
    {
        if (moonRect == null || windowFrame == null) return 0f;

        Vector3[] frameCorners = new Vector3[4];
        windowFrame.GetWorldCorners(frameCorners);

        Vector3 moonPos = moonRect.position;

        float dx = 0f;
        if (moonPos.x < frameCorners[0].x) dx = frameCorners[0].x - moonPos.x;
        else if (moonPos.x > frameCorners[2].x) dx = moonPos.x - frameCorners[2].x;

        float dy = 0f;
        if (moonPos.y < frameCorners[0].y) dy = frameCorners[0].y - moonPos.y;
        else if (moonPos.y > frameCorners[2].y) dy = moonPos.y - frameCorners[2].y;

        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    private void SetMoonAlpha(float alpha)
    {
        if (moonImage != null)
        {
            Color c = moonImage.color;
            moonImage.color = new Color(c.r, c.g, c.b, alpha);
        }
    }

    private void SetGradientAlpha(float alpha)
    {
        if (gradientOverlay != null)
        {
            Color c = gradientOverlay.color;
            gradientOverlay.color = new Color(c.r, c.g, c.b, alpha);
        }
    }
}
