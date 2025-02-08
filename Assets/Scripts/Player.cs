using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
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
    public float baseCandleIntensity;
    public float candleFlickerFactor;

    private new Rigidbody2D rigidbody;
    private BoxCollider2D boxCollider;
    private Animator animator;
    private Light2D candle;
    private RadialLightPhysics candlePhysics;

    // -1 is the idle value
    private float jumpProgress = -1;

    private float moveInput;
    private bool jumpInput;
    public bool candleOn = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        candle = GetComponentInChildren<Light2D>();
        candlePhysics = GetComponentInChildren<RadialLightPhysics>();
        candlePhysics.freezingOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        float epsilon = 0.01f;
        if (Mathf.Abs(rigidbody.linearVelocityX) > epsilon)
        {
            sprite.localScale = new Vector3(Mathf.Sign(rigidbody.linearVelocityX), 1, 1);
        }

        candle.intensity = baseCandleIntensity * candleFlickerFactor;
        animator.SetBool("candleOn", candleOn);
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
        return Physics2D.BoxCast((Vector2)transform.position + boxCollider.offset, boxCollider.size, 0, Vector2.down, epsilon, 1 << 3);
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<float>();
    }

    public void OnJump(InputValue value)
    {
        jumpInput = value.isPressed;
    }

    public void OnLightToggle()
    {
        candleOn = !candleOn;
    }

    public void Die()
    {
        print("Died");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
