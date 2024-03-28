using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float speed = 12f;
    public Vector2 direction = new Vector2(1, 0);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rb.velocity = direction * speed;

        transform.Rotate(0, 0, -10);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<EnemyLife>())
            if (collision.GetComponent<EnemyLife>().isDying) return;

        Destroy(gameObject);
    }
}
