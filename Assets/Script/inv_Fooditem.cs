using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inv_Fooditem : MonoBehaviour
{
    public Fooddata fooddata;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Image>().sprite = fooddata.icon;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
