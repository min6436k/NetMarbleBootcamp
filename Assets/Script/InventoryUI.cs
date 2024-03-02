using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    
    public Transform inv_icon;
    public Allitem item;
    public GameObject uIPrefab;

    public int itemvolume;

    // Start is called before the first frame update
    void Start()
    {
        loaditem();
    }

    private void Update()
    {
        if (item.ALLFood.Count != itemvolume)
            loaditem();
    }

    public void loaditem()
    {
        itemvolume = item.ALLFood.Count;

        for (int i = 0; i < item.ALLFood.Count; i++)
        {
            var compo = Instantiate(uIPrefab.gameObject).GetComponent<Image>();
            compo.sprite = item.ALLFood[i].icon;
            compo.transform.parent = inv_icon;
            compo.transform.localScale = Vector3.one;
        }
    }
}
