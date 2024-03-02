using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BGlist
{
    public Transform[] BG_layer = null;
    public float speed = 1f;

    [NonSerialized] public float lpx = 0f;
    [NonSerialized] public float rpx = 0f;
    [NonSerialized] public float lpx2 = 0f;
    [NonSerialized] public float rpx2 = 0f;
}


public class BGScroll : MonoBehaviour
{

    public BGlist[] BG_list;
    private float[] Allspeed;
    private GetSpeed cm;
    private float starty;
    private Vector3 localPosition;

    void Start()
    {
        cm = transform.parent.GetComponent<GetSpeed>();

        for (int i  = 0;  i < BG_list.Length; i++)
        {
            float t_length = BG_list[i].BG_layer[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x;
            BG_list[i].lpx = -1.5f * t_length;
            BG_list[i].rpx = t_length * BG_list[i].BG_layer.Length;
            BG_list[i].rpx2 = 1.5f * t_length;
            BG_list[i].lpx2 = t_length * BG_list[i].BG_layer.Length;
        }
        starty = transform.parent.position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Allspeed = cm.getSpeed();
        if (Allspeed[0] != 0f)
        {
            for (int i = 0; i < BG_list.Length; i++)
            {

                float newspeed = BG_list[i].speed * Allspeed[0];

                for (int j = 0; j < BG_list[i].BG_layer.Length; j++)
                {
                    BG_list[i].BG_layer[j].localPosition = new Vector3(BG_list[i].BG_layer[j].transform.localPosition.x, (starty - Allspeed[1])* BG_list[i].speed/15, BG_list[i].BG_layer[j].transform.localPosition.z);
                    BG_list[i].BG_layer[j].localPosition += new Vector3(newspeed, 0) * Time.deltaTime;

                    if (BG_list[i].BG_layer[j].localPosition.x < BG_list[i].lpx && newspeed < 0)
                    {
                        localPosition = BG_list[i].BG_layer[j].localPosition;
                        localPosition.x += BG_list[i].rpx;
                        BG_list[i].BG_layer[j].localPosition = localPosition;

                    }

                    if (BG_list[i].BG_layer[j].localPosition.x > BG_list[i].rpx2 && newspeed > 0)
                    {
                        localPosition = BG_list[i].BG_layer[j].localPosition;
                        localPosition.x -= BG_list[i].lpx2;
                        BG_list[i].BG_layer[j].localPosition = localPosition;

                    }

                }
            }
        }
    }
}
