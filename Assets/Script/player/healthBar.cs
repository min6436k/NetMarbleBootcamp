using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthBar : MonoBehaviour
{
    private Slider silder;
    private player player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<player>();
        silder = GetComponent<Slider>();
    }

    void Update()
    {
        HandleHpBar(player.hungry / (float) player.MaxHungry);
    }

    public void HandleHpBar(float imsi)
    {
        silder.value = Mathf.Lerp(silder.value, imsi, Time.deltaTime * 10);
    }
}
