using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private SpriteRenderer theSR;
    public Sprite DefaultImage;
    public Sprite PressedImage;
    public GameObject[] left;

    public KeyCode KeyToPress;
    void Start()
    {
        theSR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyToPress))
        {
            theSR.sprite = PressedImage;
        }

        if (Input.GetKeyUp(KeyToPress))
        {
            theSR.sprite = DefaultImage;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // 첫 번째 터치 정보를 가져옴

            if (touch.phase == TouchPhase.Began)
            {
                Vector2 touchPosition = touch.position; // 터치된 위치
                RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    if (this.name == "ButtonLeft")
                        for (int i = 0; i < left.Length; i++)
                        {
                            left[i].gameObject.GetComponent<NoteObject>().Nooo();
                        }

                }
            }
        }
    }
}
