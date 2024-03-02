using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoCook : MonoBehaviour
{

    public void GoCooked()
    {
        SceneManager.LoadScene("Cook");
    }
}
