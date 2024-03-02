using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator animator;
    public FloatingJoystick joystick;
    public float speed = 4;
    public float jumpForce = 25;
    float h;
    public bool isJumped = false;
    private bool runbool = false;
    public bool grounded;

    public bool Moving;
    float timer = 0.0f; // 레이캐스트가 닿아있는 시간을 추적하기 위한 타이머
    float maxGroundedTime = 0.1f;
    public void run()
    {
        runbool = !runbool;
    }
    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Moving)
        {
            h = joystick.Horizontal;
            if (math.sign(h) != math.sign(transform.localScale.x) && h != 0) transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    public void notmove()
    {
        Moving = false;
    }

    void FixedUpdate()
    {
        CheckGround();
        Jump();
        if (Moving)
        {
            float move = h * speed;
            if (runbool)
            {
                move *= 2;
                animator.SetFloat("runspeed", 1.2f);
            }
            else animator.SetFloat("runspeed", 0.6f);


            if (isJumped)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift)) runbool = true;
                if (Input.GetKeyUp(KeyCode.LeftShift)) runbool = false;
            }

            rigid.AddForce(Vector2.right * move * 8 * Time.fixedDeltaTime, ForceMode2D.Impulse);
            if (math.abs(rigid.velocity.x) > math.abs(move) && move != 0) rigid.velocity = new Vector2(move, rigid.velocity.y);
            if (move == 0)
            {
                animator.SetBool("run", false);
                if (grounded) rigid.velocity = new Vector2(rigid.velocity.x * (math.abs(rigid.velocity.normalized.x) < 0.2 ? 0 : 0.8f), rigid.velocity.y);
                else rigid.velocity = new Vector2(rigid.velocity.x * 0.97f, rigid.velocity.y);
            }
            else
            {
                animator.SetBool("run", true);
            }

        }
    }

    private void Jump()
    {
        if ((joystick.Vertical >= 0.5) && grounded && Moving)
            isJumped = true;
        if (isJumped)
        {
            isJumped = false;
            Vector3 jumpVelocity = Vector2.up * Mathf.Sqrt((runbool ? jumpForce : jumpForce * 2/3) * -Physics.gravity.y);

            rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);

        }
    }

    private void CheckGround()
    {
        Debug.DrawRay(transform.position, Vector2.down*0.02f, Color.green,0);

        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, Vector2.down, 0.02f, LayerMask.GetMask("Ground"));
        RaycastHit2D mobhead = Physics2D.Raycast(transform.position, Vector2.down, 0.001f, LayerMask.GetMask("mob"));
        if (raycastHit.collider != null)
        {
            timer += Time.deltaTime;

            if (timer >= maxGroundedTime)
            {
                grounded = true;
                Moving = true;
                timer = 0;
                animator.SetBool("knockback", false);
            }
        }
        else
        {
            timer = 0;
            grounded = false;
        }

        if(mobhead.collider != null && !mobhead.collider.isTrigger)
        {
            Debug.Log("wq");
            Moving = false;
            rigid.velocity = rigid.velocity.normalized * 8;
        }

    }


    private IEnumerator Wait(float t)
    {
        yield return new WaitForSeconds(t);
    }
}