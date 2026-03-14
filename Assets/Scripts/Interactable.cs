using UnityEngine;
using UnityEngine.UI;

public enum InteractableType
{
    Clue,        // Tıklayınca mesaj gösterir
    Pickup,      // Tıklayınca envantere eşya ekler
    LockedDoor,  // Doğru eşya ile açılır
    Safe,        // Şifre girişi
    Atmosphere,  // Sadece atmosfer efekti
    Puzzle       // Bulmaca Ekranı açar
}

public class Interactable : MonoBehaviour
{
    [Header("Settings")]
    public InteractableType type;
    public string itemName;           // Pickup: verilecek eşya adı
    public string requiredItem;       // LockedDoor: gerekli eşya adı
    public string clueMessage;        // Clue: gösterilecek mesaj
    public int wallIndex;             // Hangi duvara ait (0-3)

    [Header("State")]
    public bool isUsed = false;

    [Header("References")]
    public Image myImage;
    public GameObject safePopup; // Safe tipi için

    private void Start()
    {
        // Kendi butonuna listener bağla (serialize sorunu çözümü)
        Button btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(OnClicked);
    }

    public void OnClicked()
    {
        if (isUsed && type != InteractableType.Clue && type != InteractableType.Atmosphere) return;

        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        switch (type)
        {
            case InteractableType.Clue:
                gm.dialogBox.Show(clueMessage, 4f);
                break;

            case InteractableType.Pickup:
                gm.inventory.AddItem(itemName);
                gm.dialogBox.Show(itemName + " aldın!", 2f);
                isUsed = true;
                // Objeyi gizle veya rengini değiştir
                if (myImage != null)
                    myImage.color = new Color(myImage.color.r, myImage.color.g, myImage.color.b, 0.3f);
                break;

            case InteractableType.LockedDoor:
                string selected = gm.inventory.GetSelectedItem();
                if (selected == requiredItem)
                {
                    gm.dialogBox.Show("Kapı açıldı! Özgürsün!", 3f);
                    gm.inventory.RemoveItem(requiredItem);
                    isUsed = true;
                    gm.WinGame();
                }
                else if (string.IsNullOrEmpty(selected))
                {
                    gm.dialogBox.Show("Kapı kilitli. Bir anahtara ihtiyacın var.", 3f);
                }
                else
                {
                    gm.dialogBox.Show("Bu eşya burada işe yaramıyor.", 2f);
                }
                break;

            case InteractableType.Safe:
                if (safePopup != null)
                    safePopup.SetActive(true);
                break;

            case InteractableType.Atmosphere:
                gm.dialogBox.Show(clueMessage, 3f);
                break;

            case InteractableType.Puzzle:
                // We'll use safePopup as the generic "Puzzle Panel" to open,
                // or you can add a new GameObject field. Re-using safePopup for efficiency:
                if (safePopup != null)
                    safePopup.SetActive(true);
                break;
        }
    }
}
