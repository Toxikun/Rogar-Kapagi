using UnityEngine;
using UnityEngine.UI;

// Kitaplık gibi objelere eklenir.
// Tıklanınca bağlı puzzle'ı açar.
public class PuzzleOpener : MonoBehaviour
{
    public PaintingPuzzle targetPuzzle;

    private void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null && targetPuzzle != null)
        {
            btn.onClick.AddListener(() => targetPuzzle.OpenPuzzle());
        }
    }
}
