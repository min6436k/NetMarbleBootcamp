using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnWeapon : MonoBehaviour
{
    public Weapondata weapondata;
    private int Dir;
    private SpriteRenderer sprite;
    private Animation anim;

    private void Start()
    {

        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animation>();
        sprite.sprite = weapondata.icon;
        anim.clip = weapondata.motion;
    }

    public void start()
    {
        this.gameObject.SetActive(true);
        anim.Play();
    }

    public void end()
    {
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //���� ���� �ݶ��̴� �κ� Excludlayer�� Weapon �����ϱ�
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().CurrentHealth -= weapondata.Data.ATK;
            Debug.Log(weapondata.Data.ATK + "������, ���� ü�� = " + collision.gameObject.GetComponent<Enemy>().CurrentHealth);
        }
        
    }
}
