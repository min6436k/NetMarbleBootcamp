using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class useButton : MonoBehaviour
{
    public object selectitem;
    public Allitem all_item;
    public player player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<player>();

    }
    // Update is called once per frame
    void Update()
    {

    }

    public void Onclick()
    {
        if (selectitem is Fooddata)
        {
            Fooddata food = (Fooddata)selectitem;
            player.eat(food);
        }

        if (selectitem is Weapondata)
        {
            Weapondata weapon = (Weapondata)selectitem;
        }

    }
}
