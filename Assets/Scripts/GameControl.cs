using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
public class GameControl : MonoBehaviour
{
    private static GameObject whoWinsTextShadow, player1MoveText, player2MoveText, player3MoveText, player4MoveText;
    private static GameObject player1, player2, player3, player4;
    private static GameObject dice2;

    public static int diceSideThrown = 0;
    public static int player1StartWaypoint = 0;
    public static int player2StartWaypoint = 0;
    public static int player3StartWaypoint = 0;
    public static int player4StartWaypoint = 0;

    public static bool gameOver = false;
    public int whosTurn = 1;

    private player1Inventory player1InventoryScript;
    private player2Inventory player2InventoryScript;
    private player3Inventory player3InventoryScript;
    private player4Inventory player4InventoryScript;

    public static bool useDoubleDice = false;
    public static bool hasReceivedDoubleDice = false;
    private GameObject dice2Instance;

    public CinemachineCamera cmCamera;

    public void UpdateCameraTarget()
    {
        GameObject player = null;

        switch (whosTurn)
        {
            case 1: player = GameObject.Find("Player1"); break;
            case 2: player = GameObject.Find("Player2"); break;
            case 3: player = GameObject.Find("Player3"); break;
            case 4: player = GameObject.Find("Player4"); break;
        }

        if (player != null && cmCamera != null)
        {
            Transform followTarget = player.transform.Find("CameraFollowPoint");
            if (followTarget == null) followTarget = player.transform;

            cmCamera.Follow = followTarget;
        }
    }

    void Start()
    {
        player1InventoryScript = GameObject.Find("Player1").GetComponent<player1Inventory>();
        player2InventoryScript = GameObject.Find("Player2").GetComponent<player2Inventory>();
        player3InventoryScript = GameObject.Find("Player3").GetComponent<player3Inventory>();
        player4InventoryScript = GameObject.Find("Player4").GetComponent<player4Inventory>();

        whoWinsTextShadow = GameObject.Find("WhoWinsText");
        player1MoveText = GameObject.Find("Player1MoveText");
        player2MoveText = GameObject.Find("Player2MoveText");
        player3MoveText = GameObject.Find("Player3MoveText");
        player4MoveText = GameObject.Find("Player4MoveText");

        player1 = GameObject.Find("Player1");
        player2 = GameObject.Find("Player2");
        player3 = GameObject.Find("Player3");
        player4 = GameObject.Find("Player4");

        player1.GetComponent<FollowThePath>().moveAllowed = false;
        player2.GetComponent<FollowThePath>().moveAllowed = false;
        player3.GetComponent<FollowThePath>().moveAllowed = false;
        player4.GetComponent<FollowThePath>().moveAllowed = false;

        whoWinsTextShadow.SetActive(false);
        player1MoveText.SetActive(true);
        player2MoveText.SetActive(false);
        player3MoveText.SetActive(false);
        player4MoveText.SetActive(false);
        UpdateCameraTarget();
    }

    void Update()
    {
        if (whosTurn == 1 && player1.GetComponent<FollowThePath>().waypointIndex > player1StartWaypoint + diceSideThrown)
        {
            player1.GetComponent<FollowThePath>().moveAllowed = false;
            player1MoveText.SetActive(false);
            player2MoveText.SetActive(true);
            whosTurn++;
            UpdateCameraTarget();

            player1StartWaypoint = player1.GetComponent<FollowThePath>().waypointIndex - 1;

            if (player1.GetComponent<FollowThePath>().waypointIndex == 5 && !hasReceivedDoubleDice)
            {
                player1InventoryScript.GetInventory().AddItem(new Item { itemType = Item.ItemType.DoubleDice });
                hasReceivedDoubleDice = true;
            }

            GameObject.Find("DiceCheckZone").GetComponent<DiceCheckZoneScript>().rolled = true;
        }

        if (whosTurn == 2 && player2.GetComponent<FollowThePath>().waypointIndex > player2StartWaypoint + diceSideThrown)
        {
            player2.GetComponent<FollowThePath>().moveAllowed = false;
            player2MoveText.SetActive(false);
            player3MoveText.SetActive(true);
            whosTurn++;
            UpdateCameraTarget();
            player2StartWaypoint = player2.GetComponent<FollowThePath>().waypointIndex - 1;

            if (player2.GetComponent<FollowThePath>().waypointIndex == 5 && !hasReceivedDoubleDice)
            {
                player2InventoryScript.GetInventory().AddItem(new Item { itemType = Item.ItemType.DoubleDice });
                hasReceivedDoubleDice = true;
            }

            GameObject.Find("DiceCheckZone").GetComponent<DiceCheckZoneScript>().rolled = true;
        }

        if (whosTurn == 3 && player3.GetComponent<FollowThePath>().waypointIndex > player3StartWaypoint + diceSideThrown)
        {
            player3.GetComponent<FollowThePath>().moveAllowed = false;
            player3MoveText.SetActive(false);
            player4MoveText.SetActive(true);
            whosTurn++;
            UpdateCameraTarget();
            player3StartWaypoint = player3.GetComponent<FollowThePath>().waypointIndex - 1;

            if (player3.GetComponent<FollowThePath>().waypointIndex == 5 && !hasReceivedDoubleDice)
            {
                player3InventoryScript.GetInventory().AddItem(new Item { itemType = Item.ItemType.DoubleDice });
                hasReceivedDoubleDice = true;
            }

            GameObject.Find("DiceCheckZone").GetComponent<DiceCheckZoneScript>().rolled = true;
        }

        if (whosTurn == 4 && player4.GetComponent<FollowThePath>().waypointIndex > player4StartWaypoint + diceSideThrown)
        {
            player4.GetComponent<FollowThePath>().moveAllowed = false;
            player4MoveText.SetActive(false);
            player1MoveText.SetActive(true);
            whosTurn = 1;
            UpdateCameraTarget();
            player4StartWaypoint = player4.GetComponent<FollowThePath>().waypointIndex - 1;

            if (player4.GetComponent<FollowThePath>().waypointIndex == 5 && !hasReceivedDoubleDice)
            {
                player4InventoryScript.GetInventory().AddItem(new Item { itemType = Item.ItemType.DoubleDice });
                hasReceivedDoubleDice = true;
            }

            GameObject.Find("DiceCheckZone").GetComponent<DiceCheckZoneScript>().rolled = true;
        }

        // Win condition check
        if (player1.GetComponent<FollowThePath>().waypointIndex == player1.GetComponent<FollowThePath>().waypoints.Length)
        {
            whoWinsTextShadow.SetActive(true);
            whoWinsTextShadow.GetComponent<Text>().text = "Player 1 Wins";
            gameOver = true;
        }
        if (player2.GetComponent<FollowThePath>().waypointIndex == player2.GetComponent<FollowThePath>().waypoints.Length)
        {
            whoWinsTextShadow.SetActive(true);
            whoWinsTextShadow.GetComponent<Text>().text = "Player 2 Wins";
            gameOver = true;
        }
        if (player3.GetComponent<FollowThePath>().waypointIndex == player3.GetComponent<FollowThePath>().waypoints.Length)
        {
            whoWinsTextShadow.SetActive(true);
            whoWinsTextShadow.GetComponent<Text>().text = "Player 3 Wins";
            gameOver = true;
        }
        if (player4.GetComponent<FollowThePath>().waypointIndex == player4.GetComponent<FollowThePath>().waypoints.Length)
        {
            whoWinsTextShadow.SetActive(true);
            whoWinsTextShadow.GetComponent<Text>().text = "Player 4 Wins";
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
            case 3:
                player3.GetComponent<FollowThePath>().moveAllowed = true;
                break;
            case 4:
                player4.GetComponent<FollowThePath>().moveAllowed = true;
                break;
        }

        GameControl instance = GameObject.Find("GameControl").GetComponent<GameControl>();
        if (instance.dice2Instance != null)
        {
            Destroy(instance.dice2Instance);
            GameControl.useDoubleDice = false;
            instance.dice2Instance = null;
        }
    }

    public void SetDice2Instance(GameObject dice)
    {
        dice2Instance = dice;
    }
}
