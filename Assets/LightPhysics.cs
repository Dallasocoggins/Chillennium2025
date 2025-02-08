using UnityEngine;

public class LightPhysics : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
            enemy.SetFreeze(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        EnemyMovement enemy = collision.gameObject.GetComponent<EnemyMovement>();
        if (enemy != null)
        {
            enemy.SetFreeze(false);
        }
    }
}
