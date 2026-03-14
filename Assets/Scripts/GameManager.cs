using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public int currentWallIndex = 0; // 0=Kuzey, 1=Doğu, 2=Güney, 3=Batı
    public bool hasKey = false;
    public bool hasNote = false;
    public bool safeOpened = false;
    public bool gameWon = false;
    public string safeCode = "371";

    [Header("References")]
    public RoomNavigator roomNavigator;
    public Inventory inventory;
    public DialogBox dialogBox;

    public event Action<int> OnWallChanged;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void ChangeWall(int direction)
    {
        if (gameWon) return;
        currentWallIndex = (currentWallIndex + direction + 4) % 4;
        OnWallChanged?.Invoke(currentWallIndex);
    }

    public string GetWallName(int index)
    {
        switch(index)
        {
            case 0: return "Kuzey";
            case 1: return "Doğu";
            case 2: return "Güney";
            case 3: return "Batı";
            default: return "";
        }
    }

    public void WinGame()
    {
        gameWon = true;
        if (roomNavigator != null)
            roomNavigator.ShowWinScreen();
    }
}
