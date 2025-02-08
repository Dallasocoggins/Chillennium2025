using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
[RequireComponent(typeof(PolygonCollider2D))]
public class RadialLightPhysics : LightPhysics
{
    public int points = 12;

    private new Light2D light;
    private new PolygonCollider2D collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        light = GetComponent<Light2D>();
        collider = GetComponent<PolygonCollider2D>();

        collider.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        var colliderPoints = new Vector2[points];
        var lightRadius = light.pointLightOuterRadius;
        for (int i = 0; i < points; i++)
        {
            var angle = (float) i / points * 2 * Mathf.PI;
            var direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
            var hit = Physics2D.Raycast(transform.position, direction, lightRadius, 1 << 3);
            var actualRadius = hit ? hit.distance : lightRadius;
            colliderPoints[i] = direction * actualRadius;
        }
        collider.points = colliderPoints;
    }
}
