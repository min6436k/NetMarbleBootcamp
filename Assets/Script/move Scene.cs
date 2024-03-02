using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class moveScene : MonoBehaviour
{
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("이동");
            switch (this.name)
            {
                case "Go_Lobby": SceneManager.LoadScene("Lobby"); break;
                case "Go_Stage": SceneManager.LoadScene("Nomal1"); break;

                //스테이지
                case "N1_next": SceneManager.LoadScene("Nomal2"); break;
                case "N2_next": SceneManager.LoadScene("NomalBoss"); break;

                case "N1_prev": SceneManager.LoadScene("Lobby"); break;
                case "N2_prev": SceneManager.LoadScene("Nomal1"); break;
                case "N_B_prev": SceneManager.LoadScene("Nomal2"); break;


                //로비
                case "Go_CookingRoom": SceneManager.LoadScene("CookingRoom"); break;
            }
            
        }               

    }

    public void Back()
    {
        SceneManager.LoadScene("Lobby");
    }
}
