using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class itemtouch : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject info_icon;
    public AudioSource audioSource;
    public useButton USE;
    public enum istype
    {
        food,
        weapon
    }

    public istype _istype;
    public Fooddata fooddata;
    public Weapondata weapondata;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        USE = GameObject.FindGameObjectWithTag("USE").GetComponent<useButton>();
        this.gameObject.GetComponent<Image>().sprite = fooddata.icon;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Onclick()
    {
        if (_istype == istype.food)
        {
            USE.selectitem = fooddata;
            audioSource.clip = fooddata.toush_sound;
        }else if (_istype == istype.weapon)
        {
            USE.selectitem = weapondata;
            audioSource.clip = weapondata.touch;
        }

        audioSource.Play();

        if (transform.parent.name == "inv_icon" && _istype == istype.food)
        {
            Image target = info_icon.GetComponent<Image>();

            if (target.sprite != fooddata.icon)
            {
                target.sprite = fooddata.icon;
                info_icon.GetComponentInChildren<TextMeshProUGUI>().text = fooddata.Data.name + "\n" + fooddata.Data.info;
                info_icon.SetActive(true);
            }

        }
    }
}
