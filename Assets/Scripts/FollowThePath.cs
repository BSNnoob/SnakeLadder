using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class FollowThePath : MonoBehaviour
{
    public Transform[] waypoints;
    
    [SerializeField]
    private float moveSpeed = 2f;

    [HideInInspector]
    public int waypointIndex = 0;
    public bool moveAllowed = false;

    void Start()
    {
        transform.position = waypoints[waypointIndex].transform.position;
    }

    void Update()
    {
        if (moveAllowed)
            Move();
    }

    void Move()
    {
        if (waypointIndex <= waypoints.Length - 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, waypoints[waypointIndex].transform.position, moveSpeed * Time.deltaTime);
            if (transform.position == waypoints[waypointIndex].transform.position)
            {
                waypointIndex += 1;
            }
        }
    }
}
