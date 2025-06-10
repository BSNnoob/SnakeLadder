using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.SceneManagement;


public class GameControl : MonoBehaviour
{
    public GameObject backToMenuButton;

    public static bool hasGameStarted = false;
    private bool waitingForItemPanel = false;
    public GameObject infoPanel;
    public Text titleText;
    public Image itemImage;
    public Text descriptionText;

    private static GameObject whoWinsTextShadow, player1MoveText, player2MoveText, player3MoveText, player4MoveText;
    private static GameObject player1, player2, player3, player4;
    private static GameObject dice2;
    private bool turnEnded = false;


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
    public GameObject dice2Instance;

    private bool[] isWaitingForLadderGrab = new bool[4];

    private Dictionary<int, List<GameObject>> playersAtWaypoint = new Dictionary<int, List<GameObject>>();
    public float overlapOffset = 8f; // distance between stacked players


    public CinemachineCamera cmCamera;
    public static GameControl instance;

    void Awake()
    {
        instance = this;
    }

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

        ApplyOverlapOffset(player1, player1StartWaypoint);
        ApplyOverlapOffset(player2, player2StartWaypoint);
        ApplyOverlapOffset(player3, player3StartWaypoint);
        ApplyOverlapOffset(player4, player4StartWaypoint);

        if (hasGameStarted)
            UpdateCameraTarget();
    }

    void Update()
    {
        if (waitingForItemPanel || turnEnded) return;

        GameObject currentPlayer = GetCurrentPlayer();
        int playerIndex = whosTurn - 1;

        var path = currentPlayer.GetComponent<FollowThePath>();
        int currentWaypoint = path.waypointIndex;

        if (!isWaitingForLadderGrab[playerIndex] && currentWaypoint > GetStartWaypointForPlayer(whosTurn) + diceSideThrown)
        {
            path.moveAllowed = false;

            SetMoveTextActive(whosTurn, false);
            SetMoveTextActive((whosTurn % 4) + 1, true);

            SetStartWaypointForPlayer(whosTurn, currentWaypoint - 1);

            if (wishingWellWaypoints.Contains(currentWaypoint))
            {
                GiveRandomItemToPlayer(whosTurn);
                return;
            }

            if (snakePositions.ContainsKey(currentWaypoint) && !useAvoidSnake)
            {
                int newWaypoint = snakePositions[currentWaypoint];
                StartCoroutine(AnimateSnakeBite(currentPlayer, newWaypoint));
            }
            else if (ladderPositions.ContainsKey(currentWaypoint))
            {
                int newWaypoint = ladderPositions[currentWaypoint];
                StartCoroutine(MovePlayerUpLadder(currentPlayer, newWaypoint));
            }
            else if (useLadderGrab)
            {
                UseLadderGrab(currentPlayer);
                useLadderGrab = false;
                return;
            }
            else
            {
                whosTurn = (whosTurn % 4) + 1;
                if (hasGameStarted)
                    UpdateCameraTarget();

                DiceScript.canRoll = true;
                if (useAvoidSnake) useAvoidSnake = false;
            }

            if (currentWaypoint == 2 && !hasReceivedAvoidSnake && whosTurn == 1)
            {
                player1InventoryScript.GetInventory().AddItem(new Item { itemType = Item.ItemType.AvoidSnake });
                hasReceivedAvoidSnake = true;
            }

            GameObject.Find("DiceCheckZone").GetComponent<DiceCheckZoneScript>().rolled = true;
            useLadderGrab = false;
        }

        // Win check
        CheckWinCondition();
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

        // ✅ Reset flag to allow the new player's turn to start
        instance.turnEnded = false;

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

        Animator snakeAnimator = GameObject.Find($"waypoints/waypoint ({player1StartWaypoint})/snake")?.GetComponent<Animator>();

        if (whosTurn == 1)
        {
            snakeAnimator = GameObject.Find($"waypoints/waypoint ({player1StartWaypoint})/snake")?.GetComponent<Animator>();
        }
        else if (whosTurn == 2)
        {
            snakeAnimator = GameObject.Find($"waypoints/waypoint ({player2StartWaypoint})/snake")?.GetComponent<Animator>();
        }
        else if (whosTurn == 3)
        {
            snakeAnimator = GameObject.Find($"waypoints/waypoint ({player3StartWaypoint})/snake")?.GetComponent<Animator>();
        }
        else if (whosTurn == 4)
        {
            snakeAnimator = GameObject.Find($"waypoints/waypoint ({player4StartWaypoint})/snake")?.GetComponent<Animator>();
        }

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

        ApplyOverlapOffset(player, targetWaypointIndex);

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
        DiceScript.canRoll = true;
        UpdateCameraTarget();

        if (snakeAnimator != null)
        {
            snakeAnimator.Play("Idle");
        }
    }

    private IEnumerator MovePlayerUpLadder(GameObject player, int targetWaypointIndex)
    {
        var path = player.GetComponent<FollowThePath>();

        int startWaypointIndex = path.waypointIndex;

        // --- ENTRY PIPE ---
        Transform entryPipe = GameObject.Find($"waypoints/waypoint ({startWaypointIndex - 1})/PipeIn")?.transform;
        if (entryPipe == null)
        {
            Debug.LogWarning($"PipeIn not found for waypoint {startWaypointIndex}. Falling back to instant move.");
            path.waypointIndex = targetWaypointIndex;
            ApplyOverlapOffset(player, targetWaypointIndex);
            yield break;
        }

        // Move player to pipe entry
        player.transform.position = entryPipe.position;

        // Animate shrinking into pipe
        float duration = 0.3f;
        Vector3 originalScale = player.transform.localScale;
        float t = 0f;
        while (t < duration)
        {
            player.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        player.transform.localScale = Vector3.zero;
        player.GetComponent<Renderer>().enabled = false;

        // Wait inside pipe
        yield return new WaitForSeconds(0.3f);

        // --- EXIT PIPE ---
        Transform exitPipe = GameObject.Find($"waypoints/waypoint ({targetWaypointIndex})/PipeOut")?.transform;
        if (exitPipe == null)
        {
            Debug.LogWarning($"PipeOut not found for waypoint {targetWaypointIndex}. Using normal tile position.");
            player.transform.position = path.waypoints[targetWaypointIndex].position;
        }
        else
        {
            player.transform.position = exitPipe.position;
        }

        // Show and pop out of pipe
        yield return new WaitForSeconds(0.2f);
        player.GetComponent<Renderer>().enabled = true;

        t = 0f;
        while (t < duration)
        {
            player.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        player.transform.localScale = originalScale;

        // Finalize position and logic
        path.waypointIndex = targetWaypointIndex;
        ApplyOverlapOffset(player, targetWaypointIndex);

        if (player == player1) player1StartWaypoint = targetWaypointIndex;
        else if (player == player2) player2StartWaypoint = targetWaypointIndex;
        else if (player == player3) player3StartWaypoint = targetWaypointIndex;
        else if (player == player4) player4StartWaypoint = targetWaypointIndex;

        if (whosTurn <= 3) whosTurn++;
        else whosTurn = 1;
        DiceScript.canRoll = true;


        UpdateCameraTarget();
    }

    public bool UseLadderGrab(GameObject player)
    {
        if (!IsLadderNearby(player))
        {
            Debug.Log("🚫 LadderGrab not used: no ladder nearby.");
            return false;
        }

        var path = player.GetComponent<FollowThePath>();
        int currentWaypoint = path.waypointIndex;

        int nearestLadderStart = -1;
        int nearestLadderEnd = -1;
        int minDistance = int.MaxValue;

        foreach (var ladder in ladderPositions)
        {
            int distance = ladder.Key - currentWaypoint;
            if (distance >= 0 && distance <= 5 && distance < minDistance)
            {
                nearestLadderStart = ladder.Key;
                nearestLadderEnd = ladder.Value;
                minDistance = distance;
            }
        }

        if (nearestLadderStart != -1)
        {
            int playerIndex = GetPlayerIndex(player);
            isWaitingForLadderGrab[playerIndex] = true;
            StartCoroutine(UseLadderGrabClimb(player, nearestLadderStart, nearestLadderEnd));
            Debug.Log($"🪜 LadderGrab: climbing from {nearestLadderStart} to {nearestLadderEnd}");
            return true;
        }

        Debug.Log("🚫 No ladder found within range.");
        return false;
    }


    public IEnumerator UseLadderGrabClimb(GameObject player, int ladderStartIndex, int ladderEndIndex)
    {
        var path = player.GetComponent<FollowThePath>();

        Debug.Log($"⏬ LadderGrab STARTED: {player.name} from {ladderStartIndex} to {ladderEndIndex}");

        // Move to pipe entry (ladder base)
        path.waypointIndex = ladderStartIndex;
        player.transform.position = path.waypoints[ladderStartIndex].position;

        Transform entryPipe = GameObject.Find($"waypoints/waypoint ({ladderStartIndex})/PipeIn")?.transform;
        Transform exitPipe = GameObject.Find($"waypoints/waypoint ({ladderEndIndex})/PipeOut")?.transform;

        if (entryPipe != null)
            player.transform.position = entryPipe.position;

        // Smooth shrink animation
        float duration = 0.3f;
        float t = 0f;
        Vector3 originalScale = player.transform.localScale;

        while (t < duration)
        {
            player.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        player.transform.localScale = Vector3.zero;

        yield return new WaitForSeconds(0.3f);

        // Move to pipe exit (ladder top)
        if (exitPipe != null)
            player.transform.position = exitPipe.position;
        else
            player.transform.position = path.waypoints[ladderEndIndex].position;

        // Grow back to normal size
        t = 0f;
        while (t < duration)
        {
            player.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        player.transform.localScale = originalScale;

        // Finalize movement
        path.waypointIndex = ladderEndIndex;
        ApplyOverlapOffset(player, ladderEndIndex);

        if (player == player1) player1StartWaypoint = ladderEndIndex;
        else if (player == player2) player2StartWaypoint = ladderEndIndex;
        else if (player == player3) player3StartWaypoint = ladderEndIndex;
        else if (player == player4) player4StartWaypoint = ladderEndIndex;

        // Advance turn
        whosTurn = (whosTurn % 4) + 1;
        UpdateCameraTarget();
        DiceScript.canRoll = true;


        player1MoveText.SetActive(whosTurn == 1);
        player2MoveText.SetActive(whosTurn == 2);
        player3MoveText.SetActive(whosTurn == 3);
        player4MoveText.SetActive(whosTurn == 4);

        GameObject.Find("DiceCheckZone").GetComponent<DiceCheckZoneScript>().rolled = true;
        int playerIndex = GetPlayerIndex(player);
        isWaitingForLadderGrab[playerIndex] = false;
        diceSideThrown = 0;
        turnEnded = false;

        Debug.Log($"✅ LadderGrab COMPLETE: {player.name} is now at {ladderEndIndex}");
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
        {4, 7},// (+1)
        {14, 25},
        {31, 38},
        {44, 47}
    };

    private static HashSet<int> wishingWellWaypoints = new HashSet<int>
    {
        5, 11, 20, 27, 34, 46
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
                break;
            case 2:
                player2InventoryScript.GetInventory().AddItem(randomItem);
                break;
            case 3:
                player3InventoryScript.GetInventory().AddItem(randomItem);
                break;
            case 4:
                player4InventoryScript.GetInventory().AddItem(randomItem);
                break;
        }

        Debug.Log($"🎁 Player {playerNumber} received: {randomItem.itemType}");

        ShowItemInfo(randomItem, () =>
        {
            waitingForItemPanel = false;
            turnEnded = true;

            whosTurn = (whosTurn % 4) + 1;
            UpdateCameraTarget();
            DiceScript.canRoll = true;

            player1MoveText.SetActive(whosTurn == 1);
            player2MoveText.SetActive(whosTurn == 2);
            player3MoveText.SetActive(whosTurn == 3);
            player4MoveText.SetActive(whosTurn == 4);

            GameObject.Find("DiceCheckZone").GetComponent<DiceCheckZoneScript>().rolled = true;
            diceSideThrown = 0;
        });

        waitingForItemPanel = true;
    }



    public bool IsLadderNearby(GameObject player)
    {
        var path = player.GetComponent<FollowThePath>();
        int currentWaypoint = path.waypointIndex;

        foreach (var ladder in ladderPositions)
        {
            int distance = ladder.Key - currentWaypoint;
            if (distance > 0 && distance <= 10)
                return true;
        }
        return false;
    }

    private void ApplyOverlapOffset(GameObject player, int waypointIndex)
    {
        if (!playersAtWaypoint.ContainsKey(waypointIndex))
            playersAtWaypoint[waypointIndex] = new List<GameObject>();

        // Remove from other waypoints first
        foreach (var kvp in playersAtWaypoint)
            kvp.Value.Remove(player);

        // Add to current
        var list = playersAtWaypoint[waypointIndex];
        list.Add(player);

        // Get base position
        Transform[] waypoints = player.GetComponent<FollowThePath>().waypoints;
        Vector3 basePos = waypoints[waypointIndex].position;

        if (list.Count == 1)
        {
            player.transform.position = basePos;
            return;
        }

        // Circular offset
        int index = list.IndexOf(player);
        float angle = (360f / list.Count) * index;
        Vector3 offset = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            0,
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ) * overlapOffset;

        player.transform.position = basePos + offset;
    }

    private System.Action onInfoClosed;

    public void ShowItemInfo(Item item, System.Action onClosed)
    {
        titleText.text = item.GetTitle();
        descriptionText.text = item.GetDescription();
        itemImage.sprite = item.GetSprite();

        infoPanel.SetActive(true);
        waitingForItemPanel = true; // ⬅ Add this
        onInfoClosed = () =>
        {
            waitingForItemPanel = false; // ⬅ Clear it when closed
            onClosed?.Invoke();
        };
    }

    public void CloseInfoPanel()
    {
        infoPanel.SetActive(false);

        if (onInfoClosed != null)
        {
            onInfoClosed.Invoke();
            onInfoClosed = null;
        }
    }

    private GameObject GetCurrentPlayer()
    {
        switch (whosTurn)
        {
            case 1: return player1;
            case 2: return player2;
            case 3: return player3;
            case 4: return player4;
            default: return null;
        }
    }
    private void SetMoveTextActive(int playerNumber, bool active)
    {
        player1MoveText.SetActive(playerNumber == 1 && active);
        player2MoveText.SetActive(playerNumber == 2 && active);
        player3MoveText.SetActive(playerNumber == 3 && active);
        player4MoveText.SetActive(playerNumber == 4 && active);
    }

    private int GetStartWaypointForPlayer(int player)
    {
        switch (player)
        {
            case 1: return player1StartWaypoint;
            case 2: return player2StartWaypoint;
            case 3: return player3StartWaypoint;
            case 4: return player4StartWaypoint;
            default: return 0;
        }
    }

    private void SetStartWaypointForPlayer(int player, int value)
    {
        switch (player)
        {
            case 1: player1StartWaypoint = value; break;
            case 2: player2StartWaypoint = value; break;
            case 3: player3StartWaypoint = value; break;
            case 4: player4StartWaypoint = value; break;
        }
    }

    private void CheckWinCondition()
    {
        if (player1.GetComponent<FollowThePath>().waypointIndex == player1.GetComponent<FollowThePath>().waypoints.Length)
        {
            whoWinsTextShadow.SetActive(true);
            whoWinsTextShadow.GetComponent<Text>().text = "Player 1 Wins";
            gameOver = true;
            backToMenuButton.SetActive(true);
        }
        if (player2.GetComponent<FollowThePath>().waypointIndex == player2.GetComponent<FollowThePath>().waypoints.Length)
        {
            whoWinsTextShadow.SetActive(true);
            whoWinsTextShadow.GetComponent<Text>().text = "Player 2 Wins";
            gameOver = true;
            backToMenuButton.SetActive(true);
        }
        if (player3.GetComponent<FollowThePath>().waypointIndex == player3.GetComponent<FollowThePath>().waypoints.Length)
        {
            whoWinsTextShadow.SetActive(true);
            whoWinsTextShadow.GetComponent<Text>().text = "Player 3 Wins";
            gameOver = true;
            backToMenuButton.SetActive(true);
        }
        if (player4.GetComponent<FollowThePath>().waypointIndex == player4.GetComponent<FollowThePath>().waypoints.Length)
        {
            whoWinsTextShadow.SetActive(true);
            whoWinsTextShadow.GetComponent<Text>().text = "Player 4 Wins";
            gameOver = true;
            backToMenuButton.SetActive(true);
        }
    }
    public void UseLadderGrabFromInventory()
    {
        UseLadderGrab(GetCurrentPlayer());
    }
    public void SaveGame()
    {
        PlayerPrefs.SetInt("whosTurn", whosTurn);

        PlayerPrefs.SetInt("P1_Waypoint", player1.GetComponent<FollowThePath>().waypointIndex);
        PlayerPrefs.SetInt("P2_Waypoint", player2.GetComponent<FollowThePath>().waypointIndex);
        PlayerPrefs.SetInt("P3_Waypoint", player3.GetComponent<FollowThePath>().waypointIndex);
        PlayerPrefs.SetInt("P4_Waypoint", player4.GetComponent<FollowThePath>().waypointIndex);
        var items = player1InventoryScript.GetInventory().GetItemList();
        Debug.Log("💾 Items in Player 1 inventory before saving: " + items.Count);
        foreach (var item in items)
            Debug.Log("    🔸 " + item.itemType);


        string inv1 = player1InventoryScript.GetInventory().ToJson();
        Debug.Log("💾 Saving Player 1 Inventory JSON: " + inv1);
        PlayerPrefs.SetString("P1_Inventory", inv1);

        string inv2 = player2InventoryScript.GetInventory().ToJson();
        Debug.Log("💾 Saving Player 2 Inventory JSON: " + inv2);
        PlayerPrefs.SetString("P2_Inventory", inv2);

        string inv3 = player3InventoryScript.GetInventory().ToJson();
        Debug.Log("💾 Saving Player 3 Inventory JSON: " + inv3);
        PlayerPrefs.SetString("P3_Inventory", inv3);

        string inv4 = player4InventoryScript.GetInventory().ToJson();
        Debug.Log("💾 Saving Player 4 Inventory JSON: " + inv4);
        PlayerPrefs.SetString("P4_Inventory", inv4);

        PlayerPrefs.SetString("P1_Inventory", player1InventoryScript.GetInventory().ToJson());
        PlayerPrefs.SetString("P2_Inventory", player2InventoryScript.GetInventory().ToJson());
        PlayerPrefs.SetString("P3_Inventory", player3InventoryScript.GetInventory().ToJson());
        PlayerPrefs.SetString("P4_Inventory", player4InventoryScript.GetInventory().ToJson());

        PlayerPrefs.Save();
        Debug.Log("💾 Game Saved!");
    }

    public void LoadGame()
    {
        whosTurn = PlayerPrefs.GetInt("whosTurn", 1);
        player1MoveText.SetActive(whosTurn == 1);
        player2MoveText.SetActive(whosTurn == 2);
        player3MoveText.SetActive(whosTurn == 3);
        player4MoveText.SetActive(whosTurn == 4);

        player1.GetComponent<FollowThePath>().waypointIndex = PlayerPrefs.GetInt("P1_Waypoint", 0);
        player2.GetComponent<FollowThePath>().waypointIndex = PlayerPrefs.GetInt("P2_Waypoint", 0);
        player3.GetComponent<FollowThePath>().waypointIndex = PlayerPrefs.GetInt("P3_Waypoint", 0);
        player4.GetComponent<FollowThePath>().waypointIndex = PlayerPrefs.GetInt("P4_Waypoint", 0);

        player1.transform.position = player1.GetComponent<FollowThePath>().waypoints[PlayerPrefs.GetInt("P1_Waypoint", 0)].position;
        player2.transform.position = player2.GetComponent<FollowThePath>().waypoints[PlayerPrefs.GetInt("P2_Waypoint", 0)].position;
        player3.transform.position = player3.GetComponent<FollowThePath>().waypoints[PlayerPrefs.GetInt("P3_Waypoint", 0)].position;
        player4.transform.position = player4.GetComponent<FollowThePath>().waypoints[PlayerPrefs.GetInt("P4_Waypoint", 0)].position;

        player1InventoryScript.GetInventory().LoadFromJson(PlayerPrefs.GetString("P1_Inventory", ""));
        player1InventoryScript.GetInventory().OnItemListChanged?.Invoke();

        player2InventoryScript.GetInventory().LoadFromJson(PlayerPrefs.GetString("P2_Inventory", ""));
        player2InventoryScript.GetInventory().OnItemListChanged?.Invoke();

        player3InventoryScript.GetInventory().LoadFromJson(PlayerPrefs.GetString("P3_Inventory", ""));
        player3InventoryScript.GetInventory().OnItemListChanged?.Invoke();

        player4InventoryScript.GetInventory().LoadFromJson(PlayerPrefs.GetString("P4_Inventory", ""));
        player4InventoryScript.GetInventory().OnItemListChanged?.Invoke();

        UpdateCameraTarget();
        DiceScript.canRoll = true;

        Debug.Log("✅ Game Loaded!");
        Debug.Log("🧪 P1 Items after load: " + player1InventoryScript.GetInventory().GetItemList().Count);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f; // In case the game is paused
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // restart current scene
    }
}
