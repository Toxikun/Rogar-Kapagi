using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PaintingPuzzle : MonoBehaviour
{
    [Header("Puzzle Panel")]
    public GameObject puzzlePanel;

    [Header("Painting Pieces")]
    public RectTransform paintingLeft;   // Sol parça (yerinde kalır)
    public RectTransform paintingRight;  // Sağ parça (düşecek)

    [Header("Back Button")]
    public Button backButton;

    [Header("State")]
    public bool isCut = false;

    // Sağ parça butonu (kesme işlemi için tıklandığında)
    public Button paintingRightButton;

    private void Start()
    {
        if (backButton != null)
            backButton.onClick.AddListener(ClosePuzzle);
        if (paintingRightButton != null)
            paintingRightButton.onClick.AddListener(OnPaintingClicked);

        if (puzzlePanel != null)
            puzzlePanel.SetActive(false);
    }

    // ===== PUZZLE AÇ/KAPAT =====
    public void OpenPuzzle()
    {
        if (puzzlePanel != null)
            puzzlePanel.SetActive(true);
    }

    public void ClosePuzzle()
    {
        if (puzzlePanel != null)
            puzzlePanel.SetActive(false);
    }

    // ===== TABLOYA TIKLAMA =====
    public void OnPaintingClicked()
    {
        if (isCut)
        {
            Debug.Log("Tablo zaten kesilmiş.");
            return;
        }

        GameManager gm = GameManager.Instance;
        if (gm == null || gm.inventory == null) return;

        // Ay envanterde seçili mi?
        string selected = gm.inventory.GetSelectedItem();
        if (selected != "Ay")
        {
            Debug.LogWarning("Ay seçili değil! Envanterden ay objesini seç.");
            gm.dialogBox.Show("Tabloyu kesmek için keskin bir şeye (Ay) ihtiyacın var.", 3f);
            return;
        }

        // KES!
        isCut = true;
        Debug.Log("Tablo kesildi!");
        gm.dialogBox.Show("Ay bıçağı ile tabloyu kestin! Arkasında bir şey var...", 3f);
        StartCoroutine(FallAnimation(paintingRight));
    }

    private IEnumerator FallAnimation(RectTransform piece)
    {
        if (piece == null) yield break;

        Vector2 startPos = piece.anchoredPosition;
        // Sağ parça: hafifçe sağa kayarak aşağı düşer
        Vector2 endPos = startPos + new Vector2(150f, -900f);
        float duration = 1.5f;
        float elapsed = 0f;

        // Hafif saat yönünde dönme (sağ parça kopunca sağa doğru döner)
        float startRotation = 0f;
        float endRotation = 35f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Easing: hızlanarak düşme (yerçekimi hissi)
            float easedT = t * t * t; // Cubic easing – daha dramatik

            piece.anchoredPosition = Vector2.Lerp(startPos, endPos, easedT);
            piece.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(startRotation, endRotation, easedT));

            // Son %40'da fade out
            if (t > 0.6f)
            {
                Image img = piece.GetComponent<Image>();
                if (img != null)
                {
                    float alpha = Mathf.Lerp(1f, 0f, (t - 0.6f) / 0.4f);
                    img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
                }

                // İçindeki texti de soldur
                TextMeshProUGUI txt = piece.GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null)
                {
                    float alpha = Mathf.Lerp(1f, 0f, (t - 0.6f) / 0.4f);
                    txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, alpha);
                }
            }

            yield return null;
        }

        piece.gameObject.SetActive(false);
    }
}
