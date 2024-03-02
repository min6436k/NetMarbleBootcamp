using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bag : MonoBehaviour
{
    public GameObject inventory;
    public Transform inv_icon;
    public Allitem allitem;
    public GameObject uIPrefab;

    public int itemvolume;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
    }

    public void OnClick()
    {
        inventory.SetActive(!inventory.activeSelf);
        this.gameObject.SetActive(!this.gameObject.activeSelf);
        Transform order = transform.parent.parent.transform;
        for (int i = 0; i < order.childCount; i++)
        {
            if(order.GetChild(i) != inventory && order.GetChild(i) != transform.parent)
                order.GetChild(i).gameObject.SetActive(!order.GetChild(i).gameObject.activeSelf);


        }
    }

    public void loaditem(List<int> initem)
    {
        for (int i = 0; i < inv_icon.transform.childCount; i++)
            Destroy(inv_icon.transform.GetChild(i).gameObject);

        GameObject infoicon = inventory.transform.Find("info_icon").gameObject;
        for (int i = 0; i < initem.Count; i++)
        {
            var compo = Instantiate(uIPrefab.gameObject).GetComponent<itemtouch>();
            compo.fooddata = allitem.ALLFood[initem[i]];
            compo._istype = itemtouch.istype.food;
            compo.transform.SetParent(inv_icon);
            compo.transform.localScale = Vector3.one;
            compo.GetComponent<itemtouch>().info_icon = infoicon;
        }
        /*for (int i = 0; i < item.ALLWeapon.Length; i++)
        {
            var compo = Instantiate(uIPrefab.gameObject).GetComponent<Image>();
            compo.sprite = item.ALLWeapon[i].icon;
            compo.transform.parent = inv_icon;
            compo.transform.localScale = Vector3.one;
        }*/
    }
}
