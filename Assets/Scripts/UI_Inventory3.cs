using System;
using CodeMonkey.Utils;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory3 : MonoBehaviour
{
    private Inventory inventory;
    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;
    public event Action OnItemListChanged;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        itemSlotContainer = transform.Find("itemSlotContainer");
        itemSlotTemplate = itemSlotContainer.Find("itemSlotTemplate");
    }
    public void SetInventory(Inventory inventory){
        this.inventory = inventory;
        inventory.OnItemListChanged += RefreshInventoryItems;
        RefreshInventoryItems();
    }
    
    private void RefreshInventoryItems() {
    // Clear old slots
    foreach (Transform child in itemSlotContainer) {
        if (child == itemSlotTemplate) continue;
        Destroy(child.gameObject);
    }

    int x = 0;
    int y = 0;
    float cellSize = 60f;

    foreach (Item item in inventory.GetItemList()) {
        RectTransform slot = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
        slot.gameObject.SetActive(true);

        // Top-left layout: X right, Y down
        slot.anchoredPosition = new Vector2(x * cellSize, -y * cellSize);

        slot.GetComponent<Button_UI>().ClickFunc = () => {
            inventory.UseItem(item);
        };

        Image image = slot.Find("Image").GetComponent<Image>();
        image.sprite = item.GetSprite();

        x++;
        if (x >= 5) { x = 0; y++; }
    }
}




}
