using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Animator animator;

    [Space(20)]
    [SerializeField] float speed = 7f;
    [SerializeField] float deathPositionY = -15f;

    [Space(20)]
    [SerializeField] float jumpForce = 12f;
    [SerializeField] Transform groundCheck;
    [SerializeField] float radius = 0.25f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float coyoteTime = 0.25f;
    float coyoteTimer;
    Vector2 gravity;
    [SerializeField] float fallMultiplier = 3f;

    [Space(20)]
    [SerializeField] Transform enemyCheck;
    [SerializeField] float attackRadius = 1f;
    [SerializeField] LayerMask enemyLayer;

    [Space(20)]
    [SerializeField] float health = 5;
    [SerializeField] Slider healthSlider;

    [Space(20)]
    [SerializeField] float invisibilityTime = 5;
    float invisibilityTimer;
    [SerializeField] Slider invisibilitySlider;

    [Space(20)]
    [SerializeField] GameObject playerPanel;
    [SerializeField] GameObject gameoverPanel;
    // [SerializeField] AudioSource jumpSound;
    // [SerializeField] AudioSource attackSound;
    // [SerializeField] AudioSource hittedSound;
    // [SerializeField] AudioSource gameoverMusic;
    // [SerializeField] AudioSource backgroundMusic;

    bool isRunning;
    bool isJumping;
    bool isFalling;
    bool isAttacking;
    public static bool isInvisible;
    public static bool isDying;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        healthSlider.maxValue = health;

        invisibilityTimer = invisibilityTime;
        invisibilitySlider.maxValue = invisibilityTime;

        gravity = new Vector2(0, -Physics2D.gravity.y);
        coyoteTimer = 0f;
        isRunning = false;
        isJumping = false;
        isFalling = false;
        isAttacking = false;
        isInvisible = false;
        isDying = false;
    }

    void FixedUpdate()
    {
        Movement();
    }

    void Update()
    {
        HandleAnimations();

        healthSlider.value = health;

        if (health <= 0 && !isDying) Die();

        if (isDying || isAttacking) return;

        if (transform.position.y <= deathPositionY)
        {
            Die();
        }

        HandleInvisibility();

        Jump();
        Fall();
        Attack();
        Flip();
    }

    void Movement()
    {
        if (isDying || isAttacking)
        {
            isRunning = false;
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * speed, rb.velocity.y);
    }

    void Jump()
    {
        if (!IsGrounded())
        {
            coyoteTimer -= Time.deltaTime;
        }
        else
        {
            coyoteTimer = coyoteTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && coyoteTimer > 0 && !isAttacking)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
            // jumpSound.Play();
        }

        if (Input.GetKeyUp(KeyCode.Space)) coyoteTimer = 0f;
    }

    void Fall()
    {
        if (!IsGrounded() && rb.velocity.y < 0)
        {
            isJumping = false;
            isFalling = true;

            rb.velocity -= gravity * fallMultiplier * Time.deltaTime;
        }
        else
        {
            isFalling = false;
        }
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, radius, groundLayer);
    }

    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isAttacking && !isFalling && !isJumping && !isInvisible)
        {
            animator.SetTrigger("Attack");
            isAttacking = true;
            isRunning = false;
            rb.velocity = new Vector2(0, rb.velocity.y);
            StartCoroutine(AttackDelay());
        }
    }

    void DamageEnemy()
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(enemyCheck.position, attackRadius, enemyLayer);

        foreach (Collider2D collision in collisions)
        {
            collision.GetComponent<Enemy>().Die();
        }

        // attackSound.Play();
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(0.75f);
        isAttacking = false;
    }

    void Flip()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 && rb.velocity.sqrMagnitude > 0.01f)
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                transform.localEulerAngles = Vector2.zero;
            }
            else
            {
                transform.localEulerAngles = new Vector2(0, 180);
            }

            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
    }

    void HandleAnimations()
    {
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsFalling", isFalling);
    }

    public void GetHitted()
    {
        health--;
        spriteRenderer.color = Color.red;
        StartCoroutine(GetHittedDelay());
    }

    IEnumerator GetHittedDelay()
    {
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = Color.white;
    }

    void Die()
    {
        if (isDying) return;

        // hittedSound.Play();
        animator.SetTrigger("Death");
        isDying = true;
    }

    void Gameover()
    {
        // backgroundMusic.Stop();
        Time.timeScale = 0;
        // gameoverMusic.Play();
        playerPanel.SetActive(false);
        gameoverPanel.SetActive(true);
    }

    void HandleInvisibility()
    {
        if (!isInvisible) return;

        invisibilitySlider.value = invisibilityTimer;

        invisibilityTimer -= Time.deltaTime;

        if (invisibilityTimer <= 0)
        {
            invisibilityTimer = invisibilityTime;

            spriteRenderer.color = Color.white;
            invisibilitySlider.transform.parent.gameObject.SetActive(false);
            isInvisible = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HealthPotion"))
        {
            if (health >= 5) return;

            Destroy(collision.gameObject);
            health++;
        }

        if (collision.CompareTag("Durag"))
        {
            Destroy(collision.gameObject);
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            invisibilitySlider.transform.parent.gameObject.SetActive(true);
            isInvisible = true;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, radius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(enemyCheck.position, attackRadius);
    }
}
