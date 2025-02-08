using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    public float walkSpeed;
    public float walkAcceleration;
    public float turnaroundAcceleration;
    public float restGroundedAcceleration;
    public float restAirAcceleration;
    public float jumpSpeed;
    public float timeToPeakJumpSpeed;

    public Transform sprite;

    private new Rigidbody2D rigidbody;
    private BoxCollider2D boxCollider;

    // -1 is the idle value
    private float jumpProgress = -1;

    private float moveInput;
    private bool jumpInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float epsilon = 0.01f;
        if (Mathf.Abs(rigidbody.linearVelocityX) > epsilon)
        {
            sprite.localScale = new Vector3(-Mathf.Sign(rigidbody.linearVelocityX), 1, 1);
        }
    }

    private void FixedUpdate()
    {
        float targetVelocity = moveInput * walkSpeed;
        float targetVelocitySign = Mathf.Sign(targetVelocity);
        float rbVelocitySign = Mathf.Sign(rigidbody.linearVelocityX);

        if (Mathf.Abs(targetVelocity) > Mathf.Abs(rigidbody.linearVelocityX)
            && targetVelocitySign == rbVelocitySign)
        {
            rigidbody.linearVelocityX = rbVelocitySign * Mathf.Min(
                Mathf.Abs(rigidbody.linearVelocityX) + walkAcceleration * Time.fixedDeltaTime,
                Mathf.Abs(targetVelocity)
            );
        }
        else if (Mathf.Abs(targetVelocity) > 0
            && targetVelocitySign != rbVelocitySign)
        {
            rigidbody.linearVelocityX = rigidbody.linearVelocityX
                + targetVelocitySign * turnaroundAcceleration * Time.fixedDeltaTime;
        }
        else if (Mathf.Abs(targetVelocity) < Mathf.Abs(rigidbody.linearVelocityX))
        {
            float acceleration = IsGrounded() ? restGroundedAcceleration : restAirAcceleration;

            rigidbody.linearVelocityX = rbVelocitySign * Mathf.Max(
                Mathf.Abs(rigidbody.linearVelocityX) - acceleration * Time.fixedDeltaTime,
                0
            );
        }

        if (jumpInput)
        {
            if (IsGrounded() && jumpProgress == -1)
            {
                jumpProgress = 0;
            }

            if (jumpProgress != -1)
            {
                jumpProgress = Mathf.Min(jumpProgress + Time.deltaTime / timeToPeakJumpSpeed, 1);
                rigidbody.linearVelocityY = jumpSpeed * jumpProgress;
            }

            if (jumpProgress == 1)
            {
                jumpProgress = -1;
            }
        }
        else
        {
            jumpProgress = -1;
        }
    }

    private bool IsGrounded()
    {
        float epsilon = 0.01f;
        return Physics2D.BoxCast((Vector2)transform.position + boxCollider.offset, boxCollider.size, 0, Vector2.down, epsilon);
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<float>();
    }

    public void OnJump(InputValue value)
    {
        jumpInput = value.isPressed;
    }
}
