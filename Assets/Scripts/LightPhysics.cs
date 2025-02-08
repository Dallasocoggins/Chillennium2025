using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LightPhysics : MonoBehaviour
{
    public List<Transform> teleportPoints;
    private Collider2D colliderComponent;
    public bool freezingOn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colliderComponent = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyMovement enemy = collision.gameObject.GetComponent<EnemyMovement>();
        if (enemy != null && freezingOn)
        {
            enemy.IncreaseLights(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        EnemyMovement enemy = collision.gameObject.GetComponent<EnemyMovement>();
        if (enemy != null && freezingOn)
        {
            enemy.DecreaseLights(this);
        }
    }

}
