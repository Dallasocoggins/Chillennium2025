using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.FilePathAttribute;
using static UnityEngine.GraphicsBuffer;

public class EnemyMovement : MonoBehaviour
{
    enum EnemyState { Chase, Teleport, Freeze }

    public float moveSpeed = 2f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.5f;
    public float lightCheckRadius = 5.0f;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(RecordTransform());
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
        }

        if(currentState != EnemyState.Freeze) UpdateState();
    }

    private void UpdateState()
    {
        float xDist = Math.Abs(this.transform.position.x - target.position.x);
        float yDist = Math.Abs(this.transform.position.y - target.position.y);
        bool shouldTeleport = yDist > 2 || xDist > 6;
        if (shouldTeleport && currentState == EnemyState.Chase)
        {
            currentState = EnemyState.Teleport;
            SetTimeToTeleport();
        }
        else if (!shouldTeleport)
        {
            currentState = EnemyState.Chase;
        }
    }

    private void Chase()
    {
        if (target == null) return;
        positionQueue.Clear();

        Vector2 direction = (target.position - transform.position).normalized; 
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y); 

        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
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

        for (int i = 1; i <= searchRadius; i++) 
        {
            for (int j = 0; j < searchSteps; j++) 
            {
                float angle = j * (360f / searchSteps);
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

            timeTillTeleport = dist / moveSpeed;
        }
    }

    private void Freeze()
    {
        print("Freeze");
    }

    public void SetFreeze(bool on)
    {
        if (on)
        {
            currentState = EnemyState.Freeze;
        }
        else
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

}
