using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[SerializeField]
public enum BearState
{
    IDLE,
    ATTACK,
}

public class BearEnemy : Enemy
{

    [Header("AI - 상태")]
    public BearState State = BearState.IDLE;
    public bool IsAttack = false;

    [Header("AI - 주변 탐색")]
    public bool IsIdleExplore = false;
    public float ExploreMinCycleSeconds = 1F; // 자동 탐색 주기
    public float ExploreMaxCycleSeconds = 5F; // 자동 탐색 주기
    private float ExploreStartTargetTime = 0;
    private float ExploreStartEscapeTime = 0;
    public float ExploreMoveSpeed = 1.5F;
    public float ExploreMinOffset = -10F, ExploreMaxOffset = 10F; // 중심 좌표로 부터 얼마만큼 움직일 것인가?
    public float ExploreMinOffsetValue = 3F; // 움직일 거리가 최소 이정도여야한다.
    private Vector2 ExplorePoint = Vector2.zero;
    private Vector2 ExploreTargetPoint = Vector2.zero;

    public float PlayerDistance = 0.0F;

    void Start()
    {
        Initialize();
        SwitchState(BearState.IDLE);

        ExploreStartEscapeTime = 0;
        ExploreStartTargetTime = UnityEngine.Random.Range(ExploreMinCycleSeconds, ExploreMaxCycleSeconds);
        ExplorePoint = transform.position;
    }

    public override void OnEnemyUpdate()
    {
        if (isDead)
            return;

        PlayerDistance = Vector2.Distance(PlayerObject.transform.position, transform.position);
        switch (State)
        {
            case BearState.IDLE:
                HandleIdle();
                break;
            case BearState.ATTACK:
                HandleAttack();
                break;
            default:
                throw new Exception("AI State이 기능으로 구현되지 않았습니다.");
        }
    }

    public override void OnEnemyFixedUpdate()
    {
    }

    public void HandleIdle()
    {
        bool IsExploreTime = ExploreStartEscapeTime > ExploreStartTargetTime;
        bool IsLeft = ExploreTargetPoint.x - transform.position.x > 0;

        if (isDebug)
        {
            Debug.DrawRay(transform.position + new Vector3(IsLeft ? -2.8F : 2.8F, -0.7F), IsLeft ? Vector3.left : Vector3.right, Color.red);
            Debug.DrawRay(transform.position + new Vector3(IsLeft ? -2.8F : 2.8F, 0.35F), IsLeft ? Vector3.left : Vector3.right, Color.blue);
        }

        bool IsPredictGround = Physics2D.Raycast(transform.position + new Vector3(IsLeft ? -2.8F : 2.8F, -0.7F), IsLeft ? Vector3.left : Vector3.right, 0.5f, LayerMask.GetMask("Ground")).collider != null;
        bool IsPredictFront = Physics2D.Raycast(transform.position + new Vector3(IsLeft ? -2.8F : 2.8F, 0.35F), IsLeft ? Vector3.left : Vector3.right, 0.5f, LayerMask.GetMask("Ground")).collider != null;

        if (IsIdleExplore)
        {
            float DirectionOffset = ExploreTargetPoint.x - transform.position.x;
            if (Mathf.Abs(DirectionOffset) < 3F)
            {
                IsIdleExplore = false;
                ExploreStartEscapeTime = 0;
                ExploreStartTargetTime = UnityEngine.Random.Range(ExploreMinCycleSeconds, ExploreMaxCycleSeconds);
                // ExplorePoint = transform.position; // 기존 위치를 버리고 새로운 지점을 기준으로 탐색할건가?
                SetAnimationState("Go_Player", false);
                return;
            }

            if (!IsPredictGround || IsPredictFront)
            {
                IsIdleExplore = false;
                ExploreStartEscapeTime = 0;
                ExploreStartTargetTime = UnityEngine.Random.Range(ExploreMinCycleSeconds, ExploreMaxCycleSeconds);
                SetAnimationState("Go_Player", false);
                EnemyRigidbody2D.AddRelativeForce(transform.right, ForceMode2D.Impulse);
                EnemyRigidbody2D.velocity = new Vector3(0, EnemyRigidbody2D.velocity.y);
                log("멈추기");
                return;
            }

            TargetPosition.x = math.sign(DirectionOffset);
            EnemyRigidbody2D.AddRelativeForce(transform.right * ExploreMoveSpeed, ForceMode2D.Impulse);
            EnemyRigidbody2D.velocity = new Vector3((DirectionOffset > 0 ? -ExploreMoveSpeed : ExploreMoveSpeed), EnemyRigidbody2D.velocity.y);
            transform.localScale = new Vector3(DirectionOffset > 0 ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            log("Movement");
        }
        else if (!IsIdleExplore && IsExploreTime)
        {
            IsIdleExplore = true;
            do
            {
                ExploreTargetPoint = ExplorePoint + new Vector2(UnityEngine.Random.Range(ExploreMinOffset, ExploreMaxOffset), 0);
            } while (Mathf.Abs(Vector2.Distance(ExploreTargetPoint, transform.position)) < ExploreMinOffsetValue);
            ExploreStartEscapeTime = ExploreStartTargetTime = 0;
            SetAnimationState("Go_Player", true);
        }
        else
        {
            ExploreStartEscapeTime += Time.deltaTime;
            SetAnimationState("Go_Player", false);
        }
    }

    public void HandleAttack()
    {
        transform.localScale = new Vector3(TargetXOffset < 0 ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        if (!RaycastPlayer())
        {
            SetAnimationState("attack", false);
            SwitchState(BearState.IDLE);
            return;
        }

        if (TargetXDistance < 8.0f)
        {
            SetAnimationState("attack", true);
        }
        else
        {
            SetAnimationState("attack", false);
            if (EnemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("run"))
            {
                TargetPosition.x = math.sign(TargetXOffset);
                EnemyRigidbody2D.AddRelativeForce(transform.right * ExploreMoveSpeed, ForceMode2D.Impulse);
                EnemyRigidbody2D.velocity = new Vector3((TargetXOffset < 0 ? -Speed : Speed), EnemyRigidbody2D.velocity.y);
            }
        }
    }

    public void OnAttackShake()
    {
        ShakeCameraScript.ShakingCamera(0.3F, 1, true);
    }

    public override void OnDieEvent()
    {
    }

    public void SwitchState(BearState NewState)
    {
        State = NewState;

        if (NewState == BearState.IDLE)
        {
            SetAnimationState("attack", false);
            ExploreStartEscapeTime = 0;
            ExploreStartTargetTime = UnityEngine.Random.Range(ExploreMinCycleSeconds, ExploreMaxCycleSeconds);
        }
    }

    public override void OnUpdateNearbyPlayerAnimation()
    {
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;
        if (State == BearState.IDLE && RaycastPlayer())
            SwitchState(BearState.ATTACK);
        else if (State == BearState.ATTACK)
            OnTriggerStay2D_(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Player")
            return;
        if (State == BearState.ATTACK)
            SwitchState(BearState.IDLE);
        else
            OnTriggerExit2D_(collision);
    }

    public bool RaycastPlayer()
    {
        Vector3 Direction = (PlayerObject.transform.position + new Vector3(0, 1.5F)) - (transform.position + new Vector3(0, 2.5F));
        if (isDebug)
        {
            Debug.DrawRay(transform.position + new Vector3(0, 2.5F), Direction, Color.blue, 5);
        }
        return Physics2D.Raycast(transform.position + new Vector3(0, 2.5F), Direction + new Vector3(0, 2.5F), Vector2.Distance(PlayerObject.transform.position, transform.position), LayerMask.GetMask("Ground")).collider == null;
    }

}