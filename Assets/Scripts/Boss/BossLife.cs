using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
        base.GetHitted();
    }

    public override void Die()
    {
        // GetComponent<Boss>().backgroundMusic.UnPause();
        // GetComponent<Boss>().bossBattleMusic.Stop();

        base.Die();
    }
}
