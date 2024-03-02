using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[SerializeField]
public enum ChickenState
{
    IDLE,
    IDLE_EXPLORE,
    AVOIDING,
    ANGRY,
}

public class ChickenEnemy : Enemy
{

    [Header("AI - 상태")]
    public ChickenState State = ChickenState.IDLE;
    public bool IsAngry = false;

    [Header("AI - 플레이어 피하기")]
    public bool IsAvoiding = false;
    public float AvoidMoveSpeed = 2.35F;
    public float AvoidDistance = 3.0F;
    public float MinAvoidOffsetSeconds = 0.5F; // Avoid Distance를 벗어났을 때, 얼마나 추가로 움직일 것인가?
    public float MaxAvoidOffsetSeconds = 3; // Avoid Distance를 벗어났을 때, 얼마나 추가로 움직일 것인가?
    private float AvoidOffsetTargetTime = 0;
    private float AvoidOffsetEscapeTime = 0;

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
        SwitchState(ChickenState.IDLE);

        ExploreStartEscapeTime = 0;
        ExploreStartTargetTime = UnityEngine.Random.Range(ExploreMinCycleSeconds, ExploreMaxCycleSeconds);
        ExplorePoint = transform.position;
    }

    public override void OnEnemyUpdate()
    {
        if (isDead)
            return;

        if (Input.GetKeyDown(KeyCode.F))
            OnEggSteal();

        PlayerDistance = Vector2.Distance(PlayerObject.transform.position, transform.position);
        switch (State)
        {
            case ChickenState.IDLE:
                HandleIdle();
                break;
            case ChickenState.ANGRY:
                HandleAngry();
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
        bool InAvoidArea = PlayerDistance < AvoidDistance;
        bool IsRemainAvoidOffset = AvoidOffsetTargetTime > 0;
        bool IsExploreTime = ExploreStartEscapeTime > ExploreStartTargetTime;

        bool IsLeft = (InAvoidArea || IsRemainAvoidOffset) ? TargetXOffset > 0 : (ExploreTargetPoint.x - transform.position.x) < 0;
        
        if (isDebug)
        {
            Debug.DrawRay(transform.position + new Vector3(IsLeft ? -0.5F : 0.5F, -0.7F), IsLeft ? Vector3.left : Vector3.right, Color.red);
            Debug.DrawRay(transform.position + new Vector3(IsLeft ? -0.5F : 0.5F, 0.35F), IsLeft ? Vector3.left : Vector3.right, Color.blue);
        }
        
        bool IsPredictGround = Physics2D.Raycast(transform.position + new Vector3(IsLeft ? -0.5F : 0.5F, -0.7F), IsLeft ? Vector3.left : Vector3.right, 0.5f, LayerMask.GetMask("Ground")).collider != null;
        bool IsPredictFront = Physics2D.Raycast(transform.position + new Vector3(IsLeft ? -0.5F : 0.5F, 0.35F), IsLeft ? Vector3.left : Vector3.right, 0.5f, LayerMask.GetMask("Ground")).collider != null;

        if (InAvoidArea || IsRemainAvoidOffset)
        {
            IsIdleExplore = false;
            ExploreStartEscapeTime = 0;

            if (!IsPredictGround || IsPredictFront)
            {
                IsAvoiding = false;
                SetAnimationState("Go_Player", false);
                transform.localScale = new Vector3(TargetXOffset < 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                EnemyRigidbody2D.AddRelativeForce(transform.right, ForceMode2D.Impulse);
                EnemyRigidbody2D.velocity = new Vector3(0, EnemyRigidbody2D.velocity.y);
                return;
            }

            if (!IsAvoiding)
            {
                IsAvoiding = true;
                AvoidOffsetTargetTime = UnityEngine.Random.Range(MinAvoidOffsetSeconds, MaxAvoidOffsetSeconds);
                SetAnimationState("Go_Player", true);
            }

            if (AvoidOffsetEscapeTime > AvoidOffsetTargetTime)
            {
                AvoidOffsetTargetTime = AvoidOffsetEscapeTime = 0;
                ExploreStartEscapeTime = 0;
                ExploreStartTargetTime = UnityEngine.Random.Range(ExploreMinCycleSeconds, ExploreMaxCycleSeconds);
                ExplorePoint = transform.position;
            }

            TargetPosition.x = math.sign(-TargetXOffset);
            EnemyRigidbody2D.AddRelativeForce(transform.right * ExploreMoveSpeed, ForceMode2D.Impulse);
            EnemyRigidbody2D.velocity = new Vector3((TargetXOffset < 0 ? AvoidMoveSpeed : -AvoidMoveSpeed), EnemyRigidbody2D.velocity.y);
            transform.localScale = new Vector3(TargetXOffset < 0 ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            if (!InAvoidArea && IsRemainAvoidOffset)
                AvoidOffsetEscapeTime += Time.deltaTime;
        }
        else
        {
            IsAvoiding = false;
            AvoidOffsetTargetTime = AvoidOffsetEscapeTime = 0;

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
                    return;
                }

                TargetPosition.x = math.sign(DirectionOffset);
                EnemyRigidbody2D.AddRelativeForce(transform.right * ExploreMoveSpeed, ForceMode2D.Impulse);
                EnemyRigidbody2D.velocity = new Vector3((DirectionOffset < 0 ? -ExploreMoveSpeed : ExploreMoveSpeed), EnemyRigidbody2D.velocity.y);
                transform.localScale = new Vector3(DirectionOffset < 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
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
    }
    public override void OnDieEvent()
    {
    }

    public void HandleAngry()
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
                EnemyRigidbody2D.AddRelativeForce(transform.right * ExploreMoveSpeed, ForceMode2D.Impulse);
                EnemyRigidbody2D.velocity = new Vector3((TargetXOffset < 0 ? -AvoidMoveSpeed : AvoidMoveSpeed), EnemyRigidbody2D.velocity.y);
            }
        }
    }

    public void SwitchState(ChickenState NewState)
    {
        State = NewState;

        if (NewState == ChickenState.IDLE)
        {
            AvoidOffsetTargetTime = AvoidOffsetEscapeTime = 0;
        }
    }

    public void OnEggSteal()
    {
        if (IsAngry)
            return;
        IsAngry = true;
        SwitchState(ChickenState.ANGRY);
        foreach (SpriteRenderer r in GetComponentsInChildren<SpriteRenderer>())
            r.color = Color.red;
    }

    public override void OnUpdateNearbyPlayerAnimation()
    {
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (State != ChickenState.ANGRY)
            return;
        base.OnTriggerStay2D_(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (State != ChickenState.ANGRY)
            return;
        base.OnTriggerExit2D_(collision);
    }

}