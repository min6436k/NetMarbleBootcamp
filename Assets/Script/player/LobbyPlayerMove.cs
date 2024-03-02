using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayerMovement : MonoBehaviour
{
    Rigidbody2D rigid;
    public FloatingJoystick joystick;
    public float Speed = 5;
    float h,v;
    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        h = joystick.Horizontal;
        v = joystick.Vertical;
    }

    void FixedUpdate()
    {
        Vector2 dir = new Vector2(h, v);
        if (h != 0 && v != 0) dir /= 1.4f;
        if (Input.GetKey(KeyCode.LeftShift)) dir *= 2.5f;
        transform.Translate(dir * Speed * Time.deltaTime);
    }
}
