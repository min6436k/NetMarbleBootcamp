using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class player : MonoBehaviour
{
    public GameObject respon;
    public GameObject bag;

    public GameObject W1;
    public GameObject W2;

    public ShakeCamera ShakeCameraScript;

    public Allitem all_item;

    private Rigidbody2D rigid;
    private Animator anim;

    private float imsi;

    public List<int> initem;

    public float MaxHungry = 100;
    public float hungry;

    public int Level;

    public int EXP;
    public int nextEXP;

    public float send_damage = 1;       //�ִµ���
    public float riceived_damage = 1;   //�޴µ���
    public float HP_Regen_magnifying = 1;   //1ȸ�� ü��ȸ������ (1.25�� 25%�߰�)
    public float HP_less_magnifying = 1; //�ٴ� ��� ����
    public float max_HP = 1;            //�ִ� ��� ����
    public float speed_magnifying = 1; //�̼�, ���� ����

    //���� ���� ����ϴ°� ��������


    private void Awake()
    {
        
    }

    void Start()
    {
        hungry = MaxHungry;

        rigid = GetComponent<Rigidbody2D>();    
        bag.GetComponent<Bag>().loaditem(initem);
        anim = GetComponent<Animator>();
        ShakeCameraScript = GameObject.FindGameObjectWithTag("Controller").GetComponent<ShakeCamera>();
    }

    void Update()
    {

        if (hungry <= 0)
            PlayerDie();
        if (Input.GetKeyDown(KeyCode.Z))
            Attack1();
        if (Input.GetKeyDown(KeyCode.X))
            Attack2();
        if (Input.GetKeyDown(KeyCode.C))
            ShakeCameraScript.ShakingCamera(0.01F, 3, false);
        if (Input.GetKeyDown(KeyCode.V))
            ShakeCameraScript.ShakingCamera(0.1F, 3, false);
        if (Input.GetKeyDown(KeyCode.B))
            ShakeCameraScript.ShakingCamera(0.1F, 3, true);

        if (Input.GetKeyDown(KeyCode.Q))
            for (int i = 0; i < all_item.ALLFood.Count; i++)
            {
                additem(all_item.ALLFood[i]);
            }
    }

    private void FixedUpdate()
    {
    }

    public void additem(object item)
    {

        if (item is Fooddata)
        {
            Fooddata addfood = (Fooddata)item;
            initem.Add(all_item.ALLFood.IndexOf(addfood));
        }

        if (item is Weapondata)
        {
            Weapondata addweapon = (Weapondata)item;
            initem.Add(all_item.ALLWeapon.IndexOf(addweapon));
        }

        bag.GetComponent<Bag>().loaditem(initem);
    }

    public void delFood(Fooddata item)
    {
        for (int i = initem.Count-1; i >= 0; i--)
        {
            if (all_item.ALLFood.IndexOf(item) == initem[i])
            {
                initem.RemoveAt(i);
                bag.GetComponent<Bag>().loaditem(initem);
                break;
            }
        }

    }

    public void eat(Fooddata item)
    {
        hungry += (item.Data.H_Regen * HP_Regen_magnifying);
        EXP += item.Data.EXP;
        delFood(item);
    }

    public void Attack1()
    {
        Debug.Log("����1 ����, ������ : " + W1.GetComponent<OnWeapon>().weapondata.Data.ATK);
        W1.SetActive(true);
    }

    public void Attack2()
    {
        Debug.Log("����2 ����, ������ : " + W2.GetComponent<OnWeapon>().weapondata.Data.ATK);
        W2 .SetActive(true);
    }

    public void OnReceviceDamage(GameObject DamageBy, int Damage, Vector2 knockback)
    {
        if(math.sign(knockback.x) == math.sign(transform.localScale.x)) new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z); ;
        rigid.velocity = Vector2.zero;
        rigid.AddForce(knockback, ForceMode2D.Impulse);
        anim.SetBool("knockback", true);

        hungry -= Damage;
        Debug.Log(DamageBy.name + "(��)�κ��� �������� �Ծ����ϴ�. ���� ü��: " + hungry);
    }

    public void OnEatItem(object item)
    {
        additem(item);

        if (item is Fooddata fooditem)
            Debug.Log("���� ��Ḧ ������ �����ϴ�: " + fooditem.name + " (" + fooditem.Data.info + ")");
        else if (item is Weapondata weaponitem)
            Debug.Log("���⸦ ������ �����ϴ�: " + weaponitem.Data.ATK);
    }

    public void PlayerDie()
    {
        anim.SetBool("die",true);
        Debug.Log("�׾����ϴ�");
        respon.SetActive(true);
    }

}
