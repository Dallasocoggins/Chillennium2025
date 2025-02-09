using UnityEngine;

public class ScreenDetecter : MonoBehaviour
{
    private Collider2D colliderComponent;
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
        if (enemy != null)
        {
            enemy.onScreen = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        EnemyMovement enemy = collision.gameObject.GetComponent<EnemyMovement>();
        if (enemy != null)
        {
            enemy.onScreen = false;
        }
    }
}
