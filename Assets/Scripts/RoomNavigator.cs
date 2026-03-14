using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class RoomNavigator : MonoBehaviour
{
    [Header("Walls")]
    public List<GameObject> walls = new List<GameObject>(); // 4 duvar paneli

    [Header("UI")]
    public Button leftArrow;
    public Button rightArrow;
    public TextMeshProUGUI wallLabel;
    public GameObject winScreen;

    private void Start()
    {
        if (leftArrow != null) leftArrow.onClick.AddListener(() => Navigate(-1));
        if (rightArrow != null) rightArrow.onClick.AddListener(() => Navigate(1));

        GameManager gm = GameManager.Instance;
        if (gm != null)
        {
            gm.OnWallChanged += UpdateView;
            gm.roomNavigator = this;
        }

        UpdateView(0);
    }

    private void Navigate(int dir)
    {
        GameManager gm = GameManager.Instance;
        if (gm != null) gm.ChangeWall(dir);
    }

    private void UpdateView(int wallIndex)
    {
        for (int i = 0; i < walls.Count; i++)
        {
            if (walls[i] != null)
                walls[i].SetActive(i == wallIndex);
        }

        GameManager gm = GameManager.Instance;
        if (wallLabel != null && gm != null)
            wallLabel.text = gm.GetWallName(wallIndex);
    }

    public void ShowWinScreen()
    {
        if (winScreen != null) winScreen.SetActive(true);
    }
}
