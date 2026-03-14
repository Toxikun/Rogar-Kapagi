using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CubeEscapeSetup
{
    [MenuItem("Tools/Setup Cube Escape")]
    public static void Setup()
    {
        // Temizle
        DestroyIfExists("CubeEscapeRoot");

        // ====== ANA ROOT ======
        GameObject root = new GameObject("CubeEscapeRoot");

        // ====== CANVAS ======
        GameObject canvasObj = CreateCanvas("GameCanvas", root.transform);

        // ====== DUVAR PANELLERİ ======
        Color[] wallBgColors = {
            new Color(0.12f, 0.10f, 0.14f), // Kuzey - koyu mor
            new Color(0.10f, 0.12f, 0.10f), // Doğu  - koyu yeşilimsi
            new Color(0.14f, 0.10f, 0.10f), // Güney - koyu kırmızımsı
            new Color(0.10f, 0.10f, 0.14f), // Batı  - koyu mavimsi
        };

        string[] wallNames = { "Wall_North", "Wall_East", "Wall_South", "Wall_West" };
        List<GameObject> wallPanels = new List<GameObject>();

        for (int i = 0; i < 4; i++)
        {
            GameObject wall = CreatePanel(wallNames[i], canvasObj.transform, wallBgColors[i]);
            Stretch(wall);
            wallPanels.Add(wall);
        }

        // ====== KUZEY DUVAR (0) – Dolap + Tablo ======
        {
            GameObject wall = wallPanels[0];

            // Dolap
            Interactable cabinet = CreateInteractableButton(wall.transform, "Dolap",
                new Vector2(-200, -50), new Vector2(220, 300),
                new Color(0.35f, 0.22f, 0.12f), 0);
            cabinet.type = InteractableType.Puzzle;
            cabinet.clueMessage = "";

            // Üstüne "DOLAP" yazısı
            CreateLabel(cabinet.transform, "DOLAP", 28, new Color(0.8f, 0.7f, 0.5f));

            // Tablo (ipucu)
            Interactable painting = CreateInteractableButton(wall.transform, "Tablo",
                new Vector2(200, 100), new Vector2(200, 160),
                new Color(0.5f, 0.35f, 0.15f), 0);
            painting.type = InteractableType.Clue;
            painting.clueMessage = "Tablonun arkasında bir not var:\n\"Şifre: 3 - 7 - 1\"";
            CreateLabel(painting.transform, "TABLO", 24, new Color(0.9f, 0.8f, 0.6f));

            // Duvar etiketi
            CreateWallTitle(wall.transform, "Kuzey Duvar");
        }

        // ====== DOĞU DUVAR (1) – Pencere + Saat ======
        {
            GameObject wall = wallPanels[1];

            Interactable window = CreateInteractableButton(wall.transform, "Pencere",
                new Vector2(0, 100), new Vector2(250, 300),
                new Color(0.15f, 0.20f, 0.35f), 1);
            window.type = InteractableType.Atmosphere;
            window.clueMessage = "Dışarıda yoğun bir sis var.\nHiçbir şey göremiyorsun...";
            CreateLabel(window.transform, "PENCERE", 24, new Color(0.5f, 0.6f, 0.8f));

            Interactable clock = CreateInteractableButton(wall.transform, "Saat",
                new Vector2(-220, -100), new Vector2(120, 120),
                new Color(0.6f, 0.55f, 0.40f), 1);
            clock.type = InteractableType.Atmosphere;
            clock.clueMessage = "Saat durmuş...\nDuvardan tıkırtı sesleri geliyor.";
            CreateLabel(clock.transform, "SAAT", 22, new Color(0.8f, 0.7f, 0.5f));

            CreateWallTitle(wall.transform, "Doğu Duvar");
        }

        // ====== GÜNEY DUVAR (2) – Kapı + Kasa ======
        {
            GameObject wall = wallPanels[2];

            // Kapı (kilitli)
            Interactable door = CreateInteractableButton(wall.transform, "Kapi",
                new Vector2(-150, 0), new Vector2(180, 380),
                new Color(0.30f, 0.18f, 0.08f), 2);
            door.type = InteractableType.LockedDoor;
            door.requiredItem = "Anahtar";
            CreateLabel(door.transform, "KAPI\n(Kilitli)", 26, new Color(0.9f, 0.3f, 0.3f));

            // Kasa (şifreli)
            Interactable safe = CreateInteractableButton(wall.transform, "Kasa",
                new Vector2(220, -100), new Vector2(180, 140),
                new Color(0.25f, 0.25f, 0.28f), 2);
            safe.type = InteractableType.Safe;
            CreateLabel(safe.transform, "KASA", 26, new Color(0.7f, 0.7f, 0.7f));

            CreateWallTitle(wall.transform, "Güney Duvar");
        }

        // ====== BATI DUVAR (3) – Kitaplık (Puzzle açar) + Ayna ======
        {
            GameObject wall = wallPanels[3];

            // Kitaplık → Tıklayınca puzzle sahnesini açacak (PuzzleOpener ile)
            GameObject bookshelfObj = CreatePanel("Kitaplik", wall.transform, new Color(0.28f, 0.15f, 0.08f));
            bookshelfObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(180, 0);
            bookshelfObj.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 350);
            bookshelfObj.AddComponent<Shadow>().effectColor = new Color(0, 0, 0, 0.5f);
            bookshelfObj.GetComponent<Shadow>().effectDistance = new Vector2(0, -5);
            Button bookshelfBtn = bookshelfObj.AddComponent<Button>();
            PuzzleOpener opener = bookshelfObj.AddComponent<PuzzleOpener>(); // Listener'ı bu bağlayacak
            CreateLabel(bookshelfObj.transform, "KİTAPLIK\n(Tıkla)", 24, new Color(0.8f, 0.65f, 0.45f));

            Interactable mirror = CreateInteractableButton(wall.transform, "Ayna",
                new Vector2(-200, 50), new Vector2(150, 220),
                new Color(0.35f, 0.40f, 0.45f), 3);
            mirror.type = InteractableType.Atmosphere;
            mirror.clueMessage = "Aynaya baktın...\nYansımanda bir anlık\nbir gölge gördün.";
            CreateLabel(mirror.transform, "AYNA", 24, new Color(0.7f, 0.8f, 0.9f));

            CreateWallTitle(wall.transform, "Batı Duvar");
        }

        // ====== NAVİGASYON OKLARI ======
        GameObject navPanel = new GameObject("NavPanel", typeof(RectTransform));
        navPanel.transform.SetParent(canvasObj.transform, false);
        Stretch(navPanel);

        // Sol ok
        GameObject leftArrow = CreateButton(navPanel.transform, "LeftArrow",
            new Vector2(-450, 0), new Vector2(120, 120),
            new Color(0.2f, 0.2f, 0.2f, 0.7f), "◀", 60);

        // Sağ ok
        GameObject rightArrow = CreateButton(navPanel.transform, "RightArrow",
            new Vector2(450, 0), new Vector2(120, 120),
            new Color(0.2f, 0.2f, 0.2f, 0.7f), "▶", 60);

        // Duvar ismi (üst orta)
        GameObject wallLabelObj = CreateTextObj(navPanel.transform, "WallLabel",
            new Vector2(0, 420), new Vector2(400, 80), "", 40, new Color(0.9f, 0.9f, 0.9f, 0.7f));

        // ====== ENVANTER ÇUBUĞU ======
        GameObject invBar = CreatePanel("InventoryBar", canvasObj.transform, new Color(0.08f, 0.08f, 0.08f, 0.85f));
        RectTransform invRT = invBar.GetComponent<RectTransform>();
        invRT.anchorMin = new Vector2(0, 0);
        invRT.anchorMax = new Vector2(1, 0);
        invRT.pivot = new Vector2(0.5f, 0);
        invRT.sizeDelta = new Vector2(0, 140);
        invRT.anchoredPosition = Vector2.zero;

        // Envanter başlığı
        CreateTextObj(invBar.transform, "InvTitle", new Vector2(-400, 0), new Vector2(150, 60),
            "Envanter", 24, new Color(0.7f, 0.7f, 0.7f));

        // Envanter slotları
        Inventory invScript = invBar.AddComponent<Inventory>();
        float slotStart = -150f;
        for (int i = 0; i < 4; i++)
        {
            float x = slotStart + i * 130;
            GameObject slot = CreatePanel("Slot_" + i, invBar.transform, new Color(0.2f, 0.2f, 0.2f, 0.5f));
            RectTransform slotRT = slot.GetComponent<RectTransform>();
            slotRT.anchoredPosition = new Vector2(x, 0);
            slotRT.sizeDelta = new Vector2(110, 110);
            slotRT.anchorMin = new Vector2(0.5f, 0.5f);
            slotRT.anchorMax = new Vector2(0.5f, 0.5f);

            Button slotBtn = slot.AddComponent<Button>();
            InventorySlot invSlot = slot.AddComponent<InventorySlot>();
            invSlot.slotIndex = i;
            invSlot.inventory = invScript;

            invScript.slots.Add(slot.GetComponent<Image>());

            // Slot etiketi
            GameObject slotLabel = CreateTextObj(slot.transform, "Label", Vector2.zero, Vector2.zero,
                "", 20, Color.white);
            RectTransform labelRT = slotLabel.GetComponent<RectTransform>();
            labelRT.anchorMin = Vector2.zero; labelRT.anchorMax = Vector2.one;
            labelRT.sizeDelta = Vector2.zero;
            slotLabel.GetComponent<TextMeshProUGUI>().raycastTarget = false;
            invScript.slotLabels.Add(slotLabel.GetComponent<TextMeshProUGUI>());
        }

        // ====== DİYALOG KUTUSU ======
        GameObject dialogPanel = CreatePanel("DialogPanel", canvasObj.transform, new Color(0, 0, 0, 0.85f));
        RectTransform diagRT = dialogPanel.GetComponent<RectTransform>();
        diagRT.anchorMin = new Vector2(0.1f, 0.25f);
        diagRT.anchorMax = new Vector2(0.9f, 0.55f);
        diagRT.sizeDelta = Vector2.zero;

        GameObject diagText = CreateTextObj(dialogPanel.transform, "DialogText",
            new Vector2(0, 10), new Vector2(0, 0), "...", 36, Color.white);
        RectTransform diagTextRT = diagText.GetComponent<RectTransform>();
        diagTextRT.anchorMin = new Vector2(0.05f, 0.2f);
        diagTextRT.anchorMax = new Vector2(0.95f, 0.95f);
        diagTextRT.sizeDelta = Vector2.zero;

        GameObject diagCloseBtn = CreateButton(dialogPanel.transform, "DialogClose",
            new Vector2(0, 0), new Vector2(200, 60),
            new Color(0.5f, 0.2f, 0.2f), "Kapat", 28);
        RectTransform dcRT = diagCloseBtn.GetComponent<RectTransform>();
        dcRT.anchorMin = new Vector2(0.5f, 0.05f);
        dcRT.anchorMax = new Vector2(0.5f, 0.05f);
        dcRT.anchoredPosition = new Vector2(0, 30);

        DialogBox dialogScript = dialogPanel.AddComponent<DialogBox>();
        dialogScript.panel = dialogPanel;
        dialogScript.messageText = diagText.GetComponent<TextMeshProUGUI>();
        dialogScript.closeButton = diagCloseBtn.GetComponent<Button>();
        dialogPanel.SetActive(false);

        // ====== KASA POPUP ======
        GameObject safePanel = CreatePanel("SafePopup", canvasObj.transform, new Color(0, 0, 0, 0.9f));
        Stretch(safePanel);

        GameObject safeWindow = CreatePanel("SafeWindow", safePanel.transform, new Color(0.15f, 0.15f, 0.18f));
        safeWindow.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 400);

        CreateTextObj(safeWindow.transform, "SafeTitle", new Vector2(0, 130), new Vector2(400, 60),
            "Kasa Şifresi", 42, Color.white);

        // Input field
        GameObject inputObj = new GameObject("CodeInput", typeof(RectTransform), typeof(CanvasRenderer),
            typeof(Image), typeof(TMP_InputField));
        inputObj.transform.SetParent(safeWindow.transform, false);
        inputObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 30);
        inputObj.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 70);
        inputObj.GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f);

        // Input text area
        GameObject inputTextArea = new GameObject("Text Area", typeof(RectTransform));
        inputTextArea.transform.SetParent(inputObj.transform, false);
        RectTransform taRt = inputTextArea.GetComponent<RectTransform>();
        taRt.anchorMin = Vector2.zero; taRt.anchorMax = Vector2.one;
        taRt.sizeDelta = new Vector2(-16, -8);

        GameObject inputText = CreateTextObj(inputTextArea.transform, "Text", Vector2.zero, Vector2.zero,
            "", 36, Color.white);
        RectTransform itRT = inputText.GetComponent<RectTransform>();
        itRT.anchorMin = Vector2.zero; itRT.anchorMax = Vector2.one;
        itRT.sizeDelta = Vector2.zero;

        GameObject placeholder = CreateTextObj(inputTextArea.transform, "Placeholder", Vector2.zero, Vector2.zero,
            "_ _ _", 36, new Color(0.5f,0.5f,0.5f));
        RectTransform phRT = placeholder.GetComponent<RectTransform>();
        phRT.anchorMin = Vector2.zero; phRT.anchorMax = Vector2.one;
        phRT.sizeDelta = Vector2.zero;

        TMP_InputField inputField = inputObj.GetComponent<TMP_InputField>();
        inputField.textComponent = inputText.GetComponent<TextMeshProUGUI>();
        inputField.textViewport = taRt;
        inputField.placeholder = placeholder.GetComponent<TextMeshProUGUI>();
        inputField.characterLimit = 3;
        inputField.contentType = TMP_InputField.ContentType.IntegerNumber;

        GameObject safeFeedback = CreateTextObj(safeWindow.transform, "Feedback",
            new Vector2(0, -30), new Vector2(400, 40), "", 28, new Color(1f, 0.3f, 0.3f));

        GameObject safeSubmitBtn = CreateButton(safeWindow.transform, "SubmitBtn",
            new Vector2(-100, -110), new Vector2(170, 60),
            new Color(0.2f, 0.7f, 0.2f), "Gönder", 28);

        GameObject safeCancelBtn = CreateButton(safeWindow.transform, "CancelBtn",
            new Vector2(100, -110), new Vector2(170, 60),
            new Color(0.7f, 0.2f, 0.2f), "İptal", 28);

        SafePanel safeScript = safePanel.AddComponent<SafePanel>();
        safeScript.codeInput = inputField;
        safeScript.submitButton = safeSubmitBtn.GetComponent<Button>();
        safeScript.cancelButton = safeCancelBtn.GetComponent<Button>();
        safeScript.feedbackText = safeFeedback.GetComponent<TextMeshProUGUI>();
        safePanel.SetActive(false);

        // Kasa interactable'ına referansı bağla
        Interactable safeInteractable = GameObject.Find("Kasa")?.GetComponent<Interactable>();
        if (safeInteractable != null) safeInteractable.safePopup = safePanel;

        // ====== TABLO PUZZLE PANELİ ======
        GameObject puzzlePanel = CreatePanel("PuzzlePanel", canvasObj.transform, new Color(0.08f, 0.08f, 0.10f));
        Stretch(puzzlePanel);

        // Geri Butonu (sol üst)
        GameObject puzzleBackBtn = CreateButton(puzzlePanel.transform, "BackButton",
            new Vector2(-400, 400), new Vector2(160, 70),
            new Color(0.5f, 0.2f, 0.2f), "← GERİ", 28);

        // Tablo – 2 Parça (Sol + Sağ) yan yana
        // Sol parça (yerinde kalır)
        GameObject paintingLeft = CreatePanel("PaintingLeft", puzzlePanel.transform, new Color(0.50f, 0.35f, 0.18f));
        RectTransform plRT = paintingLeft.GetComponent<RectTransform>();
        plRT.anchoredPosition = new Vector2(-120, 30);
        plRT.sizeDelta = new Vector2(230, 400);
        CreateTextObj(paintingLeft.transform, "ArtLeft", Vector2.zero, Vector2.zero,
            "🖼️\nSol\nParça", 28, new Color(0.9f, 0.8f, 0.6f));
        RectTransform artLeftTR = paintingLeft.transform.GetChild(0).GetComponent<RectTransform>();
        artLeftTR.anchorMin = Vector2.zero; artLeftTR.anchorMax = Vector2.one; artLeftTR.sizeDelta = Vector2.zero;
        paintingLeft.transform.GetChild(0).GetComponent<TextMeshProUGUI>().raycastTarget = false;

        // Sağ parça (bu düşecek – tıklanabilir)
        GameObject paintingRight = CreatePanel("PaintingRight", puzzlePanel.transform, new Color(0.45f, 0.30f, 0.15f));
        RectTransform prRT = paintingRight.GetComponent<RectTransform>();
        prRT.anchoredPosition = new Vector2(112, 30);
        prRT.sizeDelta = new Vector2(230, 400);
        Button paintingClickBtn = paintingRight.AddComponent<Button>();
        CreateTextObj(paintingRight.transform, "ArtRight", Vector2.zero, Vector2.zero,
            "🖼️\nSağ\nParça\n\n✂ Kes", 26, new Color(0.9f, 0.8f, 0.6f));
        RectTransform artRightTR = paintingRight.transform.GetChild(0).GetComponent<RectTransform>();
        artRightTR.anchorMin = Vector2.zero; artRightTR.anchorMax = Vector2.one; artRightTR.sizeDelta = Vector2.zero;
        paintingRight.transform.GetChild(0).GetComponent<TextMeshProUGUI>().raycastTarget = false;

        // Kesim çizgisi (dikey – iki parça arasında)
        GameObject cutLine = CreatePanel("CutLine", puzzlePanel.transform, new Color(0.8f, 0.2f, 0.2f, 0.6f));
        cutLine.GetComponent<RectTransform>().anchoredPosition = new Vector2(-3, 30);
        cutLine.GetComponent<RectTransform>().sizeDelta = new Vector2(4, 420);

        // ON/OFF Toggle (sol taraf)
        GameObject toggleArea = new GameObject("ToggleArea", typeof(RectTransform));
        toggleArea.transform.SetParent(puzzlePanel.transform, false);
        toggleArea.GetComponent<RectTransform>().anchoredPosition = new Vector2(-380, -100);

        GameObject toggleOnBtn = CreateButton(toggleArea.transform, "ToggleON",
            new Vector2(0, 40), new Vector2(130, 55),
            new Color(0.15f, 0.65f, 0.15f), "ON", 26);

        GameObject toggleOffBtn = CreateButton(toggleArea.transform, "ToggleOFF",
            new Vector2(0, -25), new Vector2(130, 55),
            new Color(0.65f, 0.15f, 0.15f), "OFF", 26);

        GameObject toggleStatus = CreateTextObj(toggleArea.transform, "ToggleStatus",
            new Vector2(0, 90), new Vector2(200, 40), "Bıçak: KAPALI", 22, new Color(0.8f, 0.8f, 0.8f));

        // PaintingPuzzle script bağla
        PaintingPuzzle puzzleScript = puzzlePanel.AddComponent<PaintingPuzzle>();
        puzzleScript.puzzlePanel = puzzlePanel;
        puzzleScript.paintingLeft = plRT;
        puzzleScript.paintingRight = prRT;
        puzzleScript.toggleOnButton = toggleOnBtn.GetComponent<Button>();
        puzzleScript.toggleOffButton = toggleOffBtn.GetComponent<Button>();
        puzzleScript.toggleStatusText = toggleStatus.GetComponent<TextMeshProUGUI>();
        puzzleScript.backButton = puzzleBackBtn.GetComponent<Button>();

        // Tablo butonunu script'e referans olarak ver (script kendi Start()'ında bağlar)
        puzzleScript.paintingRightButton = paintingClickBtn;

        // Kitaplık objesine puzzle referansını ver
        PuzzleOpener kitaplikOpener = GameObject.Find("Kitaplik")?.GetComponent<PuzzleOpener>();
        if (kitaplikOpener != null)
            kitaplikOpener.targetPuzzle = puzzleScript;

        puzzlePanel.SetActive(false);


        // ====== KAZANMA EKRANI ======
        GameObject winScreen = CreatePanel("WinScreen", canvasObj.transform, new Color(0, 0, 0, 0.9f));
        Stretch(winScreen);
        CreateTextObj(winScreen.transform, "WinTitle",
            new Vector2(0, 100), new Vector2(800, 200),
            "ODADAN KAÇTIN!", 80, new Color(0.3f, 1f, 0.3f));
        CreateTextObj(winScreen.transform, "WinSub",
            new Vector2(0, -50), new Vector2(800, 100),
            "Tebrikler! Bulmacaları çözdün ve odadan kaçtın.", 32, Color.white);
        winScreen.SetActive(false);

        // ====== GAME MANAGER ======
        GameManager gm = root.AddComponent<GameManager>();
        gm.inventory = invScript;
        gm.dialogBox = dialogScript;

        // ====== ROOM NAVIGATOR ======
        RoomNavigator nav = root.AddComponent<RoomNavigator>();
        nav.walls = wallPanels;
        nav.leftArrow = leftArrow.GetComponent<Button>();
        nav.rightArrow = rightArrow.GetComponent<Button>();
        nav.wallLabel = wallLabelObj.GetComponent<TextMeshProUGUI>();
        nav.winScreen = winScreen;
        gm.roomNavigator = nav;

        // ====== EVENT SYSTEM ======
        if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            var t = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (t != null) es.AddComponent(t);
            else es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // ====== KUZEY DUVAR (METRONOME/TERAZI) ======
        UI_MetronomeSetup.CreateUIMetronomePuzzle();

        Selection.activeGameObject = root;
        Debug.Log("Cube Escape sahne kurulumu tamamlandı!");
    }

    // =================== HELPER FUNCTIONS ===================

    static void DestroyIfExists(string name)
    {
        GameObject go = GameObject.Find(name);
        if (go != null) Object.DestroyImmediate(go);
    }

    static GameObject CreateCanvas(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        Canvas c = obj.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler sc = obj.AddComponent<CanvasScaler>();
        sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        sc.referenceResolution = new Vector2(1080, 1920);
        sc.matchWidthOrHeight = 0.5f;
        obj.AddComponent<GraphicRaycaster>();
        return obj;
    }

    static GameObject CreatePanel(string name, Transform parent, Color color)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        obj.transform.SetParent(parent, false);
        obj.GetComponent<Image>().color = color;
        return obj;
    }

    static void Stretch(GameObject obj)
    {
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
    }

    static GameObject CreateButton(Transform parent, string name, Vector2 pos, Vector2 size, Color bgColor, string label, int fontSize)
    {
        GameObject obj = CreatePanel(name, parent, bgColor);
        obj.GetComponent<RectTransform>().anchoredPosition = pos;
        obj.GetComponent<RectTransform>().sizeDelta = size;
        obj.AddComponent<Button>();

        GameObject text = CreateTextObj(obj.transform, "Text", Vector2.zero, Vector2.zero, label, fontSize, Color.white);
        RectTransform trt = text.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one; trt.sizeDelta = Vector2.zero;
        text.GetComponent<TextMeshProUGUI>().raycastTarget = false;

        return obj;
    }

    static GameObject CreateTextObj(Transform parent, string name, Vector2 pos, Vector2 size, string text, int fontSize, Color color)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        return obj;
    }

    static Interactable CreateInteractableButton(Transform parent, string name, Vector2 pos, Vector2 size, Color color, int wallIndex)
    {
        GameObject obj = CreatePanel(name, parent, color);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        // Kenar efekti
        Shadow shadow = obj.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.5f);
        shadow.effectDistance = new Vector2(0, -5);

        Button btn = obj.AddComponent<Button>();
        Interactable inter = obj.AddComponent<Interactable>();
        inter.wallIndex = wallIndex;
        inter.myImage = obj.GetComponent<Image>();

        // OnClick'e bağla
        btn.onClick.AddListener(() => inter.OnClicked());

        return inter;
    }

    static void CreateLabel(Transform parent, string text, int fontSize, Color color)
    {
        GameObject obj = CreateTextObj(parent, "Label", Vector2.zero, Vector2.zero, text, fontSize, color);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.sizeDelta = Vector2.zero;
        obj.GetComponent<TextMeshProUGUI>().raycastTarget = false;
    }

    static void CreateWallTitle(Transform parent, string title)
    {
        GameObject obj = CreateTextObj(parent, "WallTitle", new Vector2(0, 400), new Vector2(500, 60),
            title, 32, new Color(0.6f, 0.6f, 0.6f, 0.5f));
        obj.GetComponent<TextMeshProUGUI>().raycastTarget = false;
    }
}
