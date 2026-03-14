#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Puzzles.MetronomeUI;

/// <summary>
/// Editor tool that generates the MetronomePuzzlePanel inside GameCanvas
/// and links the Dolap interactable to open it.
///
/// Layout:
///   MetronomePuzzlePanel (full-screen dark overlay)
///     ├── OuterView (shows the "box" rectangle you click to enter)
///     │     ├── BoxImage (a tall rectangle)
///     │     ├── InstructionText ("Kutuya dokun")
///     │     └── CloseButton
///     └── InnerView (the actual scale puzzle)
///           ├── Title ("Terazi Bulmacası")
///           ├── ScaleBase (horizontal bar connecting the two pans)
///           ├── LeftPanBar (horizontal rectangle, weights sit on top)
///           │     └── [Weight squares parented here by script]
///           ├── RightPanBar (horizontal rectangle)
///           │     └── [Weight squares parented here by script]  
///           ├── Weight_1kg_L1, Weight_1kg_L2, Weight_1kg_L3 (small white squares)
///           ├── Weight_2kg_L1 (larger cyan square)
///           ├── Weight_1kg_R1 (small white square)
///           ├── Weight_2kg_R1 (larger cyan square)
///           ├── WinText ("Senkronize ettin!")
///           ├── BackButton ("Geri")
///           └── CloseButton ("Kapat")
/// </summary>
public class UI_MetronomeSetup : EditorWindow
{
    public static void CreateUIMetronomePuzzle()
    {
        // 0. Cleanup: remove old panel if exists
        var oldPanel = GameObject.Find("MetronomePuzzlePanel");
        if (oldPanel != null)
        {
            DestroyImmediate(oldPanel);
            Debug.Log("Removed old MetronomePuzzlePanel.");
        }

        // 1. Find GameCanvas
        GameObject gameCanvas = GameObject.Find("GameCanvas");
        if (gameCanvas == null)
        {
            Debug.LogError("GameCanvas not found! Open SampleScene first.");
            return;
        }

        // ==========================================
        // 2. ROOT PANEL (full screen dark overlay)
        // ==========================================
        GameObject panel = CreateUIObject("MetronomePuzzlePanel", gameCanvas.transform);
        StretchFull(panel);
        var panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0.05f, 0.05f, 0.08f, 0.97f);
        var manager = panel.AddComponent<MetronomeUIPuzzleManager>();

        // ==========================================
        // 3. OUTER VIEW (the "box" you click to enter)
        // ==========================================
        GameObject outerView = CreateUIObject("OuterView", panel.transform);
        StretchFull(outerView);
        manager.outerView = outerView;

        // Box rectangle (dikdörtgen)
        GameObject box = CreateUIObject("BoxImage", outerView.transform);
        var boxRect = box.GetComponent<RectTransform>();
        boxRect.anchorMin = new Vector2(0.5f, 0.5f);
        boxRect.anchorMax = new Vector2(0.5f, 0.5f);
        boxRect.sizeDelta = new Vector2(300, 400);
        boxRect.anchoredPosition = Vector2.zero;
        var boxImg = box.AddComponent<Image>();
        boxImg.color = new Color(0.45f, 0.28f, 0.15f, 1f); // Brown wood-like color

        // Box label
        GameObject boxLabel = CreateUIObject("BoxLabel", box.transform);
        StretchFull(boxLabel);
        var boxText = boxLabel.AddComponent<TextMeshProUGUI>();
        boxText.text = "?";
        boxText.fontSize = 72;
        boxText.alignment = TextAlignmentOptions.Center;
        boxText.color = new Color(1f, 0.9f, 0.7f);

        // Box click button (transparent, covers the box)
        var boxBtn = box.AddComponent<Button>();
        boxBtn.targetGraphic = boxImg;

        // Instruction text
        GameObject instrGo = CreateUIObject("InstructionText", outerView.transform);
        var instrRect = instrGo.GetComponent<RectTransform>();
        instrRect.anchorMin = new Vector2(0.5f, 0.2f);
        instrRect.anchorMax = new Vector2(0.5f, 0.2f);
        instrRect.sizeDelta = new Vector2(500, 60);
        var instrText = instrGo.AddComponent<TextMeshProUGUI>();
        instrText.text = "Kutuya dokun";
        instrText.fontSize = 32;
        instrText.alignment = TextAlignmentOptions.Center;
        instrText.color = Color.white;
        instrText.fontStyle = FontStyles.Italic;

        // Close button (outer view)
        Button outerCloseBtn = CreateButton("CloseButton", outerView.transform,
            new Vector2(1f, 1f), new Vector2(1f, 1f),
            new Vector2(-80, -50), new Vector2(120, 50),
            "X", 28, new Color(0.8f, 0.2f, 0.2f), Color.white);
        manager.closeButton = outerCloseBtn;

        // ==========================================
        // 4. INNER VIEW (the scale puzzle)
        // ==========================================
        GameObject innerView = CreateUIObject("InnerView", panel.transform);
        StretchFull(innerView);
        manager.innerView = innerView;

        // Title
        GameObject titleGo = CreateUIObject("Title", innerView.transform);
        var titleRect = titleGo.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(500, 60);
        titleRect.anchoredPosition = new Vector2(0, -40);
        var titleText = titleGo.AddComponent<TextMeshProUGUI>();
        titleText.text = "Terazi Bulmacası";
        titleText.fontSize = 36;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        titleText.fontStyle = FontStyles.Bold;

        // Scale base / fulcrum (center triangle/support visual)
        GameObject fulcrum = CreateUIObject("Fulcrum", innerView.transform);
        var fulcrumRect = fulcrum.GetComponent<RectTransform>();
        fulcrumRect.anchorMin = new Vector2(0.5f, 0.5f);
        fulcrumRect.anchorMax = new Vector2(0.5f, 0.5f);
        fulcrumRect.sizeDelta = new Vector2(30, 80);
        fulcrumRect.anchoredPosition = new Vector2(0, -50);
        var fulcrumImg = fulcrum.AddComponent<Image>();
        fulcrumImg.color = new Color(0.6f, 0.6f, 0.6f);

        // Scale beam (horizontal bar connecting both pans)
        GameObject beam = CreateUIObject("ScaleBeam", innerView.transform);
        var beamRect = beam.GetComponent<RectTransform>();
        beamRect.anchorMin = new Vector2(0.5f, 0.5f);
        beamRect.anchorMax = new Vector2(0.5f, 0.5f);
        beamRect.sizeDelta = new Vector2(700, 8);
        beamRect.anchoredPosition = new Vector2(0, -10);
        var beamImg = beam.AddComponent<Image>();
        beamImg.color = new Color(0.5f, 0.5f, 0.5f);

        // LEFT PAN BAR
        GameObject leftPanGo = CreateUIObject("LeftPanBar", innerView.transform);
        var leftPanRect = leftPanGo.GetComponent<RectTransform>();
        leftPanRect.anchorMin = new Vector2(0.5f, 0.5f);
        leftPanRect.anchorMax = new Vector2(0.5f, 0.5f);
        leftPanRect.sizeDelta = new Vector2(280, 30);
        leftPanRect.anchoredPosition = new Vector2(-200, -30);
        var leftPanImg = leftPanGo.AddComponent<Image>();
        leftPanImg.color = new Color(0.35f, 0.35f, 0.4f);
        var leftPan = leftPanGo.AddComponent<UIScalePan>();
        leftPan.isLeftPan = true;
        leftPan.weightSpacing = 8f;
        leftPan.weightYOffset = 5f;
        manager.leftPan = leftPan;

        // Left pan label
        GameObject leftLabel = CreateUIObject("Label", leftPanGo.transform);
        StretchFull(leftLabel);
        var leftLabelText = leftLabel.AddComponent<TextMeshProUGUI>();
        leftLabelText.text = "Sol Kol";
        leftLabelText.fontSize = 16;
        leftLabelText.alignment = TextAlignmentOptions.Center;
        leftLabelText.color = new Color(0.8f, 0.8f, 0.8f);

        // RIGHT PAN BAR
        GameObject rightPanGo = CreateUIObject("RightPanBar", innerView.transform);
        var rightPanRect = rightPanGo.GetComponent<RectTransform>();
        rightPanRect.anchorMin = new Vector2(0.5f, 0.5f);
        rightPanRect.anchorMax = new Vector2(0.5f, 0.5f);
        rightPanRect.sizeDelta = new Vector2(280, 30);
        rightPanRect.anchoredPosition = new Vector2(200, -30);
        var rightPanImg = rightPanGo.AddComponent<Image>();
        rightPanImg.color = new Color(0.35f, 0.35f, 0.4f);
        var rightPan = rightPanGo.AddComponent<UIScalePan>();
        rightPan.isLeftPan = false;
        rightPan.weightSpacing = 8f;
        rightPan.weightYOffset = 5f;
        manager.rightPan = rightPan;

        // Right pan label
        GameObject rightLabel = CreateUIObject("Label", rightPanGo.transform);
        StretchFull(rightLabel);
        var rightLabelText = rightLabel.AddComponent<TextMeshProUGUI>();
        rightLabelText.text = "Sağ Kol";
        rightLabelText.fontSize = 16;
        rightLabelText.alignment = TextAlignmentOptions.Center;
        rightLabelText.color = new Color(0.8f, 0.8f, 0.8f);

        // ==========================================
        // 5. WEIGHTS
        // ==========================================
        // Left: 3x 1kg (white) + 1x 2kg (cyan)
        CreateWeight("Weight_1kg_L1", 1f, 60, new Color(0.9f, 0.9f, 0.85f), leftPan);
        CreateWeight("Weight_1kg_L2", 1f, 60, new Color(0.9f, 0.9f, 0.85f), leftPan);
        CreateWeight("Weight_1kg_L3", 1f, 60, new Color(0.9f, 0.9f, 0.85f), leftPan);
        CreateWeight("Weight_2kg_L1", 2f, 80, new Color(0.3f, 0.8f, 0.85f), leftPan);

        // Right: 1x 1kg (white) + 1x 2kg (cyan)
        CreateWeight("Weight_1kg_R1", 1f, 60, new Color(0.9f, 0.9f, 0.85f), rightPan);
        CreateWeight("Weight_2kg_R1", 2f, 80, new Color(0.3f, 0.8f, 0.85f), rightPan);

        // ==========================================
        // 6. WIN TEXT
        // ==========================================
        GameObject winGo = CreateUIObject("WinText", innerView.transform);
        var winRect = winGo.GetComponent<RectTransform>();
        winRect.anchorMin = new Vector2(0.5f, 0.8f);
        winRect.anchorMax = new Vector2(0.5f, 0.8f);
        winRect.sizeDelta = new Vector2(600, 80);
        var winTxt = winGo.AddComponent<TextMeshProUGUI>();
        winTxt.text = "Senkronize ettin!";
        winTxt.fontSize = 48;
        winTxt.alignment = TextAlignmentOptions.Center;
        winTxt.color = new Color(0.2f, 1f, 0.4f);
        winTxt.fontStyle = FontStyles.Bold;
        winGo.SetActive(false);
        manager.winText = winTxt;

        // ==========================================
        // 7. INNER VIEW BUTTONS
        // ==========================================
        // Back button
        Button backBtn = CreateButton("BackButton", innerView.transform,
            new Vector2(0f, 1f), new Vector2(0f, 1f),
            new Vector2(80, -50), new Vector2(120, 50),
            "Geri", 22, new Color(0.3f, 0.3f, 0.5f), Color.white);
        manager.backButton = backBtn;

        // Close button (inner view) - reuse the same close ref
        Button innerCloseBtn = CreateButton("CloseButton_Inner", innerView.transform,
            new Vector2(1f, 1f), new Vector2(1f, 1f),
            new Vector2(-80, -50), new Vector2(120, 50),
            "Kapat", 22, new Color(0.8f, 0.2f, 0.2f), Color.white);
        // Wire inner close button in Start() via code
        manager.innerCloseButton = innerCloseBtn;

        // ==========================================
        // 8. WIRE UP BOX BUTTON → EnterInnerView
        // ==========================================
        // Wire up via runtime script to avoid serialization loss
        var boxOpener = box.AddComponent<Button>();
        DestroyImmediate(boxOpener); // remove the duplicate
        manager.boxButton = boxBtn;

        // ==========================================
        // 9. HIDE PANEL & LINK TO DOLAP
        // ==========================================
        panel.SetActive(false);

        Interactable dolapInteractable = null;
        var allInteractables = Resources.FindObjectsOfTypeAll<Interactable>();
        foreach (var inter in allInteractables)
        {
            if (inter == null || inter.gameObject == null) continue; // Skip destroyed Unity objects

            // Yalnızca sahnede olanları al (Asset olanları atla)
            if (inter.gameObject.name == "Dolap" && inter.gameObject.scene.isLoaded)
            {
                dolapInteractable = inter;
                break;
            }
        }

        if (dolapInteractable != null)
        {
            dolapInteractable.type = InteractableType.Puzzle;
            dolapInteractable.safePopup = panel;
            EditorUtility.SetDirty(dolapInteractable);
            Debug.Log("Linked MetronomePuzzlePanel → Dolap (Puzzle type).");
        }
        else
        {
            Debug.LogWarning("Dolap not found! You'll need to manually assign the puzzle panel.");
        }

        // Mark scene dirty
        panel.SetActive(false);
        EditorUtility.SetDirty(panel);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene()
        );

        Debug.Log("✓ UI Metronome Puzzle created successfully!");
    }

    // ===== HELPER METHODS =====

    private static GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
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

    private static Button CreateButton(string name, Transform parent,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 anchoredPos, Vector2 size,
        string label, int fontSize, Color bgColor, Color textColor)
    {
        GameObject btnGo = CreateUIObject(name, parent);
        var btnRect = btnGo.GetComponent<RectTransform>();
        btnRect.anchorMin = anchorMin;
        btnRect.anchorMax = anchorMax;
        btnRect.sizeDelta = size;
        btnRect.anchoredPosition = anchoredPos;

        var btnImg = btnGo.AddComponent<Image>();
        btnImg.color = bgColor;
        var btn = btnGo.AddComponent<Button>();
        btn.targetGraphic = btnImg;

        GameObject textGo = CreateUIObject("Text", btnGo.transform);
        StretchFull(textGo);
        var txt = textGo.AddComponent<TextMeshProUGUI>();
        txt.text = label;
        txt.fontSize = fontSize;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = textColor;

        return btn;
    }

    private static UIWeightInteractable CreateWeight(string name, float weightVal, float size, Color color, UIScalePan initialPan)
    {
        GameObject wGo = CreateUIObject(name, initialPan.transform);
        var rect = wGo.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(size, size);

        var img = wGo.AddComponent<Image>();
        img.color = color;

        // Weight label
        GameObject labelGo = CreateUIObject("Label", wGo.transform);
        StretchFull(labelGo);
        var txt = labelGo.AddComponent<TextMeshProUGUI>();
        txt.text = weightVal + " kg";
        txt.fontSize = (int)(size * 0.28f);
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.black;
        txt.fontStyle = FontStyles.Bold;

        var uiw = wGo.AddComponent<UIWeightInteractable>();
        uiw.weightValue = weightVal;

        // Place in pan
        initialPan.PlaceWeight(uiw);

        return uiw;
    }
}
#endif
