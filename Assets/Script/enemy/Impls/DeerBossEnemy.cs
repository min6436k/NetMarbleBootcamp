using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeerBossEnemy : BossEnemy
{

    void Start()
    {
        Initialize();   
    }

    public override void OnColliderTriggerStay(Collider2D collision)
    {
    }

    public override void OnColliderTriggerExit(Collider2D collision)
    {
    }

}
