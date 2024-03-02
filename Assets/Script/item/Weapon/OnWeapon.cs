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
        //몹에 감지 콜라이더 부분 Excludlayer에 Weapon 설정하기
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().CurrentHealth -= weapondata.Data.ATK;
            Debug.Log(weapondata.Data.ATK + "데미지, 남은 체력 = " + collision.gameObject.GetComponent<Enemy>().CurrentHealth);
        }
        
    }
}
