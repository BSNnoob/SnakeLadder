using UnityEngine;

public class player2Inventory : MonoBehaviour
{
    [SerializeField] private UI_Inventory2 uI_Inventory;
    private Inventory inventory;
    [SerializeField] private GameObject dice2Prefab;
    public int playerNumber;
    private GameObject spawnedDice2;

    private void Awake()
    {
        inventory = new Inventory(UseItem);
        uI_Inventory.SetInventory(inventory);
    }
    public Inventory GetInventory() {
        return inventory;
    }

    private void UseItem(Item item){
        switch (item.itemType){
            case Item.ItemType.DoubleDice:
                GameControl.useDoubleDice = true;
                spawnedDice2 = Instantiate(dice2Prefab, new Vector3(2, 2, 0), Quaternion.identity);
                GameObject.Find("GameControl").GetComponent<GameControl>().SetDice2Instance(spawnedDice2);
                inventory.OnItemListChanged?.Invoke();
                break;
            case Item.ItemType.AvoidSnake:
                Debug.Log("A");
                break;
            case Item.ItemType.LadderGrab:
                Debug.Log("A");
                break;
        }
    }
}
