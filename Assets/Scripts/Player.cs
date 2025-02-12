using System.Collections;
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
    public float flameLightPoints;

    public float keyUIGrowTime;
   
    private new Rigidbody2D rigidbody;
    private BoxCollider2D boxCollider;
    private Animator animator;
    private Light2D candle;
    private RadialLightPhysics candlePhysics;
    private CandleEnergyUI candleEnergyUI;
    private GameObject keyUI;
    private FadeScreen fadeScreen;

    // -1 is the idle value
    private float jumpProgress = -1;

    private float lightBurstCurrentCooldown;
    private float keyUIProgress = 0;

    private float moveInput;
    private bool jumpInput;
    private bool downInput;
    private bool _candleOn = true;
    public bool candleOn
    {
        get => _candleOn;
        private set {
            _candleOn = value;

            if (candleEnergyUI != null) candleEnergyUI.SetCandleOn(value);
        }
    }
    private int keysCollected = 0;

    private UIScript ui;

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
        keyUI = GameObject.Find("KeyUI");
        fadeScreen = FindAnyObjectByType<FadeScreen>();

        if (keyUI != null)
        {
            keyUI.transform.localScale = Vector3.zero;
        }

        ui = FindAnyObjectByType<UIScript>();
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
        if (candleEnergyUI != null)
        {
            candleEnergyUI.proportionLeft = currentLightPoints / maxLightPoints;
        }
        
        if (transform.position.y < -50)
        {
            Die();
        }

        if (keysCollected == 1)
        {
            keyUIProgress = Mathf.Min(keyUIProgress + Time.deltaTime, 1);
        }
        else
        {
            keyUIProgress = Mathf.Max(keyUIProgress - Time.deltaTime, 0);
        }

        if (keyUI != null)
        {
            keyUI.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, Mathf.SmoothStep(0, 1, keyUIProgress / keyUIGrowTime));
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
            var epsilon = 0.01f;
            if (IsGrounded(true, false) && jumpProgress == -1 && rigidbody.linearVelocityY < epsilon && downInput)
            {
                IsGrounded(true, true);
            }
            else
            {
                if (IsGrounded() && jumpProgress == -1 && rigidbody.linearVelocityY < epsilon)
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

    private bool IsGrounded(bool justAgainstPlatform = false, bool tellPlatformToLetUsDown = false)
    {
        float epsilon = 0.1f;
        var hit = Physics2D.BoxCast((Vector2)transform.position + boxCollider.offset + Vector2.down * boxCollider.edgeRadius, boxCollider.size, 0, Vector2.down, epsilon, 1 << 3);
        if (hit)
        {
            var output = !Physics2D.GetIgnoreCollision(boxCollider, hit.collider);
            if (output)
            {
                var platform = hit.collider.GetComponent<OneWayPlatform>();
                if (justAgainstPlatform)
                {
                    output = platform != null;
                }

                if (platform != null && tellPlatformToLetUsDown)
                {
                    platform.LetPlayerThrough();
                }
            }
            return output;
        }
        return false;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<float>();
    }

    public void OnJump(InputValue value)
    {
        jumpInput = value.isPressed;
    }

    public void OnDown(InputValue value)
    {
        downInput = value.isPressed;
    }

    public void OnPause(InputValue value)
    {
        ui.TogglePause();
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
            animator.SetTrigger("lightBurst");
            candleEnergyUI.LightBurst();
        }
    }

    public void Eat()
    {
        animator.SetLayerWeight(4, 1);
        animator.SetLayerWeight(0, 0);
        animator.SetLayerWeight(1, 0);
        animator.SetLayerWeight(2, 0);
        animator.SetLayerWeight(3, 0);
        animator.SetTrigger("eaten");
        StartCoroutine(DieAfterDelay(2f));
        FindAnyObjectByType<MusicManager>().Eaten();
    }

    public IEnumerator DieAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Die();
    }

    public void Die()
    {
        print("Died");
        FindAnyObjectByType<MusicManager>().Die();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CollectKey()
    {
        keysCollected++;
    }

    public void CollectFlame()
    {
        currentLightPoints = Mathf.Min(currentLightPoints + flameLightPoints, maxLightPoints);
    }

    public void UnlockDoor(Door door)
    {
        if (keysCollected == door.NumberOfKeysNeeded)
        {
            keysCollected = 0;
            Time.timeScale = 0;
            fadeScreen.FadeToBlack();
        }
    }
}
