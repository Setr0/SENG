using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Boss : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator animator;
    protected BossLife bossLife;
    BossesManager bossesManager;
    protected PlayerController player;

    public string bossName = "Boss name";

    [Space(20)]
    [SerializeField] protected float speed = 5f;
    [SerializeField] protected float stoppingDistance = 2f;
    [SerializeField] protected bool canRunAway;
    [SerializeField] protected float runAwayDistance = 1f;

    [Space(20)]
    [SerializeField] float jumpForce = 12;
    [SerializeField] Transform groundCheck;
    [SerializeField] float radius = 0.25f;
    [SerializeField] LayerMask groundLayer;

    [Space(20)]
    public float damage = 3;

    [Space(20)]
    public AudioSource bossBattleMusic;
    public AudioSource backgroundMusic;

    protected bool isAttacking;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bossLife = GetComponent<BossLife>();
        bossesManager = GetComponentInParent<BossesManager>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>());

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), enemy.GetComponent<Collider2D>());
        }

        isAttacking = false;
    }

    void FixedUpdate()
    {
        if (!bossesManager.isActive) return;

        if (canRunAway && Vector2.Distance(transform.position, player.transform.position) <= runAwayDistance)
            FollowTarget(-1);
        else
            FollowTarget(1);
    }

    protected virtual void Update()
    {
        if (bossLife.isDying || PlayerController.isDying)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            animator.SetBool("IsRunning", false);
            return;
        }

        FlipTowardsPlayer();
        HandleAnimations();
    }

    protected virtual void FollowTarget(int speedMultiplier, Vector2 target = default)
    {
        if (player == null || isAttacking || bossLife.isDying)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        if (target == default) target = player.transform.position;

        Vector2 direction = new Vector2(target.x - transform.position.x, 0).normalized;

        rb.velocity = new Vector2(direction.x * speed * speedMultiplier, rb.velocity.y);

        if (Vector2.Distance(transform.position, target) <= stoppingDistance
        && Vector2.Distance(transform.position, target) > runAwayDistance)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    void FlipTowardsPlayer()
    {
        if (player == null)
            return;

        Vector2 direction = player.transform.position - transform.position;

        if (direction.x > 0)
        {
            transform.localEulerAngles = Vector2.zero;
        }
        else if (direction.x < 0)
        {
            transform.localEulerAngles = new Vector2(0, 180);
        }
    }

    public void Jump()
    {
        if (IsGrounded())
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, radius, groundLayer);
    }

    protected virtual void HandleAnimations()
    {
        animator.SetBool("IsRunning", rb.velocity.sqrMagnitude > 0.01f);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, radius);
    }
}
