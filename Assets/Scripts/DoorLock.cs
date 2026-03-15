using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DoorLock : MonoBehaviour
{
    public TMP_InputField codeInput;
    public Button submitButton;
    public Button cancelButton;
    public TextMeshProUGUI feedbackText;

    private const string CORRECT_PASSWORD = "1312";

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
        if (entered == CORRECT_PASSWORD)
        {
            gm.WinGame();
            gameObject.SetActive(false);
        }
        else
        {
            if (feedbackText != null)
                feedbackText.text = "Yanlış şifre!";
                
            if (gm.dialogBox != null)
                gm.dialogBox.Show("Bu şifre yanlış gibi görünüyor...", 2f);
        }
    }

    private void OnCancel()
    {
        gameObject.SetActive(false);
    }
}
