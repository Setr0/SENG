using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    Animator animator;
    public float health = 3f;
    [HideInInspector] public bool isHitted;
    [HideInInspector] public bool isDying;
    [SerializeField] AudioSource hittedSound;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();

        isHitted = false;
        isDying = false;
    }

    protected virtual void Update()
    {
        if (health <= 0 && !isDying)
        {
            Die();
        }
    }

    public virtual void GetHitted()
    {
        if (isDying) return;

        health--;
        if (hittedSound != null) hittedSound.Play();

        if (health > 0)
        {
            animator.SetTrigger("Hit");
            isHitted = true;
            StartCoroutine(GetHittedDelay());
        }
        else
        {
            Die();
        }
    }

    IEnumerator GetHittedDelay()
    {
        yield return new WaitForSeconds(0.5f);
        isHitted = false;
    }

    public virtual void Die()
    {
        if (isDying) return;

        animator.SetTrigger("Death");
        isDying = true;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnBecameInvisible()
    {
        if (isDying) Destroy(gameObject);
    }
}
