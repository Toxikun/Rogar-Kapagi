using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public List<Image> slots = new List<Image>();
    public List<TextMeshProUGUI> slotLabels = new List<TextMeshProUGUI>();

    private List<string> items = new List<string>();
    private int selectedIndex = -1;

    // Renk map
    private Dictionary<string, Color> itemColors = new Dictionary<string, Color>()
    {
        {"Anahtar", new Color(1f, 0.85f, 0.2f)},
        {"Not", new Color(0.9f, 0.9f, 0.8f)},
    };

    public void AddItem(string itemName)
    {
        if (items.Contains(itemName)) return;
        if (items.Count >= slots.Count) return;

        items.Add(itemName);
        RefreshUI();
    }

    public void RemoveItem(string itemName)
    {
        items.Remove(itemName);
        if (selectedIndex >= items.Count) selectedIndex = -1;
        RefreshUI();
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    public string GetSelectedItem()
    {
        if (selectedIndex >= 0 && selectedIndex < items.Count)
            return items[selectedIndex];
        return null;
    }

    public void OnSlotClicked(int index)
    {
        if (index < items.Count)
        {
            if (selectedIndex == index)
                selectedIndex = -1; // Toggle deselect
            else
                selectedIndex = index;
        }
        RefreshUI();
    }

    private void RefreshUI()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < items.Count)
            {
                string item = items[i];
                Color c = itemColors.ContainsKey(item) ? itemColors[item] : Color.gray;

                slots[i].color = c;
                if (i < slotLabels.Count && slotLabels[i] != null)
                {
                    slotLabels[i].text = item;
                    slotLabels[i].gameObject.SetActive(true);
                }

                // Seçili ise kenarlık efekti
                if (i == selectedIndex)
                    slots[i].transform.localScale = Vector3.one * 1.15f;
                else
                    slots[i].transform.localScale = Vector3.one;
            }
            else
            {
                slots[i].color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
                slots[i].transform.localScale = Vector3.one;
                if (i < slotLabels.Count && slotLabels[i] != null)
                {
                    slotLabels[i].text = "";
                    slotLabels[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
