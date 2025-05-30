using UnityEngine;

public class DiceCheckZoneScript : MonoBehaviour
{
    Vector3 diceVelocity;
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

        if (diceVelocity == Vector3.zero && rolled)
        {
            int temp = 0;
            switch (col.gameObject.name)
            {
                case "Side1": temp = 6; break;
                case "Side2": temp = 5; break;
                case "Side3": temp = 4; break;
                case "Side4": temp = 3; break;
                case "Side5": temp = 2; break;
                case "Side6": temp = 1; break;
            }

            diceNumber = temp;

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

            DiceNumberTextScript.diceNumber = dice1+dice2;
            GameControl.diceSideThrown = DiceNumberTextScript.diceNumber;

            Debug.Log("Dice : " + GameControl.diceSideThrown);

            if (dice1Done && dice2Done && canMove)
            {
                int currentTurn = GameObject.Find("GameControl").GetComponent<GameControl>().whosTurn;

                if (currentTurn == 1)
                {
                    GameControl.MovePlayer(1);
                }
                else if (currentTurn == 2)
                {
                    GameControl.MovePlayer(2);
                }
                else if (currentTurn == 3)
                {
                    GameControl.MovePlayer(3);
                }
                else if (currentTurn == 4)
                {
                    GameControl.MovePlayer(4);
                }

                rolled = false;
                GameControl.hasReceivedDoubleDice = false;
                canMove = false;
            }
        }
    }
}
