using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RaidInventory inventory;
    [SerializeField] private Transform listContainer;
    [SerializeField] private InventorySlotUI slotPrefab;

    [Header("Weight Display")]
    [SerializeField] private Slider weightBar;
    [SerializeField] private Text weightLabel;

    [Header("Text Display")]
    public TMP_Text itemText;
    public TMP_Text catergoryText;

    private void OnEnable()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged.AddListener(HandleInventoryChanged);

            HandleInventoryChanged(inventory.CurrentWeight, inventory.MaxWeight);
        }
    }

    private void OnDisable()
    {
        if (inventory != null)
            inventory.OnInventoryChanged.RemoveListener(HandleInventoryChanged);
    }

    private void HandleInventoryChanged(float currentWeight, float maxWeight)
    {
        RebuildList();
        UpdateWeightDisplay(currentWeight, maxWeight);
    }
    private void RebuildList()
    {
        if (listContainer == null || slotPrefab == null) return;

        // Clear existing rows.
        for (int i = listContainer.childCount - 1; i >= 0; i--)
            Destroy(listContainer.GetChild(i).gameObject);

        var items = inventory.CarriedItems;
        for (int i = 0; i < items.Count; i++)
        {
            InventorySlotUI slot = Instantiate(slotPrefab, listContainer);
            slot.Setup(items[i], i, this);
        }
    }

    private void UpdateWeightDisplay(float currentWeight, float maxWeight)
    {
        if (weightBar != null)
            weightBar.value = maxWeight > 0f ? currentWeight / maxWeight : 0f;

        if (weightLabel != null)
            weightLabel.text = $"{currentWeight:0.#} / {maxWeight:0.#} kg";
    }

    public void RequestDrop(int index)
    {
        inventory.RemoveItemAt(index);
    }

#if UNITY_EDITOR
    [ContextMenu("Add Random Item")]
    public void AddRandomItem()
    {
        if (inventory != null)
        {
            var randomItem = ItemDatabase.GetRandomItem();
            inventory.TryAddItem(randomItem);
        }
    }
    [ContextMenu("Add Random Items")]
    public void AddRandomItems()
    {
        if (inventory != null)
        {
            for (int i = 0; i < 25; i++)
            {
                var randomItem = ItemDatabase.GetRandomItem();
                inventory.TryAddItem(randomItem);
            }
        }
    }
#endif
}