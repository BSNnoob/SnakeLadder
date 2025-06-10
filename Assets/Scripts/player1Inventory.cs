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

    private void UseItem(Item item)
    {
        int currentTurn = GameObject.Find("GameControl").GetComponent<GameControl>().whosTurn;

        if (currentTurn != playerNumber)
        {
            return;
        }

        GameObject player = GameObject.Find($"Player{playerNumber}");
        GameControl gameControl = GameObject.Find("GameControl").GetComponent<GameControl>();

        switch (item.itemType)
        {
            case Item.ItemType.DoubleDice:
                GameControl.useDoubleDice = true;
                spawnedDice2 = Instantiate(dice2Prefab, new Vector3(2, 2, 0), Quaternion.identity);
                gameControl.SetDice2Instance(spawnedDice2);
                inventory.RemoveItem(item);
                inventory.OnItemListChanged?.Invoke();
                break;

            case Item.ItemType.AvoidSnake:
                GameControl.useAvoidSnake = true;
                inventory.RemoveItem(item);
                inventory.OnItemListChanged?.Invoke();
                break;

            case Item.ItemType.LadderGrab:
                if (!gameControl.IsLadderNearby(player))
                {
                    return;
                }

                bool used = gameControl.UseLadderGrab(player);
                if (used)
                {
                    inventory.RemoveItem(item);
                    inventory.OnItemListChanged?.Invoke();
                }
                break;
        }
    }
}
