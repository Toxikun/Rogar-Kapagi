#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Puzzles.SoundColor;

/// <summary>
/// Editor tool to create the Sound Color Puzzle panel on Güney Duvar (South Wall).
/// Creates 4 colored squares, wires audio clips, and links to the Kasa interactable.
/// </summary>
public class UI_SoundColorPuzzleSetup
{
    [MenuItem("Tools/Setup Sound Color Puzzle (Güney Duvar)")]
    public static void CreateUISoundColorPuzzle()
    {
        // 0. Cleanup old panel
        var oldPanel = GameObject.Find("SoundColorPuzzlePanel");
        if (oldPanel != null)
        {
            Object.DestroyImmediate(oldPanel);
            Debug.Log("Removed old SoundColorPuzzlePanel.");
        }

        // 1. Find GameCanvas
        GameObject gameCanvas = GameObject.Find("GameCanvas");
        if (gameCanvas == null)
        {
            Debug.LogError("GameCanvas not found! Open SampleScene first.");
            return;
        }

        // ==========================================
        // 2. ROOT PANEL (full-screen dark overlay)
        // ==========================================
        GameObject panel = CreateUIObject("SoundColorPuzzlePanel", gameCanvas.transform);
        StretchFull(panel);
        var panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0.05f, 0.05f, 0.1f, 0.97f);

        var puzzle = panel.AddComponent<SoundColorPuzzle>();

        // ==========================================
        // 3. TITLE
        // ==========================================
        GameObject titleGo = CreateUIObject("Title", panel.transform);
        var titleRect = titleGo.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(500, 60);
        titleRect.anchoredPosition = new Vector2(0, -50);
        var titleText = titleGo.AddComponent<TextMeshProUGUI>();
        titleText.text = "Kasa Bulmacası";
        titleText.fontSize = 36;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        titleText.fontStyle = FontStyles.Bold;

        // ==========================================
        // 4. FOUR COLOR SQUARES
        // ==========================================
        // Starting colors: Blue, Red, Red, Green
        Color blue  = new Color(0.2f, 0.4f, 0.9f, 1f);
        Color red   = new Color(0.9f, 0.2f, 0.2f, 1f);
        Color green = new Color(0.2f, 0.8f, 0.3f, 1f);

        Color[] startColors = { blue, red, red, green };
        float squareSize = 120f;
        float spacing = 30f;
        float totalWidth = 4 * squareSize + 3 * spacing;
        float startX = -totalWidth / 2f + squareSize / 2f;

        Image[] squareImages = new Image[4];

        for (int i = 0; i < 4; i++)
        {
            string name = "Square_" + i;
            GameObject sq = CreateUIObject(name, panel.transform);
            var sqRect = sq.GetComponent<RectTransform>();
            sqRect.anchorMin = new Vector2(0.5f, 0.5f);
            sqRect.anchorMax = new Vector2(0.5f, 0.5f);
            sqRect.sizeDelta = new Vector2(squareSize, squareSize);
            sqRect.anchoredPosition = new Vector2(startX + i * (squareSize + spacing), 0);

            var sqImg = sq.AddComponent<Image>();
            sqImg.color = startColors[i];
            squareImages[i] = sqImg;

            // Add button for click interaction
            var btn = sq.AddComponent<Button>();
            btn.targetGraphic = sqImg;

            // Disable auto color tint so our color cycling is visible
            var colors = btn.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
            colors.pressedColor = new Color(0.7f, 0.7f, 0.7f);
            colors.selectedColor = Color.white;
            btn.colors = colors;

            // Wire click to puzzle via runtime binding (project pattern)
            // We need to capture the index in a local variable for the closure
            int idx = i;
            btn.onClick.AddListener(() => puzzle.OnSquareClicked(idx));

            // Label showing square number
            GameObject labelGo = CreateUIObject("Label", sq.transform);
            StretchFull(labelGo);
            var labelTxt = labelGo.AddComponent<TextMeshProUGUI>();
            labelTxt.text = (i + 1).ToString();
            labelTxt.fontSize = 32;
            labelTxt.alignment = TextAlignmentOptions.Center;
            labelTxt.color = new Color(1f, 1f, 1f, 0.5f);
            labelTxt.raycastTarget = false;
        }

        puzzle.squares = squareImages;

        // ==========================================
        // 5. WIN TEXT (hidden by default)
        // ==========================================
        GameObject winGo = CreateUIObject("WinText", panel.transform);
        var winRect = winGo.GetComponent<RectTransform>();
        winRect.anchorMin = new Vector2(0.5f, 0.75f);
        winRect.anchorMax = new Vector2(0.5f, 0.75f);
        winRect.sizeDelta = new Vector2(600, 80);
        var winTxt = winGo.AddComponent<TextMeshProUGUI>();
        winTxt.text = "Senkronize ettin!";
        winTxt.fontSize = 48;
        winTxt.alignment = TextAlignmentOptions.Center;
        winTxt.color = new Color(0.2f, 1f, 0.4f);
        winTxt.fontStyle = FontStyles.Bold;
        winGo.SetActive(false);
        puzzle.winText = winTxt;

        // ==========================================
        // 6. CLOSE BUTTON
        // ==========================================
        GameObject closeBtnGo = CreateUIObject("CloseButton", panel.transform);
        var closeRect = closeBtnGo.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1f, 1f);
        closeRect.anchorMax = new Vector2(1f, 1f);
        closeRect.sizeDelta = new Vector2(120, 50);
        closeRect.anchoredPosition = new Vector2(-80, -50);
        var closeBtnImg = closeBtnGo.AddComponent<Image>();
        closeBtnImg.color = new Color(0.8f, 0.2f, 0.2f);
        var closeBtn = closeBtnGo.AddComponent<Button>();
        closeBtn.targetGraphic = closeBtnImg;
        puzzle.closeButton = closeBtn;

        GameObject closeTextGo = CreateUIObject("Text", closeBtnGo.transform);
        StretchFull(closeTextGo);
        var closeTxt = closeTextGo.AddComponent<TextMeshProUGUI>();
        closeTxt.text = "Kapat";
        closeTxt.fontSize = 22;
        closeTxt.alignment = TextAlignmentOptions.Center;
        closeTxt.color = Color.white;

        // ==========================================
        // 7. LOAD AUDIO CLIPS
        // ==========================================
        AudioClip bgClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/sound_example.mp3");
        AudioClip hornClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/horn_sound.mp3");

        if (bgClip != null) puzzle.backgroundClip = bgClip;
        else Debug.LogWarning("sound_example.mp3 not found at Assets/sound_example.mp3");

        if (hornClip != null) puzzle.solvedClip = hornClip;
        else Debug.LogWarning("horn_sound.mp3 not found at Assets/horn_sound.mp3");

        // ==========================================
        // 8. HIDE PANEL & LINK TO KASA
        // ==========================================
        panel.SetActive(false);

        // Find Kasa using Resources.FindObjectsOfTypeAll (same pattern as Metronome setup)
        Interactable kasaInteractable = null;
        var allInteractables = Resources.FindObjectsOfTypeAll<Interactable>();
        foreach (var inter in allInteractables)
        {
            if (inter == null || inter.gameObject == null) continue;
            if (inter.gameObject.name == "Kasa" && inter.gameObject.scene.isLoaded)
            {
                kasaInteractable = inter;
                break;
            }
        }

        if (kasaInteractable != null)
        {
            kasaInteractable.type = InteractableType.Puzzle;
            kasaInteractable.safePopup = panel;
            EditorUtility.SetDirty(kasaInteractable);
            Debug.Log("Linked SoundColorPuzzlePanel → Kasa (Puzzle type).");
        }
        else
        {
            Debug.LogWarning("Kasa not found! Manually assign the puzzle panel.");
        }

        // Mark dirty
        EditorUtility.SetDirty(panel);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene()
        );

        Debug.Log("✓ Sound Color Puzzle created successfully!");
    }

    // ===== HELPERS =====

    private static GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private static void StretchFull(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
    }
}
#endif
