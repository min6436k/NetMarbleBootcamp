using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

// 2��° �ۼ� tq;

[SerializeField]
public enum FrogState
{
    IDLE,
    ATTACK,
    AVOIDING,
}

public class FrogEnemy : Enemy
{

    [Header("AI - ����")]
    public FrogState State = FrogState.IDLE;
    public bool IsAngry = false;

    [Header("AI - �÷��̾� ���ϱ�")]
    public float AvoidMoveSpeed = 2.35F;
    public float MinAvoidOffsetSeconds = 0.5F; // Avoid Distance�� ����� ��, �󸶳� �߰��� ������ ���ΰ�?
    public float MaxAvoidOffsetSeconds = 3; // Avoid Distance�� ����� ��, �󸶳� �߰��� ������ ���ΰ�?
    private float AvoidOffsetTargetTime = 0;
    private float AvoidOffsetEscapeTime = 0;

    [Header("AI - ����")]
    public float AttackDistance = 10;
    public bool IsAttack = false;

    [Header("AI - �ֺ� Ž��")]
    public bool IsIdleExplore = false;
    public float ExploreMinCycleSeconds = 1F; // �ڵ� Ž�� �ֱ�
    public float ExploreMaxCycleSeconds = 5F; // �ڵ� Ž�� �ֱ�
    private float ExploreStartTargetTime = 0;
    private float ExploreStartEscapeTime = 0;
    public float ExploreMoveXSpeed = 3F;
    public float ExploreMoveYSpeed = 6F;
    public float ExploreMinOffset = -10F, ExploreMaxOffset = 10F; // �߽� ��ǥ�� ���� �󸶸�ŭ ������ ���ΰ�?
    public float ExploreMinOffsetValue = 3F; // ������ �Ÿ��� �ּ� �����������Ѵ�.
    public bool ExploreDone = false;
    public float ExploreDirection = 0.0F;
    private Vector2 ExplorePoint = Vector2.zero;
    private Vector2 ExploreTargetPoint = Vector2.zero;

    public float PlayerDistance = 0.0F;

    [Header("������ ����")]
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
                throw new Exception("AI State�� ������� �������� �ʾҽ��ϴ�.");
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
                log("Ž��(��ǥ ���� �̵�)�� �Ϸ�Ǿ����ϴ�.");
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
                    log("Ž���� �Ϸ�Ǿ��ٴ� ���� Ȯ���ϰ� Ÿ�̸Ӹ� �ʱ�ȭ�մϴ�.");
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
                    log("�տ� ��ֹ��� �ֽ��ϴ�.");
                    return;
                }

                IsJumping = true;
                IsJumpable = false;
                IsJumpingAnimation = true;
                ExploreDirection = DirectionOffset;
                SetAnimationState("Go_Player", true);
                log("�����ϴ� ��...");
            }

            if (IsGround && IsJumping && !IsJumpingAnimation)
            {
                IsJumping = false;
                SetAnimationState("rundown", true);
                SetAnimationState("Go_Player", false);
                TargetPosition.x = math.sign(ExploreDirection);
                EnemyRigidbody2D.AddRelativeForce(transform.right * ExploreMoveXSpeed);
                EnemyRigidbody2D.velocity = new Vector3((ExploreDirection < 0 ? -ExploreMoveXSpeed : ExploreMoveXSpeed), 0);
                log("�ϰ��� �����ϴ� ��...");
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
            log("Ž�� ��ġ�� ���߽��ϴ�.");
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
            log("�ǵ����� ���� ���� �ִϸ��̼��� �߻��߽��ϴ�.");
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
        log("������ �����ϴ� ��");
    }

    public void TriggerOnDoneJumpingAnimation()
    {
        if (isDead)
            return;

        log("���� ����� �������ϴ�.");
        IsJumpingAnimation = false;
    }

    public void TriggerOnEndJumpingAnimation()
    {
        if (isDead)
            return;

        log("������ ����Ǿ����ϴ�.");

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
                log("���¸� �������� ��ȯ�մϴ�.");
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
            log("������ �����մϴ�.");
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
                log("�÷��̾ �ʹ� �־����� Idle�� ��ȯ�մϴ�.");
                SwitchState(FrogState.IDLE);
                return;
            }
            else if (IsGround && IsJumpable && !IsJumping)
            {
                log("������ �����մϴ�.");
                IsJumping = true;
                IsJumpable = false;
                IsJumpingAnimation = true;
                SetAnimationState("Go_Player", true);
            }
            else if (IsGround && IsJumping && !IsJumpingAnimation)
            {
                log("�ϰ��� �����մϴ�.");
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
                log("����ġ�� �ð��� �������ϴ�.");
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

            log("������ �����ϴ� ��... (avoid)");
            IsJumping = true;
            IsJumpable = false;
            IsJumpingAnimation = true;
            SetAnimationState("Go_Player", true);
        }
        else if (IsGround && IsJumping && !IsJumpingAnimation)
        {
            log("�ϰ��� �����ϴ� ��... (avoid)");
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

        log("������ �������ϴ�. ����ġ�� ���·� ��ȯ�մϴ�.");
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