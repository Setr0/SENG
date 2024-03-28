using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class BossLife : EnemyLife
{
    BossesManager bossesManager;

    protected override void Start()
    {
        base.Start();
        bossesManager = GetComponentInParent<BossesManager>();
    }

    protected override void Update()
    {
        if (!bossesManager.isActive) return;
        base.Update();
    }

    public override void GetHitted()
    {
        bossesManager.healthSlider.value--;
        base.GetHitted();
    }

    public override void Die()
    {
        base.Die();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), enemy.GetComponent<Collider2D>());
        }

        if (GetComponent<PlayableDirector>() != null) GetComponent<PlayableDirector>().Play();
    }
}
