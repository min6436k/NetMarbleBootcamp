using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DropInformation
{
    [Header("����")]
    public string name;

    [Header("Ȯ��")]
    public double percent = 100;

    [Header("���� �� ����")]
    public Fooddata food;
    public Weapondata weapon;
}

public abstract class Enemy : MonoBehaviour
{

    private EnemyController Controller;

    protected GameObject PlayerObject;
    protected player PlayerController;

    protected Animator EnemyAnimator;
    protected Collider2D EnemyCollider2D;
    protected Rigidbody2D EnemyRigidbody2D;

    protected GameObject TargetObject = null;
    protected float TargetXOffset = 0, TargetXDistance = 0;
    protected Vector2 TargetPosition = Vector2.zero;

    protected Vector2 DroppedPosition = Vector2.zero;

    private GameObject AttackObject;
    protected EnemyAttack AttackController;

    private GameObject HealthBarObject;
    private GameObject HealthBarUI;
    private Slider HealthSlider;

    public ShakeCamera ShakeCameraScript;

    private Dictionary<object, double> DropData = new Dictionary<object, double>();

    [Header("�ӵ�")]
    public float Speed = 2;
    public float CurrentSpeed = 2;

    [Header("ü��")]
    public int MaxHealth = 20;
    public int CurrentHealth;

    [Header("���ݷ�")]
    public int ATK = 20;

    [Header("�˹�")]
    public Vector2 knockback = Vector2.zero;

    [Header("����")]
    public bool isFollowingPlayer = false;
    public bool isDead = false;

    [Header("��� ������")]
    public GameObject BaseItemObject;
    public List<DropInformation> DropInformations = new List<DropInformation>();

    [Header("ü�¹�")]
    public Canvas GlobalCanvas;
    public GameObject HealthBarPrefeb;
    public float HealthBarWidthScale = 1.5F, HealthBarHeightScale = 2.0F;
    public float HealthBarHeight = 0.0F;

    [Header("���߿�")]
    public bool isDebug = false;

    protected void Initialize()
    {
        ShakeCameraScript = GameObject.FindGameObjectWithTag("Controller").GetComponent<ShakeCamera>();
        if (ShakeCameraScript == null)
            throw new Exception("Controller�� Shake Camera ��ũ��Ʈ�� �߰����ּ���.");

        GameObject ControllerObject = GameObject.FindGameObjectWithTag("Controller");
        if (ControllerObject == null)
            throw new Exception("�� ��ü�� �߰��Ϸ��� Scene�� \"Controller\" Prefeb(��)�� �߰��ؾ��մϴ�.");
        Controller = ControllerObject.GetComponent<EnemyController>();
        if (Controller == null)
            throw new Exception("Controller �±׸� ���� ���ӿ�����Ʈ�� �߸��Ǿ����ϴ�.");

        PlayerObject = GameObject.FindGameObjectWithTag("Player");
        if (PlayerObject == null)
            throw new Exception("���� Scene���� �÷��̾ ã�� �� �����ϴ�.");
        PlayerController = PlayerObject.GetComponent<player>();
        if (PlayerController == null)
            throw new Exception("Player �±׸� ���� ���ӿ�����Ʈ�� �߸��Ǿ����ϴ�.");

        EnemyAnimator = GetComponent<Animator>();
        if (EnemyAnimator == null)
            throw new Exception("Animator(��)�� ã�� �� �����ϴ�.");
        EnemyCollider2D = GetComponent<Collider2D>();
        if (EnemyCollider2D == null)
            throw new Exception("Collider2D(��)�� ã�� �� �����ϴ�.");
        EnemyRigidbody2D = GetComponent<Rigidbody2D>();
        if (EnemyRigidbody2D == null)
            throw new Exception("Rigidbody2D(��)�� ã�� �� �����ϴ�.");

        InitializeAttack();

        if (BaseItemObject == null)
            throw new Exception("Prefab���� BaseItem(��)�� ��ũ��Ʈ�� �������ּ���.");

        TargetObject = PlayerObject;
        CurrentHealth = MaxHealth;
        CurrentSpeed = Speed;

        InitializeDropData();
        InitializeHealthBar();
    }

    protected virtual void InitializeAttack()
    {
        Transform AttackTransform = transform.Find("AttackObject");
        if (AttackTransform == null)
            throw new Exception("AttackObject(��)�� ã�� �� �����ϴ�.");
        AttackObject = AttackTransform.gameObject;
        AttackController = AttackObject.GetComponent<EnemyAttack>();
        if (AttackController == null)
            throw new Exception("AttackObject���� AttackController(��)�� ã�� �� �����ϴ�.");
    }

    protected virtual void InitializeDropData()
    {
        foreach (DropInformation drop in DropInformations)
        {
            DropData.Add(drop.food != null ? drop.food : drop.weapon, drop.percent);
        }
    }

    protected virtual void InitializeHealthBar()
    {
        if (GlobalCanvas == null)
            throw new Exception("UI�� �׸��� ���� \"GlobalCanvas\"(��)�� �߰����ּ���.");
        if (HealthBarPrefeb == null)
            throw new Exception("ü�¹ٸ� �����ϱ� ���� Prefeb(��)�� �߰��ؾ��մϴ�.");
        Transform HealthBarTransform = transform.Find("HealthBar");
        if (HealthBarTransform == null)
            throw new Exception("ü�¹� ��ġ�� �߽��� �� ���� ������Ʈ�� \"HealthBar\"(��)�� �߰����ּ���.");
        HealthBarObject = HealthBarTransform.gameObject;

        HealthBarUI = Instantiate(HealthBarPrefeb, GlobalCanvas.transform);
        HealthSlider = HealthBarUI.GetComponent<Slider>();
        HealthSlider.interactable = false;
    }

    protected virtual void ResizeHealthBar(float WidthScale, float HeightScale)
    {
        if (HealthBarUI != null)
            HealthBarUI.transform.localScale = new Vector3(WidthScale, HeightScale, 0);
    }

    protected void Update()
    {
        if(math.sign(PlayerObject.transform.position.x - transform.position.x) != math.sign(knockback.x))
            knockback.x *= -1;
        if (TargetObject != null)
        {
            TargetXOffset = TargetObject.transform.position.x - transform.position.x;
            TargetXDistance = Mathf.Abs(TargetXOffset);
        }
        else
            SetAnimationState("attack", false);

        if (HealthBarUI != null)
        {
            Vector3 HpBarPos = Camera.main.WorldToScreenPoint(new Vector3(HealthBarObject.transform.position.x, HealthBarObject.transform.position.y + HealthBarHeight, 0));
            HealthBarUI.transform.position = HpBarPos;
            HealthSlider.value = Mathf.Lerp(HealthSlider.value, CurrentHealth / (float) MaxHealth, Time.deltaTime * 10);
        }

        if (!isDead && CurrentHealth <= 0)
            OnDie();

        OnEnemyUpdate();
        ResizeHealthBar(HealthBarWidthScale, HealthBarHeightScale);
    }

    protected void FixedUpdate()
    {
        if (this is not BossEnemy && IsTriggerAnimation("Go_Player"))
        {
            OnUpdateNearbyPlayerAnimation();
        }
        OnEnemyFixedUpdate();
    }

    protected void OnTriggerStay2D_(Collider2D collision)
    {
        OnTriggerStay2D(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && isFollowingPlayer != true)
            SetAnimationState("Go_Player", true);
    }

    protected void OnTriggerExit2D_(Collider2D collision)
    {
        OnTriggerExit2D(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            SetAnimationState("Go_Player", false);
    }

    public void BeforeAttackAnimation()
    {
        if (isDead)
            return;
        AttackController.BeforeAttack();
    }

    public void AfterAttackAnimation()
    {
        if (isDead)
            return;
        AttackController.AfterAttack();
    }

    public virtual void OnDropItem()
    {
        object droppedItemData = getWeightedRandom(DropData, null);
        if (droppedItemData == null)
            return;

        GameObject droppedObject = Instantiate(BaseItemObject);
        SpriteRenderer renderer = droppedObject.GetComponent<SpriteRenderer>();

        if (droppedItemData is Fooddata fooditem)
        {
            renderer.sprite = fooditem.icon;
            renderer.transform.localScale = Vector3.one;
        }
        else if (droppedItemData is Fooddata weaponitem)
        {
            renderer.sprite = weaponitem.icon;
            renderer.transform.localScale = Vector3.one;
        }

        droppedObject.transform.position = DroppedPosition != Vector2.zero ? DroppedPosition : transform.position;
        droppedObject.SetActive(true);

        DroppedItemController droppedController = droppedObject.GetComponent<DroppedItemController>();
        droppedController.Initialize(droppedItemData);
    }

    public virtual void OnDie()
    {
        if (isDead)
            return;
        isDead = true;
        log("���� ���� �̺�Ʈ�� ����Ǿ����ϴ�.");

        OnDieEvent();

        Destroy(HealthBarUI);
        HealthBarUI = null;
        TriggerAnimation("die");

        DroppedPosition = transform.position;

        EnemyCollider2D.enabled = false;
        EnemyRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        foreach (Transform child in transform)
        {
            Collider2D collider2D = child.GetComponent<Collider2D>();
            if (collider2D != null)
                collider2D.enabled = false;
        }

        EnemyRigidbody2D.AddRelativeForce(transform.right);
        EnemyRigidbody2D.velocity = new Vector3(0, 0);

        AttackObject.SetActive(false);
    }

    public virtual void OnPreDead()
    {
        EnemyRigidbody2D.velocity = Vector3.zero;
        EnemyRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        SetAnimationState("Go_Player", false);

        foreach (Transform child in transform)
        {
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                spriteRenderer.sortingOrder -= 10;
        }
    }

    public virtual void OnPostDead()
    {
        log("���� �Ϸ� �̺�Ʈ�� ����Ǿ����ϴ�.");
        OnDropItem();
        Destroy(this.gameObject);
    }

    public void OnDamageBy(GameObject Attacker, int Damage)
    {
        string AttackerName = Attacker != null ? Attacker.name : string.Empty;
        log("������ �̺�Ʈ�� ����Ǿ����ϴ�: " + AttackerName + "�� ������ " + Damage + "(��)�� �����߽��ϴ�.");

        SetHealth(CurrentHealth - Damage);
        if (CurrentHealth == 0)
        {
            OnDie();
        }
    }

    public void SetAnimationState(string Name, bool State)
    {
        EnemyAnimator.SetBool(Name, State);
    }

    public void TriggerAnimation(string name)
    {
        EnemyAnimator.SetTrigger(name);
    }

    public bool IsTriggerAnimation(string Name)
    {
        return EnemyAnimator.GetBool(Name);
    }

    private void SetHealth(int health)
    {
        CurrentHealth = math.max(0, health);
    }

    public string GetName()
    {
        return gameObject.name;
    }

    protected void log(string message)
    {
        if (isDebug)
            Debug.Log(GetName() + "(Enemy): " + message);
    }

    public abstract void OnEnemyUpdate();

    public abstract void OnEnemyFixedUpdate();

    public abstract void OnUpdateNearbyPlayerAnimation();

    public abstract void OnDieEvent();

    private E getWeightedRandom<E>(Dictionary<E, double> weights, E defaultValue)
    {
        E result = defaultValue;
        double bestValue = double.MaxValue;

        foreach (E element in weights.Keys)
        {
            double value = -math.log(UnityEngine.Random.Range(0, float.MaxValue)) / weights.GetValueOrDefault(element);
            if (value < bestValue)
            {
                bestValue = value;
                result = element;
            }
        }
        return result;
    }

}

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
