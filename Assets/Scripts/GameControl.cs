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
    public static bool useAvoidSnake = false;
    public static bool useLadderGrab = false;

    public static bool hasReceivedDoubleDice = false;
    public static bool hasReceivedAvoidSnake = false;
    private GameObject dice2Instance;


    private bool[] isWaitingForLadderGrab = new bool[4];

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
        player1InventoryScript.GetInventory().AddItem(new Item { itemType = Item.ItemType.LadderGrab });
        player2InventoryScript.GetInventory().AddItem(new Item { itemType = Item.ItemType.LadderGrab });

    }

    void Update()
    {
        // --- Player 1 ---
        if (whosTurn == 1 && !isWaitingForLadderGrab[0] && player1.GetComponent<FollowThePath>().waypointIndex > player1StartWaypoint + diceSideThrown)
        {
            player1.GetComponent<FollowThePath>().moveAllowed = false;
            player1MoveText.SetActive(false);
            player2MoveText.SetActive(true);

            var path1 = player1.GetComponent<FollowThePath>();
            player1StartWaypoint = path1.waypointIndex - 1;
            int currentWaypoint = path1.waypointIndex;

            if (wishingWellWaypoints.Contains(currentWaypoint))
                GiveRandomItemToPlayer(1);

            if (snakePositions.ContainsKey(currentWaypoint) && !useAvoidSnake)
            {
                int newWaypoint = snakePositions[currentWaypoint];
                StartCoroutine(AnimateSnakeBite(player1, newWaypoint));
            }
            else if (ladderPositions.ContainsKey(currentWaypoint))
            {
                int newWaypoint = ladderPositions[currentWaypoint];
                StartCoroutine(MovePlayerUpLadder(player1, newWaypoint));
            }
            else if (useLadderGrab)
            {
                return;
            }
            else
            {
                whosTurn++;
                UpdateCameraTarget();
                if (useAvoidSnake) useAvoidSnake = false;
            }

            if (currentWaypoint == 2 && !hasReceivedAvoidSnake)
            {
                player1InventoryScript.GetInventory().AddItem(new Item { itemType = Item.ItemType.AvoidSnake });
                hasReceivedAvoidSnake = true;
            }

            GameObject.Find("DiceCheckZone").GetComponent<DiceCheckZoneScript>().rolled = true;
            useLadderGrab = false;
        }

        // --- Player 2 ---
        if (whosTurn == 2 && !isWaitingForLadderGrab[1] && player2.GetComponent<FollowThePath>().waypointIndex > player2StartWaypoint + diceSideThrown)
        {
            player2.GetComponent<FollowThePath>().moveAllowed = false;
            player2MoveText.SetActive(false);
            player3MoveText.SetActive(true);

            var path2 = player2.GetComponent<FollowThePath>();
            player2StartWaypoint = path2.waypointIndex - 1;
            int currentWaypoint = path2.waypointIndex;

            if (wishingWellWaypoints.Contains(currentWaypoint))
                GiveRandomItemToPlayer(2);

            if (snakePositions.ContainsKey(currentWaypoint) && !useAvoidSnake)
            {
                int newWaypoint = snakePositions[currentWaypoint];
                StartCoroutine(AnimateSnakeBite(player2, newWaypoint));
            }
            else if (ladderPositions.ContainsKey(currentWaypoint))
            {
                int newWaypoint = ladderPositions[currentWaypoint];
                StartCoroutine(MovePlayerUpLadder(player2, newWaypoint));
            }
            else if (useLadderGrab)
            {
                return;
            }
            else
            {
                whosTurn++;
                UpdateCameraTarget();
                if (useAvoidSnake) useAvoidSnake = false;
            }

            GameObject.Find("DiceCheckZone").GetComponent<DiceCheckZoneScript>().rolled = true;
            useLadderGrab = false;
        }

        // --- Player 3 ---
        if (whosTurn == 3 && !isWaitingForLadderGrab[2] && player3.GetComponent<FollowThePath>().waypointIndex > player3StartWaypoint + diceSideThrown)
        {
            player3.GetComponent<FollowThePath>().moveAllowed = false;
            player3MoveText.SetActive(false);
            player4MoveText.SetActive(true);

            var path3 = player3.GetComponent<FollowThePath>();
            player3StartWaypoint = path3.waypointIndex - 1;
            int currentWaypoint = path3.waypointIndex;

            if (wishingWellWaypoints.Contains(currentWaypoint))
                GiveRandomItemToPlayer(3);

            if (snakePositions.ContainsKey(currentWaypoint) && !useAvoidSnake)
            {
                int newWaypoint = snakePositions[currentWaypoint];
                StartCoroutine(AnimateSnakeBite(player3, newWaypoint));
            }
            else if (ladderPositions.ContainsKey(currentWaypoint))
            {
                int newWaypoint = ladderPositions[currentWaypoint];
                StartCoroutine(MovePlayerUpLadder(player3, newWaypoint));
            }
            else if (useLadderGrab)
            {
                return;
            }
            else
            {
                whosTurn++;
                UpdateCameraTarget();
                if (useAvoidSnake) useAvoidSnake = false;
            }

            GameObject.Find("DiceCheckZone").GetComponent<DiceCheckZoneScript>().rolled = true;
            useLadderGrab = false;
        }

        // --- Player 4 ---
        if (whosTurn == 4 && !isWaitingForLadderGrab[3] && player4.GetComponent<FollowThePath>().waypointIndex > player4StartWaypoint + diceSideThrown)
        {
            player4.GetComponent<FollowThePath>().moveAllowed = false;
            player4MoveText.SetActive(false);
            player1MoveText.SetActive(true);

            var path4 = player4.GetComponent<FollowThePath>();
            player4StartWaypoint = path4.waypointIndex - 1;
            int currentWaypoint = path4.waypointIndex;

            if (wishingWellWaypoints.Contains(currentWaypoint))
                GiveRandomItemToPlayer(4);

            if (snakePositions.ContainsKey(currentWaypoint) && !useAvoidSnake)
            {
                int newWaypoint = snakePositions[currentWaypoint];
                StartCoroutine(AnimateSnakeBite(player4, newWaypoint));
            }
            else if (ladderPositions.ContainsKey(currentWaypoint))
            {
                int newWaypoint = ladderPositions[currentWaypoint];
                StartCoroutine(MovePlayerUpLadder(player4, newWaypoint));
            }
            else if (useLadderGrab)
            {
                return;
            }
            else
            {
                whosTurn = 1;
                UpdateCameraTarget();
                if (useAvoidSnake) useAvoidSnake = false;
            }

            GameObject.Find("DiceCheckZone").GetComponent<DiceCheckZoneScript>().rolled = true;
            useLadderGrab = false;
        }

        // --- Win Check ---
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
            player1StartWaypoint = newWaypoint;
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

        Animator snakeAnimator = GameObject.Find($"waypoints/waypoint ({player1StartWaypoint + 1})/snake")?.GetComponent<Animator>();
        Debug.Log($"({targetWaypointIndex})");
        if (snakeAnimator != null)
        {
            snakeAnimator.Play("Attack");
        }
        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;
            player.transform.position = originalPos + new Vector3(offsetX, offsetY, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        player.transform.position = originalPos;

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
        if (player == player1)
            player1StartWaypoint = targetWaypointIndex;
        else if (player == player2)
            player2StartWaypoint = targetWaypointIndex;
        else if (player == player3)
            player3StartWaypoint = targetWaypointIndex;
        else if (player == player4)
            player4StartWaypoint = targetWaypointIndex;

        if (whosTurn <= 3)
        {
            whosTurn++;
        }
        else
        {
            whosTurn = 1;
        }
        UpdateCameraTarget();

        if (snakeAnimator != null)
        {
            snakeAnimator.Play("Idle");
        }
    }

    private IEnumerator MovePlayerUpLadder(GameObject player, int targetWaypointIndex)
    {
        var path = player.GetComponent<FollowThePath>();
        path.moveAllowed = false;

        Vector3 startPos = player.transform.position;
        Vector3 targetPos = path.waypoints[targetWaypointIndex].position;
        float moveDuration = 0.5f;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            player.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        player.transform.position = targetPos;
        path.waypointIndex = targetWaypointIndex;

        if (player == player1) player1StartWaypoint = targetWaypointIndex;
        else if (player == player2) player2StartWaypoint = targetWaypointIndex;
        else if (player == player3) player3StartWaypoint = targetWaypointIndex;
        else if (player == player4) player4StartWaypoint = targetWaypointIndex;

        if (whosTurn <= 3) whosTurn++;
        else whosTurn = 1;

        UpdateCameraTarget();
    }

    public void UseLadderGrab(GameObject player)
    {
        if (!IsLadderNearby(player))
        {
            Debug.Log("🚫 No ladder nearby — LadderGrab not used.");
            return; // Don't consume the item
        }

        var path = player.GetComponent<FollowThePath>();
        int currentWaypoint = path.waypointIndex;

        int nearestLadderStart = -1;
        int nearestLadderEnd = -1;
        int minDistance = int.MaxValue;

        foreach (var ladder in ladderPositions)
        {
            int distance = ladder.Key - currentWaypoint;
            if (distance > 0 && distance <= 5 && distance < minDistance)
            {
                nearestLadderStart = ladder.Key;
                nearestLadderEnd = ladder.Value;
                minDistance = distance;
            }
        }

        if (nearestLadderStart != -1)
        {
            Debug.Log($"🪜 LadderGrab: climbing from {nearestLadderStart} to {nearestLadderEnd}");
            int playerIndex = GetPlayerIndex(player); // helper
            isWaitingForLadderGrab[playerIndex] = true;
            StartCoroutine(UseLadderGrabClimb(player, nearestLadderStart, nearestLadderEnd));

        }
    }

    public IEnumerator UseLadderGrabClimb(GameObject player, int ladderStartIndex, int ladderEndIndex)
    {
        var path = player.GetComponent<FollowThePath>();

        // Jump to ladder start instantly
        path.waypointIndex = ladderStartIndex;
        player.transform.position = path.waypoints[ladderStartIndex].position;

        yield return new WaitForSeconds(0.3f); // slight pause

        // Animate climb
        Vector3 startPos = player.transform.position;
        Vector3 targetPos = path.waypoints[ladderEndIndex].position;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            player.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        player.transform.position = targetPos;
        path.waypointIndex = ladderEndIndex;

        // Update start waypoint
        if (player == player1) player1StartWaypoint = ladderEndIndex;
        else if (player == player2) player2StartWaypoint = ladderEndIndex;
        else if (player == player3) player3StartWaypoint = ladderEndIndex;
        else if (player == player4) player4StartWaypoint = ladderEndIndex;

        // Advance turn immediately
        whosTurn = (whosTurn % 4) + 1;
        UpdateCameraTarget();

        // Update UI
        player1MoveText.SetActive(whosTurn == 1);
        player2MoveText.SetActive(whosTurn == 2);
        player3MoveText.SetActive(whosTurn == 3);
        player4MoveText.SetActive(whosTurn == 4);

        GameObject.Find("DiceCheckZone").GetComponent<DiceCheckZoneScript>().rolled = true;
        int playerIndex = GetPlayerIndex(player);
        isWaitingForLadderGrab[playerIndex] = false;
        diceSideThrown = 0;
    }

    private int GetPlayerIndex(GameObject player)
    {
        if (player == player1) return 0;
        if (player == player2) return 1;
        if (player == player3) return 2;
        if (player == player4) return 3;
        return -1; // fail-safe
    }



    private static Dictionary<int, int> snakePositions = new Dictionary<int, int>()
    {
        {3, 0},
        {13, 9},
        {21, 16},
        {33, 25},
        {51, 46}
    };

    private static Dictionary<int, int> ladderPositions = new Dictionary<int, int>()
    {
        {4, 7},
        {14, 20},
        {31, 38},
        {44, 47}
    };

    private static HashSet<int> wishingWellWaypoints = new HashSet<int>
    {
        5, 14, 20, 27, 33, 46
    };

    private void GiveRandomItemToPlayer(int playerNumber)
    {
        Item.ItemType[] possibleItems = new Item.ItemType[]
        {
        Item.ItemType.AvoidSnake,
        Item.ItemType.DoubleDice,
        Item.ItemType.LadderGrab
        };

        int randomIndex = Random.Range(0, possibleItems.Length);
        Item randomItem = new Item { itemType = possibleItems[randomIndex] };

        switch (playerNumber)
        {
            case 1:
                player1InventoryScript.GetInventory().AddItem(randomItem);
                Debug.Log("🎁 Player 1 received: " + randomItem.itemType);
                break;
            case 2:
                player2InventoryScript.GetInventory().AddItem(randomItem);
                Debug.Log("🎁 Player 2 received: " + randomItem.itemType);
                break;
            case 3:
                player3InventoryScript.GetInventory().AddItem(randomItem);
                Debug.Log("🎁 Player 3 received: " + randomItem.itemType);
                break;
            case 4:
                player4InventoryScript.GetInventory().AddItem(randomItem);
                Debug.Log("🎁 Player 4 received: " + randomItem.itemType);
                break;
        }
    }

    public bool IsLadderNearby(GameObject player)
    {
        var path = player.GetComponent<FollowThePath>();
        int currentWaypoint = path.waypointIndex;

        foreach (var ladder in ladderPositions)
        {
            int distance = ladder.Key - currentWaypoint;
            if (distance > 0 && distance <= 5)
                return true;
        }
        return false;
    }
}
