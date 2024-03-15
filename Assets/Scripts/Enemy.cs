using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    Transform player;
    [SerializeField] float speed = 3f;
    [SerializeField] GameObject path;

    [Space(20f)]
    [SerializeField] Transform playerCheck;
    [SerializeField] float radius = 0.5f;
    [SerializeField] LayerMask playerLayer;

    List<Vector2> waypoints;
    int waypointIndex;
    bool isRunning;
    bool isAttacking;
    bool isPathCompleted;
    bool isWaiting;
    bool isDying;

    // [Space(20f)]
    // [SerializeField] AudioSource hittedSound;
    // [SerializeField] AudioSource attackSound;

    // [Space(20f)] Adds a space of 20 units before the variable field in the inspector so it's more readable
    // [SerializeField] gives to us the possibility to edit the variable in the inspector

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        // Ignore collisions with the Player
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>());

        // Ignore collisions with the other enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), enemy.GetComponent<Collider2D>());
        }

        // Get all waypoints' positions and store them in waypoints
        waypoints = new List<Vector2>();
        for (int i = 0; i < path.transform.childCount; i++)
        {
            waypoints.Add(path.transform.GetChild(i).position);
        }

        Transform paths = GameObject.Find("Paths").transform;
        path.transform.parent = paths;

        waypointIndex = 0;
        isRunning = false;
        isAttacking = false;
        isPathCompleted = false;
        isWaiting = false;
        isDying = false;
    }

    void Update()
    {
        // If the player is dead or the enemy is dead the enemy stops
        if (isDying || PlayerController.isDying)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            animator.SetBool("IsRunning", false);
            return;
        }

        // If the player is more than 1.5 units distant the enemy follows its path. If the player is closer the enemy attacks him
        if (Vector2.Distance(transform.position, player.position) > 1.5f && !isAttacking)
        {
            Patrol();
            Flip();
        }
        else
        {
            Attack();
            FlipTowardsPlayer();
        }

        // The magnitude is the multiplications of the parameters in a vector. sqrMagnitude() return the square root of the magnitude which gives better performance
        // If the sqrMagnitude of the velocity is higher than 0.01 means the enemy is moving
        if (rb.velocity.sqrMagnitude > 0.01f) isRunning = true;
        else isRunning = false;

        HandleAnimations();
    }

    void Patrol()
    {
        // If the enemy is waiting it stops
        if (isWaiting)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // Calculate the distance between the waypoint and the current posittion of the enemy
        // The vector gets normalized so we have only 1 or 0t.
        // new Vector2(20, 0).normalized --> Vector2(1, 0)
        // new Vector2(-20, 0).normalized --> Vector2(-1, 0)
        Vector2 direction = (waypoints[waypointIndex] - (Vector2)transform.position).normalized;

        // The enemy moves towards the waypoint
        rb.velocity = direction * speed;

        // Check if the enemy is close to the waypoint
        if (Vector2.Distance(transform.position, waypoints[waypointIndex]) < 0.25f)
        {
            if (!isPathCompleted)
            {
                // Increment the waypoint's index so the enemy moves towards the next waypoint
                waypointIndex++;

                // If the enemy has reached all the waypoints it stops for 1 second
                if (waypointIndex == waypoints.Count)
                {
                    waypointIndex--;
                    isPathCompleted = true;

                    StartCoroutine(Wait()); // Stop for 1 second
                }
            }
            else
            {
                // Decrease the waypoint's index so the enemy moves towards the next waypoint (reverse)
                waypointIndex--;

                // If the enemy has reached all the waypoints it stops for 1 second (reverse)
                if (waypointIndex == -1)
                {
                    waypointIndex++;
                    isPathCompleted = false;

                    StartCoroutine(Wait()); // Stop for 1 second 
                }
            }
        }
    }

    IEnumerator Wait()
    {
        isWaiting = true;
        yield return new WaitForSeconds(1f);
        isWaiting = false;
    }

    void Flip()
    {
        if (rb.velocity.x > 0)
        {
            // Turn right
            transform.localEulerAngles = Vector2.zero;
        }

        if (rb.velocity.x < 0)
        {
            // Turn left
            transform.localEulerAngles = new Vector2(0, 180);
        }
    }

    void FlipTowardsPlayer()
    {
        if (player.position.x > transform.position.x)
        {
            // Turn right
            transform.localEulerAngles = Vector2.zero;
        }
        if (player.position.x < transform.position.x)
        {
            // Turn left
            transform.localEulerAngles = new Vector2(0, 180);
        }
    }

    void Attack()
    {
        if (isAttacking) return;

        animator.SetTrigger("Attack");

        // Stop the enemy
        animator.SetBool("IsRunning", false);
        rb.velocity = new Vector2(0, rb.velocity.y);

        isAttacking = true;

        StartCoroutine(AttackDelay()); // The enemy can attack after 0.75 seconds
    }

    void DamagePlayer()
    {
        // Check if the player is in the playerCheck circle

        Collider2D collision = Physics2D.OverlapCircle(playerCheck.position, radius, playerLayer);

        if (collision == null) return;

        if (collision.GetComponent<PlayerController>())
            collision.GetComponent<PlayerController>().GetHitted();

        // if (attackSound != null) attackSound.Play();
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(2f);
        isAttacking = false;
    }

    public void Die()
    {
        if (isDying) return;

        // hittedSound.Play();
        animator.SetTrigger("Death");
        isDying = true;
        rb.gravityScale = 3;
        Invoke(nameof(DestroyObject), 1); // Destroy the gameObject after 1 second
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }

    void HandleAnimations()
    {
        animator.SetBool("IsRunning", isRunning);
    }

    // Draw the playerCheck circle in the editor so we can edit it
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerCheck.position, radius);
    }
}
