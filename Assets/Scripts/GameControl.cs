using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    private static GameObject whoWinsTextShadow, player1MoveText, player2MoveText;
    private static GameObject player1, player2;
    private static GameObject dice2;
    public static int diceSideThrown = 0;
    public static int player1StartWaypoint = 0;
    public static int player2StartWaypoint = 0;

    public static bool gameOver = false;
    public int whosTurn;
    private player1Inventory player1InventoryScript;
    public static bool useDoubleDice = false;
    public static bool hasReceivedDoubleDice = false;
    private GameObject dice2Instance;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player1InventoryScript = GameObject.Find("Player1").GetComponent<player1Inventory>();
        whoWinsTextShadow = GameObject.Find("WhoWinsText");
        player1MoveText = GameObject.Find("Player1MoveText");
        player2MoveText = GameObject.Find("Player2MoveText");
        dice2 = GameObject.Find("dice2(Clone)");

        player1 = GameObject.Find("Player1");
        player2 = GameObject.Find("Player2");

        player1.GetComponent<FollowThePath>().moveAllowed = false;
        player2.GetComponent<FollowThePath>().moveAllowed = false;

        whoWinsTextShadow.gameObject.SetActive(false);
        player1MoveText.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        bool showDice2 = GameControl.useDoubleDice;
        Debug.Log(useDoubleDice);
        Debug.Log(whosTurn);
        if (player1.GetComponent<FollowThePath>().waypointIndex >
        player1StartWaypoint + diceSideThrown && whosTurn == 1)
        {
            Debug.Log(player1.GetComponent<FollowThePath>().waypointIndex);
            player1.GetComponent<FollowThePath>().moveAllowed = false;
            player1MoveText.gameObject.SetActive(false);
            player2MoveText.gameObject.SetActive(true);
            whosTurn *= -1;
            player1StartWaypoint = player1.GetComponent<FollowThePath>().waypointIndex - 1;
            if (player1.GetComponent<FollowThePath>().waypointIndex == 5 && !hasReceivedDoubleDice)
            {
                Inventory player1Inventory = player1InventoryScript.GetInventory();
                player1Inventory.AddItem(new Item { itemType = Item.ItemType.DoubleDice });
                hasReceivedDoubleDice = true;
            }
            GameObject.Find("DiceCheckZone").GetComponent<DiceCheckZoneScript>().rolled = true;
        }

        if (player2.GetComponent<FollowThePath>().waypointIndex >
        player2StartWaypoint + diceSideThrown)
        {
            player2.GetComponent<FollowThePath>().moveAllowed = false;
            player2MoveText.gameObject.SetActive(false);
            player1MoveText.gameObject.SetActive(true);
            whosTurn *= -1;
            player2StartWaypoint = player2.GetComponent<FollowThePath>().waypointIndex - 1;
            GameObject.Find("DiceCheckZone").GetComponent<DiceCheckZoneScript>().rolled = true;
        }
        if (player1.GetComponent<FollowThePath>().waypointIndex ==
        player1.GetComponent<FollowThePath>().waypoints.Length)
        {
            whoWinsTextShadow.gameObject.SetActive(true);
            whoWinsTextShadow.GetComponent<Text>().text = "Player 1 Wins";
            gameOver = true;
        }
        if (player2.GetComponent<FollowThePath>().waypointIndex ==
        player2.GetComponent<FollowThePath>().waypoints.Length)
        {
            whoWinsTextShadow.gameObject.SetActive(true);
            player1MoveText.gameObject.SetActive(false);
            player2MoveText.gameObject.SetActive(false);
            whoWinsTextShadow.GetComponent<Text>().text = "Player 2 Wins";
            gameOver = true;
        }
    }

    public static void MovePlayer(int playerToMove)
    {
        switch (playerToMove)
        {
            case 1:
                player1.GetComponent<FollowThePath>().moveAllowed = true;
                break;

            case 2:
                player2.GetComponent<FollowThePath>().moveAllowed = true;
                break;
        }
        GameControl instance = GameObject.Find("GameControl").GetComponent<GameControl>();
        if (instance.dice2Instance != null)
        {
            Destroy(instance.dice2Instance);
            GameControl.useDoubleDice = false;
            instance.dice2Instance = null; // clean up the reference
        }
    }
    
    public void SetDice2Instance(GameObject dice)
    {
        dice2Instance = dice;
    }
}
