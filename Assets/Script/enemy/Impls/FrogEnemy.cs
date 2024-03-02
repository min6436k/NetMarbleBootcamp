using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

// 2번째 작성 tq;

[SerializeField]
public enum FrogState
{
    IDLE,
    ATTACK,
    AVOIDING,
}

public class FrogEnemy : Enemy
{

    [Header("AI - 상태")]
    public FrogState State = FrogState.IDLE;
    public bool IsAngry = false;

    [Header("AI - 플레이어 피하기")]
    public float AvoidMoveSpeed = 2.35F;
    public float MinAvoidOffsetSeconds = 0.5F; // Avoid Distance를 벗어났을 때, 얼마나 추가로 움직일 것인가?
    public float MaxAvoidOffsetSeconds = 3; // Avoid Distance를 벗어났을 때, 얼마나 추가로 움직일 것인가?
    private float AvoidOffsetTargetTime = 0;
    private float AvoidOffsetEscapeTime = 0;

    [Header("AI - 공격")]
    public float AttackDistance = 10;
    public bool IsAttack = false;

    [Header("AI - 주변 탐색")]
    public bool IsIdleExplore = false;
    public float ExploreMinCycleSeconds = 1F; // 자동 탐색 주기
    public float ExploreMaxCycleSeconds = 5F; // 자동 탐색 주기
    private float ExploreStartTargetTime = 0;
    private float ExploreStartEscapeTime = 0;
    public float ExploreMoveXSpeed = 3F;
    public float ExploreMoveYSpeed = 6F;
    public float ExploreMinOffset = -10F, ExploreMaxOffset = 10F; // 중심 좌표로 부터 얼마만큼 움직일 것인가?
    public float ExploreMinOffsetValue = 3F; // 움직일 거리가 최소 이정도여야한다.
    public bool ExploreDone = false;
    public float ExploreDirection = 0.0F;
    private Vector2 ExplorePoint = Vector2.zero;
    private Vector2 ExploreTargetPoint = Vector2.zero;

    public float PlayerDistance = 0.0F;

    [Header("움직임 상태")]
    public bool IsGround = false;
    public bool IsJumpingAnimation = false, IsJumpable = true, IsJumping = false;

    void Start()
    {
        Initialize();
        SwitchState(FrogState.IDLE);

        ExploreStartEscapeTime = 0;
        ExploreStartTargetTime = UnityEngine.Random.Range(ExploreMinCycleSeconds, ExploreMaxCycleSeconds);
        ExplorePoint = transform.position;
    }

    public override void OnEnemyUpdate()
    {
        if (isDead)
            return;

        Debug.DrawRay(transform.position - new Vector3(1.12F, 0), Vector2.down * 1.0f, Color.green, 0);
        Debug.DrawRay(transform.position, Vector2.down * 1.0f, Color.green, 0);
        Debug.DrawRay(transform.position + new Vector3(1.12F, 0), Vector2.down * 1.0f, Color.green, 0);

        IsGround = false;
        IsGround |= Physics2D.Raycast(transform.position - new Vector3(1.12F, 0), Vector2.down, 1.0f, LayerMask.GetMask("Ground")).collider != null;
        IsGround |= Physics2D.Raycast(transform.position, Vector2.down, 1.0f, LayerMask.GetMask("Ground")).collider != null;
        IsGround |= Physics2D.Raycast(transform.position + new Vector3(1.12F, 0), Vector2.down, 1.0f, LayerMask.GetMask("Ground")).collider != null;

        PlayerDistance = Vector2.Distance(PlayerObject.transform.position, transform.position);
        switch (State)
        {
            case FrogState.IDLE:
                HandleIdle();
                break;
            case FrogState.ATTACK:
                HandleAttack();
                break;
            case FrogState.AVOIDING:
                HandleAvoiding();
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
        AvoidOffsetTargetTime = AvoidOffsetEscapeTime = 0;

        bool IsExploreTime = ExploreStartEscapeTime > ExploreStartTargetTime;
        bool IsLeft = ExploreDirection < 0;

        if (isDebug)
        {
            Debug.DrawRay(transform.position + new Vector3(IsLeft ? -0.5F : 0.5F, 0.1F), IsLeft ? Vector3.left : Vector3.right, Color.blue);
        }

        bool IsPredictFront = Physics2D.Raycast(transform.position + new Vector3(IsLeft ? -0.5F : 0.5F, 0.1F), IsLeft ? Vector3.left : Vector3.right, 0.5f, LayerMask.GetMask("Ground")).collider != null;

        if (IsIdleExplore)
        {
            float DirectionOffset = ExploreTargetPoint.x - transform.position.x;
            if (!ExploreDone && Mathf.Abs(DirectionOffset) < 3F)
            {
                ExploreDone = true;
                log("탐험(목표 지점 이동)이 완료되었습니다.");
            }

            transform.localScale = new Vector3(ExploreDirection < 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            if (IsGround && IsJumpable && !IsJumping && !IsTriggerAnimation("Go_Player"))
            {
                if (CheckAndSwitchAttack())
                {
                    return;
                }
                else if (ExploreDone)
                {
                    ExploreDone = false;
                    IsJumping = false;
                    IsJumpingAnimation = false;
                    IsJumpable = true;
                    IsIdleExplore = false;
                    ExploreStartEscapeTime = 0;
                    ExploreStartTargetTime = UnityEngine.Random.Range(ExploreMinCycleSeconds, ExploreMaxCycleSeconds);
                    SetAnimationState("rundown", false);
                    SetAnimationState("Go_Player", false);
                    log("탐험이 완료되었다는 것을 확인하고 타이머를 초기화합니다.");
                    return;
                }

                if (IsPredictFront)
                {
                    ExploreDone = false;
                    IsIdleExplore = false;
                    ExploreStartEscapeTime = 0;
                    ExploreStartTargetTime = UnityEngine.Random.Range(ExploreMinCycleSeconds, ExploreMaxCycleSeconds);
                    SetAnimationState("rundown", false);
                    SetAnimationState("Go_Player", false);
                    do
                    {
                        ExploreTargetPoint = ExplorePoint + new Vector2(UnityEngine.Random.Range(ExploreMinOffset, ExploreMaxOffset), 0);
                        ExploreDirection = (ExploreTargetPoint.x - transform.position.x);
                        IsLeft = ExploreDirection < 0;
                        IsPredictFront = Physics2D.Raycast(transform.position + new Vector3(IsLeft ? -0.5F : 0.5F, 0.1F), IsLeft ? Vector3.left : Vector3.right, 0.5f, LayerMask.GetMask("Ground")).collider != null;
                        transform.localScale = new Vector3(ExploreDirection < 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    } while (IsPredictFront || Mathf.Abs(Vector2.Distance(ExploreTargetPoint, transform.position)) < ExploreMinOffsetValue);
                    ExploreStartEscapeTime = ExploreStartTargetTime = 0;
                    log("앞에 장애물이 있습니다.");
                    return;
                }

                IsJumping = true;
                IsJumpable = false;
                IsJumpingAnimation = true;
                ExploreDirection = DirectionOffset;
                SetAnimationState("Go_Player", true);
                log("점프하는 중...");
            }

            if (IsGround && IsJumping && !IsJumpingAnimation)
            {
                IsJumping = false;
                SetAnimationState("rundown", true);
                SetAnimationState("Go_Player", false);
                TargetPosition.x = math.sign(ExploreDirection);
                EnemyRigidbody2D.AddRelativeForce(transform.right * ExploreMoveXSpeed);
                EnemyRigidbody2D.velocity = new Vector3((ExploreDirection < 0 ? -ExploreMoveXSpeed : ExploreMoveXSpeed), 0);
                log("하강을 시작하는 중...");
            }
        }
        else if (!IsIdleExplore && IsExploreTime)
        {
            IsIdleExplore = true;
            do
            {
                ExploreTargetPoint = ExplorePoint + new Vector2(UnityEngine.Random.Range(ExploreMinOffset, ExploreMaxOffset), 0);
            } while (Mathf.Abs(Vector2.Distance(ExploreTargetPoint, transform.position)) < ExploreMinOffsetValue);
            ExploreStartEscapeTime = ExploreStartTargetTime = 0;
            log("탐색 위치를 정했습니다.");
        }
        else
        {
            ExploreStartEscapeTime += Time.deltaTime;
            SetAnimationState("Go_Player", false);
        }
    }

    public void TriggerOnJumpingAnimation()
    {
        if (!IsJumpingAnimation)
        {
            log("의도하지 않은 점프 애니메이션이 발생했습니다.");
            SetAnimationState("Go_Player", false);
            SetAnimationState("rundown", false);
            return;
        }

        if (isDead)
            return;

        float DirectionOffset = State == FrogState.AVOIDING ? TargetXOffset : (State == FrogState.ATTACK ? PlayerObject.transform.position.x - transform.position.x : ExploreDirection);
        TargetPosition.x = math.sign(DirectionOffset);
        EnemyRigidbody2D.AddRelativeForce(transform.right * ExploreMoveXSpeed * ExploreMoveYSpeed);
        EnemyRigidbody2D.velocity = new Vector3((DirectionOffset < 0 ? -ExploreMoveXSpeed : ExploreMoveXSpeed) * (State == FrogState.AVOIDING ? -1 : 1), ExploreMoveYSpeed);
        log("점프를 시작하는 중");
    }

    public void TriggerOnDoneJumpingAnimation()
    {
        if (isDead)
            return;

        log("점프 모션이 끝났습니다.");
        IsJumpingAnimation = false;
    }

    public void TriggerOnEndJumpingAnimation()
    {
        if (isDead)
            return;

        log("점프가 종료되었습니다.");

        IsJumping = false;
        IsJumpingAnimation = false;
        IsJumpable = true;
        SetAnimationState("Go_Player", false);
        SetAnimationState("rundown", false);
        EnemyRigidbody2D.AddRelativeForce(transform.right);
        EnemyRigidbody2D.velocity = new Vector3(0, 0);

        if (State == FrogState.IDLE)
        {
            CheckAndSwitchAttack();
        }
    }

    public bool CheckAndSwitchAttack()
    {
        if (PlayerDistance < AttackDistance)
        {
            Vector3 Direction = (PlayerObject.transform.position + new Vector3(0, 1.5F)) - transform.position;
            if (isDebug)
            {
                Debug.DrawRay(transform.position, Direction, Color.blue, 5);
            }

            if (Physics2D.Raycast(transform.position + new Vector3(0, 1.5F), Direction, AttackDistance, LayerMask.GetMask("Ground")).collider == null)
            {
                log("상태를 공격으로 전환합니다.");
                SwitchState(FrogState.ATTACK);
                return true;
            }
        }
        return false;
    }

    public void HandleAttack()
    {
        float DirectionOffset = PlayerObject.transform.position.x - transform.position.x;
        transform.localScale = new Vector3(DirectionOffset < 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        if (!IsAttack && IsGround && IsJumpable && !IsJumping && PlayerDistance < 7.5f && !IsTriggerAnimation("Go_Player"))
        {
            log("공격을 시작합니다.");
            IsAttack = true;
            IsJumping = false;
            IsJumpingAnimation = false;
            IsJumpable = true;
            SetAnimationState("rundown", false);
            SetAnimationState("Go_Player", false);
            SetAnimationState("attack", true);
        }
        else if (!IsAttack)
        {
            SetAnimationState("attack", false);
            if (PlayerDistance > AttackDistance)
            {
                log("플레이어가 너무 멀어져서 Idle로 전환합니다.");
                SwitchState(FrogState.IDLE);
                return;
            }
            else if (IsGround && IsJumpable && !IsJumping)
            {
                log("점프를 시작합니다.");
                IsJumping = true;
                IsJumpable = false;
                IsJumpingAnimation = true;
                SetAnimationState("Go_Player", true);
            }
            else if (IsGround && IsJumping && !IsJumpingAnimation)
            {
                log("하강을 시작합니다.");
                IsJumping = false;
                SetAnimationState("rundown", true);
                SetAnimationState("Go_Player", false);
                TargetPosition.x = math.sign(DirectionOffset);
                EnemyRigidbody2D.AddRelativeForce(transform.right * ExploreMoveXSpeed);
                EnemyRigidbody2D.velocity = new Vector3((DirectionOffset < 0 ? -ExploreMoveXSpeed : ExploreMoveXSpeed), 0);
            }
        }
    }

    public void HandleAvoiding()
    {
        IsIdleExplore = false;
        ExploreStartEscapeTime = 0;

        bool IsLeft = ExploreDirection < 0;

        if (isDebug)
        {
            Debug.DrawRay(transform.position + new Vector3(IsLeft ? -0.5F : 0.5F, 0.1F), IsLeft ? Vector3.left : Vector3.right, Color.blue);
        }

        bool IsPredictFront = Physics2D.Raycast(transform.position + new Vector3(IsLeft ? -0.5F : 0.5F, 0.1F), IsLeft ? Vector3.left : Vector3.right, 0.5f, LayerMask.GetMask("Ground")).collider != null;

        transform.localScale = new Vector3(TargetXOffset > 0 ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        if (IsGround && IsJumpable && !IsJumping && !IsTriggerAnimation("Go_Player"))
        {
            if (
                IsPredictFront || AvoidOffsetEscapeTime > AvoidOffsetTargetTime)
            {
                log("도망치기 시간이 끝났습니다.");
                AvoidOffsetTargetTime = AvoidOffsetEscapeTime = 0;
                ExploreStartEscapeTime = 0;
                ExploreStartTargetTime = UnityEngine.Random.Range(ExploreMinCycleSeconds, ExploreMaxCycleSeconds);
                ExplorePoint = transform.position;
                IsJumping = false;
                IsJumpingAnimation = false;
                IsJumpable = true;
                IsIdleExplore = false;
                SetAnimationState("rundown", false);
                SetAnimationState("Go_Player", false);
                SwitchState(FrogState.IDLE);
                return;
            }

            log("점프를 시작하는 중... (avoid)");
            IsJumping = true;
            IsJumpable = false;
            IsJumpingAnimation = true;
            SetAnimationState("Go_Player", true);
        }
        else if (IsGround && IsJumping && !IsJumpingAnimation)
        {
            log("하강을 시작하는 중... (avoid)");
            IsJumping = false;
            SetAnimationState("rundown", true);
            SetAnimationState("Go_Player", false);
            TargetPosition.x = math.sign(-TargetXOffset);
            EnemyRigidbody2D.AddRelativeForce(transform.right * ExploreMoveXSpeed);
            EnemyRigidbody2D.velocity = new Vector3((TargetXOffset < 0 ? ExploreMoveXSpeed : -ExploreMoveXSpeed), 0);
        }

        AvoidOffsetEscapeTime += Time.deltaTime;
    }

    public override void OnDieEvent()
    {
        log("OnDieEvent");
        SetAnimationState("rundown", false);
        SetAnimationState("Go_Player", false);
        SetAnimationState("attack", false);
    }

    public void TriggerAvoid()
    {
        if (isDead)
            return;

        log("공격이 끝났습니다. 도망치는 상태로 전환합니다.");
        IsAttack = false;
        SetAnimationState("attack", false);
        SetAnimationState("rundown", false);
        SetAnimationState("Go_Player", false);
        SwitchState(FrogState.AVOIDING);
    }

    public void SwitchState(FrogState NewState)
    {
        if (isDead)
            return;

        State = NewState;
        IsAttack = false;

        if (NewState == FrogState.IDLE)
        {
            ExplorePoint = transform.position;
            ExploreDone = false;
        }
        else if (NewState == FrogState.AVOIDING)
        {
            AvoidOffsetEscapeTime = 0;
            AvoidOffsetTargetTime = UnityEngine.Random.Range(MinAvoidOffsetSeconds, MaxAvoidOffsetSeconds);
        }
    }

    public override void OnUpdateNearbyPlayerAnimation()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
    }

}