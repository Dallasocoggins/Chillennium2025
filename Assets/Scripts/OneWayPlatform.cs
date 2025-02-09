using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class OneWayPlatform : MonoBehaviour
{
    public float playerLetThroughCooldown = 0.4f;

    private new BoxCollider2D collider;
    private BoxCollider2D playerCollider;
    private Transform playerTransform;
    private float currentPlayerLetThroughCooldown = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        var player = FindAnyObjectByType<Player>();
        playerTransform = player.transform;
        playerCollider = player.GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        var ignoreCollision = playerTransform.position.y + playerCollider.bounds.min.y < transform.position.y + collider.bounds.max.y || currentPlayerLetThroughCooldown > 0;
        Physics2D.IgnoreCollision(collider, playerCollider, ignoreCollision);

        currentPlayerLetThroughCooldown -= Time.fixedDeltaTime;
    }

    public void LetPlayerThrough()
    {
        currentPlayerLetThroughCooldown = playerLetThroughCooldown;
    }
}
