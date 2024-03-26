using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBoss : Boss
{
    [Space(20)]
    [SerializeField] Transform playerCheck;
    [SerializeField] float attackRadius;
    [SerializeField] LayerMask playerLayer;

    protected override void Update()
    {
        if (bossLife.isDying) return;

        base.Update();
        if (Vector2.Distance(transform.position, player.transform.position) <= stoppingDistance && !isAttacking)
        {
            Attack();
        }
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
        isAttacking = true;
        StartCoroutine(AttackDelay());
    }

    void DamagePlayer()
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(playerCheck.position, attackRadius, playerLayer);

        foreach (Collider2D collision in collisions)
        {
            collision.GetComponent<PlayerController>().GetHitted();
        }
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerCheck.position, attackRadius);
    }
}
