using UnityEngine;

public class DiceCheckZoneScript : MonoBehaviour
{
    Vector3 diceVelocity;
    private int whosTurn = 1;
    public bool canMove = false;
    public static bool dice1Done = false;
    public static bool dice2Done = false;
    public int diceNumber;
    public bool rolled = false;
    public static int dice1 = 0;
    public static int dice2 = 0;

    void FixedUpdate()
    {
        diceVelocity = DiceScript.diceVelocity;
    }

    void OnTriggerStay(Collider col)
    {
        Transform diceObject = col.transform.parent;

        if (diceVelocity.x == 0f && diceVelocity.y == 0f && diceVelocity.z == 0f && rolled)
        {
            int temp = 0;
            switch (col.gameObject.name)
            {
                case "Side1":
                    diceNumber = 6;
                    temp = 6;
                    break;
                case "Side2":
                    diceNumber = 5;
                    temp = 5;
                    break;
                case "Side3":
                    diceNumber = 4;
                    temp = 4;
                    break;
                case "Side4":
                    diceNumber = 3;
                    temp = 3;
                    break;
                case "Side5":
                    diceNumber = 2;
                    temp = 2;
                    break;
                case "Side6":
                    diceNumber = 1;
                    temp = 1;
                    break;
            }

            if (col.transform.parent.name == "dice")
            {
                dice1 = temp;
                dice1Done = true;
            }
            else if (col.transform.parent.name == "dice2(Clone)")
            {
                dice2 = temp;
                dice2Done = true;
            }

            DiceNumberTextScript.diceNumber = dice1 + dice2;
            GameControl.diceSideThrown = DiceNumberTextScript.diceNumber;

            Debug.Log("Dice : " + GameControl.diceSideThrown);

            if (dice1Done && dice2Done)
            {
                if (GameObject.Find("GameControl").GetComponent<GameControl>().whosTurn == 1)
                {
                    if (canMove == true)
                    {
                        GameControl.MovePlayer(1);
                        rolled = false;
                        GameControl.hasReceivedDoubleDice = false;
                        canMove = false;
                    }
                }
                else if (GameObject.Find("GameControl").GetComponent<GameControl>().whosTurn == -1)
                {
                    if (canMove == true)
                    {
                        GameControl.MovePlayer(2);
                        canMove = false;
                    }
                }
            }
        }
    }
}
