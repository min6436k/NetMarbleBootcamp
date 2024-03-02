using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItemController : MonoBehaviour
{
    private player PlayerController;

    private object ItemData;
    private bool Initialized = false, isAte = false;

    void Start()
    {
        GameObject PlayerObject = GameObject.FindGameObjectWithTag("Player");
        PlayerController = PlayerObject.GetComponent<player>();
    }

    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!this.Initialized || collision.tag != "Player")
            return;
        if (isAte)
            return;

        isAte = true;
        PlayerController.OnEatItem(ItemData);
        Destroy(gameObject);
    }

    public void Initialize(object ItemData)
    {
        this.ItemData = ItemData;
        Initialized = true;
    }
}
