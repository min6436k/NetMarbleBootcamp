using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour
{
    public GameObject FryingPan;
    public GameObject InventoryUI;
    public GameObject SelectButton;

    public void PotClick()
    {
        FryingPan.SetActive(false);
        InventoryUI.SetActive(true);
        SelectButton.SetActive(true);
        gameObject.SetActive(false);
    }
}
