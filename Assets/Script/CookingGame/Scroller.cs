using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : MonoBehaviour
{
    public float BeatTempo;

    public bool HasStarted;

    // Start is called before the first frame update
    void Start()
    {
        BeatTempo = BeatTempo / 60f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!HasStarted)
        {
            if (Input.anyKeyDown)
            {
                HasStarted = true;
            }
        }
        else
        {
            transform.position -= new Vector3(0f, BeatTempo * Time.deltaTime, 0f);
        }

    }
}
