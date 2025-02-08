using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;

public class EnemyMovement : MonoBehaviour
{
    enum EnemyState { Chase, Teleport, Freeze, Eating }

    public float moveSpeed = 2f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.5f;
    public float lightCheckRadius = 5.0f;
    public Player player;
    public Transform target;
    public float teleportDistance = 10;
    public float teleportOffset = 5.0f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private EnemyState currentState = EnemyState.Chase;

    public Queue<Vector3> positionQueue = new Queue<Vector3>();
    private float lastTeleportTime = 0;
    private float timeTillTeleport = 3f;

    private int lightsOnMe = 0;
    private bool playerLightOnMe = false;
    private float eatingTime;

    public bool onScreen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(RecordTransform());
        player = FindAnyObjectByType<Player>();
        target = player.transform;
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Teleport:
                Teleport();
                break;
            case EnemyState.Freeze:
                Freeze();
                break;
            case EnemyState.Eating:
                Eat();
                break;
        }

        UpdateState();
    }

    private void UpdateState()
    {
        float xDist = Math.Abs(this.transform.position.x - target.position.x);
        float yDist = Math.Abs(this.transform.position.y - target.position.y);
        bool shouldTeleport = (yDist > 4 || xDist > 10);
        if (shouldTeleport && currentState == EnemyState.Chase)
        {
            currentState = EnemyState.Teleport;
            SetTimeToTeleport();
        }
        else if (!shouldTeleport && (currentState != EnemyState.Freeze || !onScreen))
        {
            currentState = EnemyState.Chase;
        }

        if (xDist < 3)
        {
            if (player.candleOn)
            {
                playerLightOnMe = true;
            }
            else if (lightsOnMe <= 0)
            {
                playerLightOnMe = false;
                currentState = EnemyState.Eating;
            }
        } else
        {
            playerLightOnMe = false;
        }

        if ((lightsOnMe > 0 || playerLightOnMe) && onScreen)
        {
            currentState = EnemyState.Freeze;
        }
    }

    private void Eat()
    {
        eatingTime += Time.deltaTime;

        if (eatingTime >= 3f)
        {
            if (player != null)
            {
                player.Die();
                eatingTime = 0f;
            }
        }
    }


    private void Chase()
    {
        if (target == null) return;

        positionQueue.Clear();

        Vector3 direction = (target.position - transform.position).normalized;
        float moveStep = moveSpeed * Time.fixedDeltaTime;
        Vector3 nextPosition = transform.position + (direction * moveStep);

        if (IsGapAhead(direction))
        {
            return;
        }

        // Move towards the target
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        if ((direction.x > 0 && transform.localScale.x < 0) ||
            (direction.x < 0 && transform.localScale.x > 0))
            transform.localScale = -transform.localScale;
    }

    private bool IsGapAhead(Vector3 direction)
    {
        float checkDistance = 1.5f;
        Vector3 checkPosition = transform.position + (Vector3.right * checkDistance * Mathf.Sign(direction.x));

        RaycastHit2D hit = Physics2D.Raycast(checkPosition, Vector2.down, groundCheckDistance, groundLayer);

        return hit.collider == null; 
    }

    private void Teleport()
    {
        if (Time.time - lastTeleportTime >= timeTillTeleport && positionQueue.Count > 0)
        {
            Vector3 teleportLocation = positionQueue.Dequeue();
            LightPhysics lp = IsLightNearby(teleportLocation);

            bool usedLightTeleport = false;

            if (lp)
            {
                float myDistanceToTp = Vector3.Distance(this.transform.position, teleportLocation);
                Vector3 lpTeleportLocation = GetClosestTransform(lp.teleportPoints).position;
                float LpDistanceToTp = Vector3.Distance(lpTeleportLocation, teleportLocation);
                float myDistanceToLp = Vector3.Distance(this.transform.position, lpTeleportLocation);

                if (LpDistanceToTp < myDistanceToTp && myDistanceToLp < myDistanceToTp)
                {
                    teleportLocation = lpTeleportLocation;
                    usedLightTeleport = true;
                }
            }

            if (!usedLightTeleport)
            {
                Vector3 direction = (teleportLocation - transform.position).normalized;
                teleportLocation -= direction * teleportOffset;
            }

            if (!IsGrounded(teleportLocation))
            {
                teleportLocation = FindNearestGroundPosition(teleportLocation);
            }

            this.transform.position = teleportLocation;

            SetTimeToTeleport();
            lastTeleportTime = Time.time;

            print("Time till teleport" + timeTillTeleport);
        }
    }

    private Vector3 FindNearestGroundPosition(Vector3 origin)
    {
        float searchRadius = 5f;
        int searchSteps = 8;

        Vector3 directionToTarget = (target.position - origin).normalized;


        // Search in a half-circle in the direction of the target
        for (int i = 1; i <= searchRadius; i++)
        {
            for (int j = 0; j < searchSteps / 2; j++)
            {
                float angle = j * (180f / (searchSteps / 2));
                Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * i;
                Vector3 testPosition = origin + offset;

                if (IsGrounded(testPosition))
                {
                    return testPosition;
                }
            }
        }

        // If no valid position found, search the full 360 - degree circle
        for (int i = 1; i <= searchRadius; i++)
        {
            for (int j = 0; j < searchSteps; j++)
            {
                float angle = j * (360f / searchSteps); // 0 to 360 degrees
                Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * i;
                Vector3 testPosition = origin + offset;

                if (IsGrounded(testPosition))
                {
                    return testPosition;
                }
            }
        }

        return origin;
    }


    private void SetTimeToTeleport()
    {
        if (positionQueue.Count > 0)
        {
            float xDist = Math.Abs(this.transform.position.x - positionQueue.Peek().x);
            float yDist = Math.Abs(this.transform.position.y - positionQueue.Peek().y);
            float dist = xDist + yDist;

            timeTillTeleport = dist / (moveSpeed + 1);
        }
    }

    private void Freeze()
    {
        print("Freeze");
        if (!onScreen)
        {
            currentState = EnemyState.Chase;
        }
    }

    IEnumerator RecordTransform()
    {
        while (true) // Runs indefinitely
        {
            if (target != null && IsGrounded(target.position))
            {
                positionQueue.Enqueue(target.position);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private bool IsGrounded(Vector3 position)
    {
        float groundCheckDistance = 1f; // Adjust this based on your game's needs
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null; // Returns true if ground exists below the position
    }

    private LightPhysics IsLightNearby(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, lightCheckRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Light"))
            {
                return collider.GetComponent<LightPhysics>();
            }
        }

        Vector2 directionToTeleport = (position - transform.position).normalized;
        float distanceToTeleport = Vector3.Distance(transform.position, position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTeleport, distanceToTeleport, LayerMask.GetMask("Light"));

        if (hit.collider != null && hit.collider.CompareTag("Light"))
        {
            return hit.collider.GetComponent<LightPhysics>();
        }

        return null;
    }

    Transform GetClosestTransform(List<Transform> transforms)
    {
        if (transforms == null || transforms.Count == 0)
            return null;

        Transform closest = null;
        float minDistance = float.MaxValue;
        Vector3 currentPosition = transform.position;

        foreach (Transform t in transforms)
        {
            float distance = Vector3.Distance(currentPosition, t.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = t;
            }
        }

        return closest; 
    }

    public void IncreaseLights()
    {
        lightsOnMe++;
    }

    public void DecreaseLights()
    {
        lightsOnMe--;
        if(lightsOnMe <= 0)
        {
            currentState = EnemyState.Chase;
        }
    }

}
