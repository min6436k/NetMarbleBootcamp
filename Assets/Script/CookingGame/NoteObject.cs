using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public bool CanBePressed;

    // Start is called before the first frame update
    public KeyCode KeyToPress;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Activator")
        {
            CanBePressed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (gameObject.active)
        {
            if (other.tag == "Activator")
            {
                CanBePressed = false;
                //GameManager.instance.NoteMissed();

            }
        }

    }


    public void Nooo()
    {
        if (CanBePressed)
        {
            //GameManager.instance.NoteHit();

            gameObject.SetActive(false);
        }
    }


}
