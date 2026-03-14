using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogBox : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI messageText;
    public Button closeButton;

    private Coroutine autoHideCoroutine;

    private void Start()
    {
        if (panel != null) panel.SetActive(false);
        if (closeButton != null) closeButton.onClick.AddListener(Hide);
    }

    public void Show(string message, float duration = 0f)
    {
        if (panel == null) return;
        panel.SetActive(true);
        if (messageText != null) messageText.text = message;

        if (autoHideCoroutine != null) StopCoroutine(autoHideCoroutine);
        if (duration > 0) autoHideCoroutine = StartCoroutine(AutoHide(duration));
    }

    public void Hide()
    {
        if (panel != null) panel.SetActive(false);
    }

    private IEnumerator AutoHide(float delay)
    {
        yield return new WaitForSeconds(delay);
        Hide();
    }
}
