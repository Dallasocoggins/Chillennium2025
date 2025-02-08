using System.Collections.Generic;
using Unity.VisualScripting;
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

    public GameObject lightBurstPrefab;
    public float lightBurstCooldown;

    public float maxLightPoints;
    public float currentLightPoints;
    public float candlePointDrain; // points / s
    public float lightBurstCost;
   
    private new Rigidbody2D rigidbody;
    private BoxCollider2D boxCollider;
    private Animator animator;
    private Light2D candle;
    private RadialLightPhysics candlePhysics;
    private CandleEnergyUI candleEnergyUI;

    // -1 is the idle value
    private float jumpProgress = -1;

    private float lightBurstCurrentCooldown;

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

        candleEnergyUI = FindAnyObjectByType<CandleEnergyUI>();
    }

    // Update is called once per frame
    void Update()
    {
        candle.intensity = baseCandleIntensity * candleFlickerFactor;
        animator.SetBool("candleOn", candleOn);
        int moveSign = (int)Mathf.Sign(moveInput);
        var epsilon = 0.1f;
        if (Mathf.Abs(moveInput) < epsilon)
        {
            moveSign = 0;
        }
        animator.SetInteger("moveSign", moveSign);
        animator.SetBool("isGrounded", IsGrounded());
        candleEnergyUI.proportionLeft = currentLightPoints / maxLightPoints;
        
        if (transform.position.y < -10)
        {
            Die();
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
                animator.SetTrigger("jump");
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

        lightBurstCurrentCooldown = Mathf.Max(lightBurstCurrentCooldown - Time.fixedDeltaTime, 0);
        if (candleOn)
        {
            currentLightPoints -= candlePointDrain * Time.fixedDeltaTime;
        }
        if (currentLightPoints <= 0)
        {
            candleOn = false;
        }
    }

    private bool IsGrounded()
    {
        float epsilon = 0.1f;
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
        if (currentLightPoints <= 0)
        {
            candleOn = false;
        }
    }

    public void OnLightBurst()
    {
        if (lightBurstCurrentCooldown <= 0 && currentLightPoints >= lightBurstCost)
        {
            Instantiate(lightBurstPrefab, candle.transform.position, Quaternion.identity);
            lightBurstCurrentCooldown = lightBurstCooldown;
            currentLightPoints -= lightBurstCost;
        }
    }

    public void Die()
    {
        print("Died");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
