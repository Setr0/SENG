using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    EnemyLife enemyLife;

    Transform player;
    [SerializeField] float speed = 3f;
    [SerializeField] GameObject path;

    [Space(20f)]
    [SerializeField] Transform playerCheck;
    [SerializeField] float radius = 0.5f;
    [SerializeField] LayerMask playerLayer;

    List<Vector2> waypoints;
    int waypointIndex;

    [Space(20f)]
    [SerializeField] AudioSource attackSound;
    [SerializeField] AudioSource hittedSound;

    bool isRunning;
    bool isAttacking;
    bool isPathCompleted;
    bool isWaiting;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyLife = GetComponent<EnemyLife>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>());

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), enemy.GetComponent<Collider2D>());
        }

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
    }

    void Update()
    {
        if (enemyLife.isDying || PlayerController.isDying || enemyLife.isHitted || DialogueManager.isDialogueActive)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            animator.SetBool("IsRunning", false);
            return;
        }

        if ((Vector2.Distance(transform.position, player.position) > 2f && !isAttacking) || PlayerController.isInvisible)
        {
            Patrol();
            Flip();
        }
        else
        {
            Attack();
            FlipTowardsPlayer();
        }

        if (rb.velocity.sqrMagnitude > 0.01f) isRunning = true;
        else isRunning = false;

        HandleAnimations();
    }

    void Patrol()
    {
        if (isWaiting)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 direction = (waypoints[waypointIndex] - (Vector2)transform.position).normalized;

        rb.velocity = direction * speed;

        if (Vector2.Distance(transform.position, waypoints[waypointIndex]) < 0.25f)
        {
            if (!isPathCompleted)
            {
                waypointIndex++;

                if (waypointIndex == waypoints.Count)
                {
                    waypointIndex--;
                    isPathCompleted = true;

                    StartCoroutine(Wait());
                }
            }
            else
            {
                waypointIndex--;

                if (waypointIndex == -1)
                {
                    waypointIndex++;
                    isPathCompleted = false;

                    StartCoroutine(Wait());
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
            transform.localEulerAngles = Vector2.zero;
        }

        if (rb.velocity.x < 0)
        {
            transform.localEulerAngles = new Vector2(0, 180);
        }
    }

    void FlipTowardsPlayer()
    {
        if (player.position.x > transform.position.x)
        {
            transform.localEulerAngles = Vector2.zero;
        }
        if (player.position.x < transform.position.x)
        {
            transform.localEulerAngles = new Vector2(0, 180);
        }
    }

    void Attack()
    {
        if (isAttacking) return;

        animator.SetTrigger("Attack");

        animator.SetBool("IsRunning", false);
        rb.velocity = new Vector2(0, rb.velocity.y);

        isAttacking = true;

        StartCoroutine(AttackDelay());
    }

    void DamagePlayer()
    {
        Collider2D collision = Physics2D.OverlapCircle(playerCheck.position, radius, playerLayer);

        if (collision == null) return;

        if (collision.GetComponent<PlayerController>())
            collision.GetComponent<PlayerController>().GetHitted();

        if (attackSound != null) attackSound.Play();
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(0.75f);
        isAttacking = false;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    void HandleAnimations()
    {
        animator.SetBool("IsRunning", isRunning);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerCheck.position, radius);
    }
}
