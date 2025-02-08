using System;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class EnemyMovement : MonoBehaviour
{
    enum EnemyState { Chase, Teleport, Freeze }

    public float moveSpeed = 2f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.5f;
    public Transform target;
    public float teleportDistance = 10;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private EnemyState currentState = EnemyState.Chase;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        bool shouldTeleport = yDist > 2 || xDist > 5;
        if (shouldTeleport)
        {
            currentState = EnemyState.Teleport;
        }
        else
        {
            currentState = EnemyState.Chase;
        }
    }

    private void Chase()
    {
        print("Chase");
    }

    private void Teleport()
    {
        print("Teleport");
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
}
