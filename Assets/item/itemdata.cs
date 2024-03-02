using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Weapontype
{
    nomal,
    cooktool
}

public enum Foodtype
{
    main_ingre,//�����
    sub_ingre, //�����
    spice,    //��ŷ�
    ______________,
    steaming, //���
    Boiling,  //���̱�
    Baking,   //����
    Frying,   //Ƣ���
    drying,   //������
}

[System.Serializable]
public class Weaponitem
{
    public int ATK;
    public Weapontype type;
}

[System.Serializable]
public class Fooditem
{
    public string name;
    public string info;
    public int H_Regen = 0;
    public int EXP = 0;
    public Foodtype type;
    public float ATK_Buff = 0;
    public float Speed_Buff = 0;
    public float HP_Max_Buff = 0;
    public float HP_Regen_Buff = 0;
    public float Resist = 0;
}
