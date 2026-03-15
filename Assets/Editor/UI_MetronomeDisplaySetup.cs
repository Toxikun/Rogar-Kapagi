#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sets up a proper UI-based MetronomeDisplay on Wall_South/Kasa.
/// Hides old Metronom false/true objects and creates a new Image-based animation.
/// </summary>
public class UI_MetronomeDisplaySetup
{
    [MenuItem("Tools/Setup Metronome Display (Güney Duvar)")]
    public static void Setup()
    {
        // 1. Find Wall_South/Kasa
        GameObject kasa = GameObject.Find("Kasa");
        if (kasa == null)
        {
            // Try deeper path
            var wallSouth = GameObject.Find("Wall_South");
            if (wallSouth != null)
                kasa = wallSouth.transform.Find("Kasa")?.gameObject;
        }
        if (kasa == null)
        {
            Debug.LogError("Kasa not found on Wall_South!");
            return;
        }

        // 2. Hide old Metronom false/true (don't delete)
        Transform metronomFalseTr = kasa.transform.Find("Metronom false");
        Transform metronomTrueTr = kasa.transform.Find("Metronom true");
        if (metronomFalseTr != null)
        {
            metronomFalseTr.gameObject.SetActive(false);
            EditorUtility.SetDirty(metronomFalseTr.gameObject);
            Debug.Log("Hidden: Metronom false");
        }
        if (metronomTrueTr != null)
        {
            metronomTrueTr.gameObject.SetActive(false);
            EditorUtility.SetDirty(metronomTrueTr.gameObject);
            Debug.Log("Hidden: Metronom true");
        }

        // 3. Remove old MetronomeDisplay if exists
        Transform oldDisplay = kasa.transform.Find("MetronomeDisplayUI");
        if (oldDisplay != null)
        {
            Object.DestroyImmediate(oldDisplay.gameObject);
            Debug.Log("Removed old MetronomeDisplayUI.");
        }

        // 4. Create new UI Image for the metronome
        GameObject displayGo = new GameObject("MetronomeDisplayUI", typeof(RectTransform));
        displayGo.transform.SetParent(kasa.transform, false);

        RectTransform rt = displayGo.GetComponent<RectTransform>();
        // Position above the Kasa label area (matching old Metronom false position)
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0f, 501f); // Same Y as old Metronom false
        rt.sizeDelta = new Vector2(636f, 407f);       // Same size as old Metronom false

        Image img = displayGo.AddComponent<Image>();
        img.raycastTarget = false; // Don't block clicks
        img.preserveAspect = true;

        // 5. Load all metronome sprites (ordered -6 to 6)
        string basePath = "Assets/kege sprites/metronome/";
        string[] frameNames = {
            "metronome -6", "metronome -5", "metronome -4", "metronome -3",
            "metronome -2", "metronome -1", "metronome 0",
            "metronome 1", "metronome 2", "metronome 3",
            "metronome 4", "metronome 5", "metronome 6"
        };

        Sprite[] frames = new Sprite[frameNames.Length];
        for (int i = 0; i < frameNames.Length; i++)
        {
            string path = basePath + frameNames[i] + ".png";
            Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (spr == null)
            {
                // Try loading as Texture2D and getting the sprite
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (tex != null)
                {
                    // Get all objects at this path (sprite is a sub-asset of the texture)
                    Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
                    foreach (var a in assets)
                    {
                        if (a is Sprite s)
                        {
                            spr = s;
                            break;
                        }
                    }
                }
            }
            frames[i] = spr;
            if (spr == null)
                Debug.LogWarning($"Could not load sprite: {path}");
            else
                Debug.Log($"Loaded: {path}");
        }

        // 6. Add MetronomeDisplay script
        MetronomeDisplay display = displayGo.AddComponent<MetronomeDisplay>();
        display.frames = frames;
        display.displayImage = img;
        display.frameInterval = 0.08f;

        // Set initial sprite
        if (frames.Length > 0 && frames[0] != null)
            img.sprite = frames[0];

        EditorUtility.SetDirty(displayGo);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene()
        );

        Debug.Log("✓ MetronomeDisplay setup complete on Wall_South/Kasa!");
    }
}
#endif
