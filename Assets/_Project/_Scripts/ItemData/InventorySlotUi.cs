using Items;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text catergoryText;
    [SerializeField] private TMP_Text weightText; 
    [SerializeField] private Button dropButton;

    private int myIndex;
    private InventoryUIController owner;

    private ItemData itemData;

    public void Setup(ItemData item, int index, InventoryUIController controller)
    {
        myIndex = index;
        owner = controller;
        itemData = item;

        if(iconImage != null)
        {
            iconImage.sprite = item.icon;
            // iconImage.enabled = item.icon != null;
        }
        if(nameText != null) nameText.text = item.itemName;
        if(catergoryText != null) catergoryText.text = item.catergory.ToString();
        if(weightText != null) weightText.text = $"{item.weight:0.#} kg";

        if(dropButton != null)
        {
            dropButton.onClick.AddListener(() =>
            {
                owner.RequestDrop(myIndex);
                nameText.text = "";
                catergoryText.text = "";
            });
        }

        nameText = owner.itemText;
        catergoryText = owner.catergoryText;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        nameText.text = itemData.itemName;
        catergoryText.text = itemData.catergory.ToString();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        nameText.text = "";
        catergoryText.text = "";
    }
}