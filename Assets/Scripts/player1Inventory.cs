using UnityEngine;

public class player1Inventory : MonoBehaviour
{
    [SerializeField] private UI_Inventory1 uI_Inventory;
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
        int currentTurn = GameObject.Find("GameControl").GetComponent<GameControl>().whosTurn;

        if (currentTurn != playerNumber)
        {
            Debug.Log($"Player {playerNumber} tried to use an item on Player {currentTurn}'s turn — Not allowed.");
            return;
        }

        switch (item.itemType)
        {
            case Item.ItemType.DoubleDice:
                GameControl.useDoubleDice = true;
                spawnedDice2 = Instantiate(dice2Prefab, new Vector3(2, 2, 0), Quaternion.identity);
                GameObject.Find("GameControl").GetComponent<GameControl>().SetDice2Instance(spawnedDice2);
                inventory.RemoveItem(item);
                inventory.OnItemListChanged?.Invoke();
                break;
            case Item.ItemType.AvoidSnake:
                GameControl.useAvoidSnake = true;
                inventory.RemoveItem(item);
                inventory.OnItemListChanged?.Invoke();
                Debug.Log($"avoid snake {GameControl.useAvoidSnake}");
                break;
            case Item.ItemType.LadderGrab:

                GameObject player = GameObject.Find("Player1");
                GameControl gameControl = GameObject.Find("GameControl").GetComponent<GameControl>();

                // Check if a ladder is nearby BEFORE using the item
                if (!gameControl.IsLadderNearby(player))
                {
                    Debug.Log("🚫 LadderGrab not used: no ladder nearby.");
                    return; // Don't use or remove the item
                }

                gameControl.UseLadderGrab(player);

                inventory.RemoveItem(item); // ✅ Only remove when actually used
                inventory.OnItemListChanged?.Invoke();
                break;
    

        }
    }
}
