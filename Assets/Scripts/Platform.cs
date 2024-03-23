using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] GameObject path;
    List<Vector2> waypoints;
    int waypointIndex;
    bool isPathCompleted;

    void Start()
    {
        waypoints = new List<Vector2>();
        for (int i = 0; i < path.transform.childCount; i++)
        {
            waypoints.Add(path.transform.GetChild(i).position);
        }
        waypointIndex = 0;

        path.transform.parent = GameObject.Find("Paths").transform;
        isPathCompleted = false;
    }

    void FixedUpdate()
    {
        Patrol();
    }

    void Patrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, waypoints[waypointIndex], speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, waypoints[waypointIndex]) < 0.25f)
        {
            if (!isPathCompleted)
            {
                waypointIndex++;

                if (waypointIndex == waypoints.Count)
                {
                    waypointIndex--;
                    isPathCompleted = true;
                }
            }
            else
            {
                waypointIndex--;

                if (waypointIndex == -1)
                {
                    waypointIndex++;
                    isPathCompleted = false;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.transform.parent = transform;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        collision.transform.parent = null;
    }
}
