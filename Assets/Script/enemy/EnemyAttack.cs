using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

    private Enemy ParentEnemyController;

    private player PlayerController;
    private bool EnableAttack = false;

    private Collider2D EnemyCollider2D;

    public int MaxDamageTickDelay = 30;
    private int DamageTickDelay = 0;

    void Start()
    {
        ParentEnemyController = GetComponentInParent<Enemy>();
        GameObject PlayerObject = GameObject.FindGameObjectWithTag("Player");
        PlayerController = PlayerObject.GetComponent<player>();
        EnemyCollider2D = GetComponent<Collider2D>();
        EnemyCollider2D.enabled = false;
    }

    void Update()
    {
        if (DamageTickDelay > 0)
            --DamageTickDelay;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!EnableAttack)
            return;
        PlayerController.OnReceviceDamage(transform.parent.gameObject, ParentEnemyController.ATK, ParentEnemyController.knockback);
        EnableAttack = false;
    }

    public void BeforeAttack()
    {
        EnableAttack = true;
        EnemyCollider2D.enabled = true;
        DamageTickDelay = 0;
    }

    public void AfterAttack()
    {
        EnableAttack = false;
        EnemyCollider2D.enabled = false;
        DamageTickDelay = 0;
    }
}
