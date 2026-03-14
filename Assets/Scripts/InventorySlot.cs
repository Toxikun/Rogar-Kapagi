using UnityEngine;
using UnityEngine.UI;

// Envanter slotuna eklenen küçük yardımcı.
// Start()'da kendi onClick'ini bağlar.
public class InventorySlot : MonoBehaviour
{
    public int slotIndex;
    public Inventory inventory;

    private void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null && inventory != null)
        {
            int idx = slotIndex; // closure için kopyala
            btn.onClick.AddListener(() => inventory.OnSlotClicked(idx));
        }
    }
}
