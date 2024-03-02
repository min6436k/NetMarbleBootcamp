using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FryingPan : MonoBehaviour
{
    public GameObject Pot;
    public GameObject InventoryUI;
    public GameObject SelectButton;
    

    public void FryingPanClick()
    {
        Pot.SetActive(false);
        InventoryUI.SetActive(true);
        SelectButton.SetActive(true);
        gameObject.SetActive(false);
    }
}
