using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SafePanel : MonoBehaviour
{
    public TMP_InputField codeInput;
    public Button submitButton;
    public Button cancelButton;
    public TextMeshProUGUI feedbackText;

    private void Start()
    {
        if (submitButton != null) submitButton.onClick.AddListener(OnSubmit);
        if (cancelButton != null) cancelButton.onClick.AddListener(OnCancel);
        gameObject.SetActive(false);
    }

    private void OnSubmit()
    {
        if (codeInput == null) return;
        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        string entered = codeInput.text.Trim();
        if (entered == gm.safeCode)
        {
            gm.safeOpened = true;
            gm.inventory.AddItem("Anahtar");
            gm.dialogBox.Show("Kasa açıldı! Anahtar aldın!", 3f);
            gameObject.SetActive(false);
        }
        else
        {
            if (feedbackText != null)
                feedbackText.text = "Yanlış şifre!";
        }
    }

    private void OnCancel()
    {
        gameObject.SetActive(false);
    }
}
