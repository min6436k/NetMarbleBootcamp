using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject S_P;
    public GameObject enemy;
    public GameObject next;
    public GameObject UI;
    public GameObject Weapon1;
    public GameObject Weapon2;
    public GameObject run;
    public GameObject Interaction;

    public GameObject p;

    public GameObject CineCam;

    public GameObject UIInteraction;

    private void Awake()
    {
        p = GameObject.FindWithTag("Player");
        Prefabset();
        //sponenemy();

        Destroy(S_P);
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies != null && enemies.Length == 0 && next != null)
        {
            next.SetActive(true);
        }
    }

    private void sponenemy()
    {
        for (int i = 0; i < Random.Range(1, 3); i++)
        {
            Instantiate(enemy, new Vector2(Random.Range(-8, 10), Random.Range(-0.4f, 6)), Quaternion.identity);
        }
    }
    private void Pset()
    {

    }

    private void Prefabset()
    {
        var pre_ui = Instantiate(UI, GameObject.Find("Canvas").transform);

        //var p = Instantiate(player, S_P.transform.position, Quaternion.identity);
        p.GetComponent<PlayerMovement>().joystick = pre_ui.transform.Find("Floating Joystick").GetComponent<FloatingJoystick>();

        p.GetComponent<player>().respon = pre_ui.transform.Find("respon").gameObject;

        if(CineCam != null)
            Instantiate(CineCam, Vector3.zero, Quaternion.identity).GetComponent<CinemachineVirtualCamera>().Follow = p.transform;
        Instantiate(Weapon1, GameObject.Find("Canvas").transform).GetComponent<Button>().onClick.AddListener(p.GetComponent<player>().Attack1);
        Instantiate(Weapon2, GameObject.Find("Canvas").transform).GetComponent<Button>().onClick.AddListener(p.GetComponent<player>().Attack2);
        Instantiate(run, GameObject.Find("Canvas").transform).GetComponent<Button>().onClick.AddListener(p.GetComponent<PlayerMovement>().run);
        UIInteraction = Instantiate(Interaction, GameObject.Find("Canvas").transform);
        UIInteraction.GetComponent<Button>().onClick.AddListener(p.GetComponent<PlayerInteraction>().OnInteraction);

        p.GetComponent<player>().bag = pre_ui.transform.Find("Bag").gameObject;
    }
}
