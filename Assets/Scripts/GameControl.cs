using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Cinemachine;
using System.Collections;

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

            var path1 = player1.GetComponent<FollowThePath>();
player1StartWaypoint = path1.waypointIndex - 1;

if (snakePositions.ContainsKey(path1.waypointIndex))
{
    int newWaypoint = snakePositions[path1.waypointIndex];
    StartCoroutine(AnimateSnakeBite(player1, newWaypoint));

    Debug.Log($"🐍 Player 1 hit a snake! Sent from {path1.waypointIndex} to {newWaypoint}");
}
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
            CheckForSnake(player1);
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
            CheckForSnake(player1);
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
            CheckForSnake(player1);
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
    private void CheckForSnake(GameObject player)
    {
        var path = player.GetComponent<FollowThePath>();
        int currentWaypoint = path.waypointIndex - 1;

        Debug.Log($"[SnakeCheck] {player.name} at waypointIndex {path.waypointIndex}, currentWaypoint: {currentWaypoint}");

        if (snakePositions.ContainsKey(currentWaypoint))
        {
            int newWaypoint = snakePositions[currentWaypoint];
            path.waypointIndex = newWaypoint;
            player.transform.position = path.waypoints[newWaypoint].position;
            Debug.Log($"[SnakeTrigger] {player.name} hit a snake! Moved from {currentWaypoint} to {newWaypoint}");
        }
    }
private IEnumerator AnimateSnakeBite(GameObject player, int targetWaypointIndex)
{
    var path = player.GetComponent<FollowThePath>();
    path.moveAllowed = false;

    Vector3 originalPos = player.transform.position;
    float shakeDuration = 0.4f;
    float shakeMagnitude = 0.2f;
    float elapsed = 0f;

    // 🔥 Find Snake Animator from Waypoint 3
    Transform snakeWaypoint = path.waypoints[2]; // Assumes snake is on waypoint 3
    Animator snakeAnimator = snakeWaypoint.Find("Snake")?.GetComponent<Animator>();
    
    if (snakeAnimator != null)
    {
        snakeAnimator.Play("Attack"); // Make sure your attack animation is called exactly "Attack"
    }

    // 🐍 Player shake effect
    while (elapsed < shakeDuration)
    {
        float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
        float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;
        player.transform.position = originalPos + new Vector3(offsetX, offsetY, 0);
        elapsed += Time.deltaTime;
        yield return null;
    }

    player.transform.position = originalPos;

    // Move player to new position
    Vector3 startPos = player.transform.position;
    Vector3 targetPos = path.waypoints[targetWaypointIndex].position;
    float moveDuration = 0.5f;
    elapsed = 0f;

    while (elapsed < moveDuration)
    {
        player.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveDuration);
        elapsed += Time.deltaTime;
        yield return null;
    }

    player.transform.position = targetPos;
    path.waypointIndex = targetWaypointIndex;

    // ⏸️ Optional: Reset snake to idle
    if (snakeAnimator != null)
    {
        snakeAnimator.Play("Idle");
    }
}




    private static Dictionary<int, int> snakePositions = new Dictionary<int, int>()
    {
        { 3, 0 }, // Snake from 3 to 1
        // Add more here
    };
}
