using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossEnemy : Enemy
{

    [Header("Raycast")]
    public float RaycastPlayerPositionOffsetX = 0.0F;
    public float RaycastPlayerPositionOffsetY = 0.0F;
    public float RaycastEnemyPositionOffsetX = 0.0F;
    public float RaycastEnemyPositionOffsetY = 0.0F;

    protected override void InitializeAttack()
    {
    }

    public override void OnEnemyUpdate()
    {
        log("Raycast: " + IsCanSeePlayer());
    }

    public override void OnEnemyFixedUpdate()
    {
    }

    protected override void InitializeHealthBar()
    {
    }

    public override void OnDieEvent()
    {
    }

    public override void OnUpdateNearbyPlayerAnimation()
    {
    }

    public override void OnPostDead()
    {
    }

    protected bool IsCanSeePlayer()
    {
        Vector3 PlayerPosition = PlayerObject.transform.position + new Vector3(RaycastPlayerPositionOffsetX, RaycastPlayerPositionOffsetY);
        Vector3 EnemyPosition = transform.position + new Vector3(RaycastEnemyPositionOffsetX, RaycastEnemyPositionOffsetY);
        Vector3 Direction = PlayerPosition - EnemyPosition;

        if (isDebug)
            Debug.DrawRay(EnemyPosition, Direction, Color.cyan, 3);

        float Distance = Vector2.Distance(PlayerPosition, EnemyPosition);
        RaycastHit2D result = Physics2D.Raycast(EnemyPosition, Direction, Distance, LayerMask.GetMask("Ground"));

        return result.collider == null;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;
        OnColliderTriggerStay(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;
        OnColliderTriggerExit(collision);
    }

    public abstract void OnColliderTriggerStay(Collider2D collision);

    public abstract void OnColliderTriggerExit(Collider2D collision);
}