using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class weaponbuttencooltime : MonoBehaviour
{
    public GameObject cool;
    private GameObject player;
    private PlayerMovement moveScript;
    public UnityEngine.UI.Button button;
    public UnityEngine.UI.Image img_Skill;
    public float cooltime;
    private bool Moving;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        moveScript = player.GetComponent<PlayerMovement>();
        switch (this.name)
        {
            case "W1(Clone)": cooltime = player.transform.Find("W1").GetComponent<OnWeapon>().weapondata.cooltime;break;
            case "W2(Clone)": cooltime = player.transform.Find("W2").GetComponent<OnWeapon>().weapondata.cooltime;break;
        }

        button = GetComponent<UnityEngine.UI.Button>();
        img_Skill = cool.GetComponent<UnityEngine.UI.Image>();
    }

    private void Update()
    {
        Moving = moveScript.Moving;
    }

    public void Onclick()
    {
        if (Moving)
        {
            button.interactable = false;
            StartCoroutine(CoolTime(cooltime));
        }
    }

    IEnumerator CoolTime(float coolInSeconds)
    {
        float startTime = Time.time;
        float endTime = startTime + coolInSeconds;

        print("쿨타임 코루틴 실행");

        while (Time.time < endTime)
        {
            float remainingTime = endTime - Time.time;
            img_Skill.fillAmount = remainingTime / coolInSeconds;
            yield return null; // 다음 프레임까지 대기
        }

        button.interactable = true;
        print("쿨타임 코루틴 완료");
    }
}