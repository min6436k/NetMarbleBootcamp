using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;


public class DefaultEnemy : Enemy
{

    void Start()
    {
        Initialize();
    }

    public override void OnEnemyUpdate()
    {
    }

    public override void OnEnemyFixedUpdate()
    {
    }

    public override void OnUpdateNearbyPlayerAnimation()
    {
        if (math.sign(transform.localScale.x) == math.sign(TargetXOffset))
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        if (TargetXDistance < 1.5f)
        {
            SetAnimationState("attack", true);
        }
        else
        {
            SetAnimationState("attack", false);
            if (EnemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("run"))
            {
                TargetPosition.x = math.sign(TargetXOffset);
                EnemyRigidbody2D.MovePosition(EnemyRigidbody2D.position + (TargetPosition * CurrentSpeed * Time.fixedDeltaTime));
            }
        }
    }

    public override void OnDieEvent()
    {
    }

}