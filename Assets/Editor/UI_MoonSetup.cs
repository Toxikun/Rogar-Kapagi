#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Doğu duvarındaki Ay puzzle'ını kurar.
/// </summary>
public class UI_MoonSetup
{
    public static void CreateUIMoonPuzzle()
    {
        // 0. Cleanup
        var oldPanel = GameObject.Find("MoonPuzzlePanel");
        if (oldPanel != null) Object.DestroyImmediate(oldPanel);

        // 1. Find GameCanvas
        GameObject canvas = GameObject.Find("GameCanvas");
        if (canvas == null) return;

        // 2. Main Panel
        GameObject panel = CreateUIObject("MoonPuzzlePanel", canvas.transform);
        StretchFull(panel);
        var panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.95f);
        var moonPuzzle = panel.AddComponent<MoonPuzzle>();
        moonPuzzle.puzzlePanel = panel;

        // 3. Window Frame (visual only)
        GameObject frame = CreateUIObject("WindowFrame", panel.transform);
        var frameRect = frame.GetComponent<RectTransform>();
        frameRect.sizeDelta = new Vector2(400, 500);
        var frameImg = frame.AddComponent<Image>();
        frameImg.color = new Color(0.1f, 0.1f, 0.15f, 1f);
        moonPuzzle.windowFrame = frameRect;

        // 4. Gradient Overlay (Multiply)
        GameObject grad = CreateUIObject("GradientOverlay", frame.transform);
        StretchFull(grad);
        var gradImg = grad.AddComponent<Image>();
        gradImg.color = new Color(0.2f, 0.2f, 0.3f, 0.8f);
        
        // Apply Multiply shader
        Shader multShader = Shader.Find("UI/Multiply");
        if (multShader != null)
        {
            gradImg.material = new Material(multShader);
        }
        moonPuzzle.gradientOverlay = gradImg;

        // 5. Moon
        GameObject moon = CreateUIObject("Moon", frame.transform);
        var moonRect = moon.GetComponent<RectTransform>();
        moonRect.sizeDelta = new Vector2(100, 100);
        var moonImg = moon.AddComponent<Image>();
        moonImg.color = new Color(1f, 1f, 0.8f, 1f);
        moonPuzzle.moonImage = moonImg;
        moonPuzzle.moonRect = moonRect;

        // Add Drag Handler
        var drag = moon.AddComponent<MoonDragHandler>();
        drag.moonPuzzle = moonPuzzle;

        // 6. Back Button
        GameObject backBtnGo = CreateUIObject("BackButton", panel.transform);
        var backRect = backBtnGo.GetComponent<RectTransform>();
        backRect.anchorMin = new Vector2(0, 1);
        backRect.anchorMax = new Vector2(0, 1);
        backRect.sizeDelta = new Vector2(160, 60);
        backRect.anchoredPosition = new Vector2(100, -50);
        var backImg = backBtnGo.AddComponent<Image>();
        backImg.color = new Color(0.5f, 0.2f, 0.2f);
        var btn = backBtnGo.AddComponent<Button>();
        moonPuzzle.backButton = btn;

        GameObject backTextGo = CreateUIObject("Text", backBtnGo.transform);
        StretchFull(backTextGo);
        var txt = backTextGo.AddComponent<TextMeshProUGUI>();
        txt.text = "← GERİ";
        txt.fontSize = 24;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.white;

        // 7. Link to East Wall (Olta Buttons & Window)
        GameObject wallEast = GameObject.Find("Wall_East");
        if (wallEast != null)
        {
            // Update Pencere interactable
            Transform winTrans = wallEast.transform.Find("Pencere");
            if (winTrans != null)
            {
                var inter = winTrans.GetComponent<Interactable>();
                if (inter != null)
                {
                    inter.type = InteractableType.Puzzle;
                    inter.safePopup = panel;
                    EditorUtility.SetDirty(inter);
                }
            }

            // Create Olta Toggle UI on wall
            GameObject oltaToggle = new GameObject("OltaToggleArea", typeof(RectTransform));
            oltaToggle.transform.SetParent(wallEast.transform, false);
            var tr = oltaToggle.GetComponent<RectTransform>();
            tr.anchoredPosition = new Vector2(380, -200);
            tr.sizeDelta = new Vector2(200, 150);

            // ON Button
            GameObject onBtnGo = CreateUIObject("OltaON", oltaToggle.transform);
            var onRect = onBtnGo.GetComponent<RectTransform>();
            onRect.anchoredPosition = new Vector2(0, 40);
            onRect.sizeDelta = new Vector2(140, 50);
            var onImg = onBtnGo.AddComponent<Image>();
            onImg.color = new Color(0.2f, 0.6f, 0.2f);
            var onBtn = onBtnGo.AddComponent<Button>();
            moonPuzzle.toggleOnButton = onBtn;
            CreateLabel(onBtnGo.transform, "OLTA AL", 20);

            // OFF Button
            GameObject offBtnGo = CreateUIObject("OltaOFF", oltaToggle.transform);
            var offRect = offBtnGo.GetComponent<RectTransform>();
            offRect.anchoredPosition = new Vector2(0, -20);
            offRect.sizeDelta = new Vector2(140, 50);
            var offImg = offBtnGo.AddComponent<Image>();
            offImg.color = new Color(0.6f, 0.2f, 0.2f);
            var offBtn = offBtnGo.AddComponent<Button>();
            moonPuzzle.toggleOffButton = offBtn;
            CreateLabel(offBtnGo.transform, "BIRAK", 20);

            // Status Text
            GameObject statusGo = CreateUIObject("Status", oltaToggle.transform);
            var statRect = statusGo.GetComponent<RectTransform>();
            statRect.anchoredPosition = new Vector2(0, 90);
            var statTxt = statusGo.AddComponent<TextMeshProUGUI>();
            statTxt.text = "Olta: KAPALI";
            statTxt.fontSize = 22;
            statTxt.alignment = TextAlignmentOptions.Center;
            moonPuzzle.toggleStatusText = statTxt;
        }

        panel.SetActive(false);
        EditorUtility.SetDirty(panel);
        Debug.Log("Moon Puzzle UI created.");
    }

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
    }

    private static void CreateLabel(Transform parent, string text, int size)
    {
        GameObject go = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.sizeDelta = Vector2.zero;
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.raycastTarget = false;
    }
}
#endif
