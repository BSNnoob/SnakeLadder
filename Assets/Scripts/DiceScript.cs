using UnityEngine;

public class DiceScript : MonoBehaviour
{
    private Rigidbody rb;
    public static Vector3 diceVelocity;
    internal int diceNumber;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("DiceScript START on: " + gameObject.name);
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb == null)
        {
            Debug.LogWarning("Rigidbody missing on dice — Update aborted!");
            return;
        }
            
        diceVelocity = rb.linearVelocity;
        Debug.Log("Dice Update running");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space pressed, rolling dice");
            DiceCheckZoneScript.dice1 = 0;
            DiceCheckZoneScript.dice2 = 0;
            DiceCheckZoneScript.dice1Done = false;
            if (GameControl.useDoubleDice)
            {
                DiceCheckZoneScript.dice2Done = false;
            }
            else
            {
                DiceCheckZoneScript.dice2Done = true;
            }
            DiceNumberTextScript.diceNumber = 0;
            float dirX = Random.Range(0, 500);
            float dirY = Random.Range(0, 500);
            float dirZ = Random.Range(0, 500);
            transform.position = new Vector3(0, 2, 0);
            transform.rotation = Quaternion.identity;
            rb.AddForce(transform.up * 500);
            rb.AddTorque(dirX, dirY, dirZ);
            GameObject.Find("DiceCheckZone").GetComponent<DiceCheckZoneScript>().canMove = true;
            GameObject.Find("DiceCheckZone").GetComponent<DiceCheckZoneScript>().rolled = true;
        }
    }
}
