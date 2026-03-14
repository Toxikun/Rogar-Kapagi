#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Puzzles.MetronomeUI;

public class UI_MetronomeSetup : EditorWindow
{
    [MenuItem("Tools/Setup UI Metronome Puzzle (Kuzey Duvar)")]
    public static void CreateUIMetronomePuzzle()
    {
        // 1. Find GameCanvas
        GameObject gameCanvas = GameObject.Find("GameCanvas");
        if (gameCanvas == null)
        {
            Debug.LogError("GameCanvas not found! Make sure you are in SampleScene.");
            return;
        }

        // 2. Create PuzzlePanel if it doesn't exist, or just create a specific one
        GameObject metronomePanel = new GameObject("MetronomePuzzlePanel");
        metronomePanel.transform.SetParent(gameCanvas.transform, false);
        var panelRect = metronomePanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;
        
        var bgImage = metronomePanel.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f); // Dark background

        var manager = metronomePanel.AddComponent<MetronomeUIPuzzleManager>();
        
        // 3. Close Button
        GameObject closeBtnGo = new GameObject("CloseButton");
        closeBtnGo.transform.SetParent(metronomePanel.transform, false);
        var closeRect = closeBtnGo.AddComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1, 1);
        closeRect.anchorMax = new Vector2(1, 1);
        closeRect.sizeDelta = new Vector2(150, 80);
        closeRect.anchoredPosition = new Vector2(-100, -60);
        
        var closeImg = closeBtnGo.AddComponent<Image>();
        closeImg.color = Color.red;
        var closeBtn = closeBtnGo.AddComponent<Button>();
        closeBtn.onClick.AddListener(() => { metronomePanel.SetActive(false); });

        GameObject closeTextGo = new GameObject("Text");
        closeTextGo.transform.SetParent(closeBtnGo.transform, false);
        var ctRect = closeTextGo.AddComponent<RectTransform>();
        ctRect.anchorMin = Vector2.zero; ctRect.anchorMax = Vector2.one;
        ctRect.sizeDelta = Vector2.zero;
        var cText = closeTextGo.AddComponent<TextMeshProUGUI>();
        cText.text = "Kapat";
        cText.color = Color.white;
        cText.alignment = TextAlignmentOptions.Center;

        // 4. "Senkronize ettin!" Text
        GameObject winTextGo = new GameObject("WinText");
        winTextGo.transform.SetParent(metronomePanel.transform, false);
        var winRect = winTextGo.AddComponent<RectTransform>();
        winRect.anchorMin = new Vector2(0.5f, 0.8f);
        winRect.anchorMax = new Vector2(0.5f, 0.8f);
        winRect.sizeDelta = new Vector2(600, 100);
        winRect.anchoredPosition = Vector2.zero;
        var wText = winTextGo.AddComponent<TextMeshProUGUI>();
        wText.text = "Senkronize ettin!";
        wText.color = Color.green;
        wText.fontSize = 50;
        wText.alignment = TextAlignmentOptions.Center;
        wText.fontStyle = FontStyles.Bold;
        manager.winText = wText;

        // 5. Left Pan
        var leftPan = CreateScalePan(metronomePanel.transform, "LeftPan", new Vector2(-300, -100));
        manager.leftPan = leftPan;
        leftPan.isLeftPan = true;

        // 6. Right Pan
        var rightPan = CreateScalePan(metronomePanel.transform, "RightPan", new Vector2(300, -100));
        manager.rightPan = rightPan;
        leftPan.isLeftPan = false;

        // 7. Initial Weights
        // Left Side: 3x 1kg, 1x 2kg
        CreateWeight(metronomePanel.transform, "Weight_1kg_L1", new Vector2(-400, 50), 1f, Color.white, leftPan);
        CreateWeight(metronomePanel.transform, "Weight_1kg_L2", new Vector2(-300, 50), 1f, Color.white, leftPan);
        CreateWeight(metronomePanel.transform, "Weight_1kg_L3", new Vector2(-200, 50), 1f, Color.white, leftPan);
        CreateWeight(metronomePanel.transform, "Weight_2kg_L1", new Vector2(-300, 150), 2f, Color.cyan, leftPan);

        // Right Side: 1x 1kg, 1x 2kg
        CreateWeight(metronomePanel.transform, "Weight_1kg_R1", new Vector2(250, 50), 1f, Color.white, rightPan);
        CreateWeight(metronomePanel.transform, "Weight_2kg_R1", new Vector2(350, 50), 2f, Color.cyan, rightPan);

        // Temporarily hide the panel
        metronomePanel.SetActive(false);

        // 8. Link to Dolap interactable
        GameObject dolap = GameObject.Find("CubeEscapeRoot/GameCanvas/Wall_North/Dolap");
        if (dolap != null)
        {
            var interactable = dolap.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactable.type = InteractableType.Puzzle;
                interactable.safePopup = metronomePanel; // using safePopup variable for generic puzzle panel
                EditorUtility.SetDirty(interactable);
                Debug.Log("Linked MetronomePuzzlePanel to Dolap interactable.");
            }
        }
        else
        {
            Debug.LogWarning("Dolap object not found to link interactable.");
        }

        Debug.Log("UI Metronome Puzzle successfully created!");
    }

    private static UIScalePan CreateScalePan(Transform parent, string name, Vector2 anchoredPos)
    {
        GameObject panGo = new GameObject(name);
        panGo.transform.SetParent(parent, false);
        var rect = panGo.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(250, 100);
        rect.anchoredPosition = anchoredPos;
        
        var img = panGo.AddComponent<Image>();
        img.color = new Color(0.3f, 0.3f, 0.3f, 1f); // Gray box

        GameObject textGo = new GameObject("Label");
        textGo.transform.SetParent(panGo.transform, false);
        var tRect = textGo.AddComponent<RectTransform>();
        tRect.anchorMin = Vector2.zero; tRect.anchorMax = Vector2.one;
        tRect.sizeDelta = Vector2.zero;
        var txt = textGo.AddComponent<TextMeshProUGUI>();
        txt.text = name;
        txt.fontSize = 24;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.black;

        var scalePan = panGo.AddComponent<UIScalePan>();
        
        // Setup layour point for putting weights
        GameObject placement = new GameObject("WeightPlacement");
        placement.transform.SetParent(panGo.transform, false);
        var pRect = placement.AddComponent<RectTransform>();
        pRect.anchorMin = new Vector2(0.5f, 0.5f); pRect.anchorMax = new Vector2(0.5f, 0.5f);
        pRect.anchoredPosition = new Vector2(0, 100); // 100 px above pan
        scalePan.weightPlacementPoint = placement.transform;

        return scalePan;
    }

    private static UIWeightInteractable CreateWeight(Transform parent, string name, Vector2 anchoredPos, float weightVal, Color color, UIScalePan initialPan)
    {
        GameObject wGo = new GameObject(name);
        wGo.transform.SetParent(parent, false);
        
        var rect = wGo.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        // Size proportional to weight
        rect.sizeDelta = new Vector2(80, 80) * (weightVal == 1 ? 1 : 1.3f); 
        rect.anchoredPosition = anchoredPos;

        var img = wGo.AddComponent<Image>();
        img.color = color;

        GameObject textGo = new GameObject("Label");
        textGo.transform.SetParent(wGo.transform, false);
        var tRect = textGo.AddComponent<RectTransform>();
        tRect.anchorMin = Vector2.zero; tRect.anchorMax = Vector2.one;
        tRect.sizeDelta = Vector2.zero;
        var txt = textGo.AddComponent<TextMeshProUGUI>();
        txt.text = weightVal + " kg";
        txt.fontSize = 20;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.black;

        var uiw = wGo.AddComponent<UIWeightInteractable>();
        uiw.weightValue = weightVal;

        // Auto place in initial pan
        if (initialPan != null)
        {
            initialPan.PlaceWeight(uiw);
        }

        return uiw;
    }
}
#endif
